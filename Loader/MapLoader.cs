using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using CustomFungamePack.Patch;
using MossLib.Tool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFungamePack.Loader;

public static class MapLoader
{
    private const string LocaleKeyPre = "map_loader.";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    public static void LoadAndApplyMapFromFungame(Fungame fungame)
    {
        try
        {
            if (fungame == null)
            {
                Error("no_current_fungame");
                return;
            }

            var hasMapData = fungame.MapData != null;
            var hasCustomStructures = !string.IsNullOrEmpty(fungame.CustomStructures);

            switch (hasMapData)
            {
                case false when !hasCustomStructures:
                    Error("load_error");
                    return;
                case false:
                    Warning("custom_structures_not_supported", ModLocale.Log("common.map"));
                    return;
            }

            var mapData = fungame.MapData;

            if (mapData.Map == null || mapData.Map.Length == 0)
            {
                Error("invalid_format");
                return;
            }

            LogFeatureInfo(fungame);

            ParseAndApplyStringMap(fungame);

            MoreLogs("load_success", fungame.X, fungame.Y, mapData.Map.Length,
                mapData.Map.Max(row => row?.Length ?? 0));
        }
        catch (Exception ex)
        {
            Error("load_failed", ex.Message);
        }
    }

    private static void LogFeatureInfo(Fungame fungame)
    {
        var settings = fungame.WorldSettings;

        var hasAnyFeature = false;

        if (settings.Fullbright)
        {
            MoreLogs("feature_enabled", ModLocale.GetFormat("feature.fullbright"));
            hasAnyFeature = true;
        }

        if (settings.ForgivingLevel)
        {
            MoreLogs("feature_enabled", ModLocale.GetFormat("feature.forgiving_level"));
            hasAnyFeature = true;
        }

        if (!Mathf.Approximately(settings.Gravity, Physics2D.gravity.y))
        {
            MoreLogs("feature_enabled_with_value", ModLocale.GetFormat("feature.gravity"), settings.Gravity);
            hasAnyFeature = true;
        }

        if (fungame.SkipTerrain)
        {
            MoreLogs("skip_generation", ModLocale.Log("common.terrain"));
            hasAnyFeature = true;
        }

        if (fungame.SkipStructures)
        {
            MoreLogs("skip_generation", ModLocale.Log("common.structure"));
            hasAnyFeature = true;
        }

        if (fungame.SkipBackground)
        {
            MoreLogs("skip_generation", ModLocale.Log("common.background"));
            hasAnyFeature = true;
        }

        if (!hasAnyFeature)
        {
            Warning("no_features_enabled");
        }
    }

    private static void ParseAndApplyStringMap(Fungame fungame)
    {
        var mapData = fungame.MapData;
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            MoreLogs("validation.no_data", ModLocale.Log("common.map"), "string map");
            return;
        }

        if (mapData.Key == null || mapData.Key.Count == 0)
        {
            Error("key_missing");
            return;
        }

        var rowCount = mapData.Map.Length;
        var maxColCount = mapData.Map.Max(row => row?.Length ?? 0);

        if (maxColCount == 0)
        {
            MoreLogs("validation.row_data_empty", "string map");
            return;
        }

        var worldY = fungame.Y;
        var blockCount = 0;
        var itemCount = 0;
        var failCount = 0;

        for (int row = 0; row < rowCount; row++)
        {
            var mapRow = mapData.Map[row];
            if (string.IsNullOrEmpty(mapRow))
            {
                worldY--;
                continue;
            }

            var worldX = fungame.X;

            foreach (var charStr in mapRow.Select(t => t.ToString()))
            {
                if (!mapData.Key.TryGetValue(charStr, out var value))
                {
                    worldX++;
                    continue;
                }

                ProcessValue(value, ref worldX, ref worldY, ref blockCount, ref itemCount, ref failCount);

                worldX++;
            }

            worldY--;
        }

        MoreLogs("string_map_applied", blockCount, itemCount, failCount);
        PickItems(fungame);
    }

    private static void ProcessValue(object value, ref int worldX, ref int worldY, ref int blockCount,
        ref int itemCount, ref int failCount)
    {
        switch (value)
        {
            case JArray jArray:
            {
                ProcessListValue(jArray, ref worldX, ref worldY, ref blockCount, ref itemCount, ref failCount);
                break;
            }
            case long longValue:
            {
                PlaceBlock((int)longValue, worldX, worldY, ref blockCount, ref failCount);
                break;
            }
            case int intValue:
            {
                PlaceBlock(intValue, worldX, worldY, ref blockCount, ref failCount);
                break;
            }
            case ushort ushortValue:
            {
                PlaceBlock(ushortValue, worldX, worldY, ref blockCount, ref failCount);
                break;
            }
            case string stringValue:
            {
                PlaceItem(stringValue, worldX, worldY, ref itemCount, ref failCount);
                break;
            }
        }
    }

    private static void ProcessListValue(JArray jArray, ref int worldX, ref int worldY, ref int blockCount,
        ref int itemCount, ref int failCount)
    {
        if (jArray == null || jArray.Count == 0)
        {
            return;
        }

        bool hasPlacedBlock = false;

        foreach (var token in jArray)
        {
            switch (token)
            {
                case JValue jValue:
                {
                    var rawValue = jValue.Value;
                    switch (rawValue)
                    {
                        case long longVal:
                            if (hasPlacedBlock)
                            {
                                Warning("multiple_blocks_in_list", worldX, worldY);
                            }
                            else if (longVal >= 0)
                            {
                                PlaceBlock((int)longVal, worldX, worldY, ref blockCount, ref failCount);
                                hasPlacedBlock = true;
                            }

                            break;
                        case int intVal:
                            if (hasPlacedBlock)
                            {
                                Warning("multiple_blocks_in_list", worldX, worldY);
                            }
                            else if (intVal >= 0)
                            {
                                PlaceBlock(intVal, worldX, worldY, ref blockCount, ref failCount);
                                hasPlacedBlock = true;
                            }

                            break;
                        case string stringVal:
                            if (!string.IsNullOrEmpty(stringVal))
                            {
                                PlaceItem(stringVal, worldX, worldY, ref itemCount, ref failCount);
                            }

                            break;
                    }

                    break;
                }
            }
        }
    }

    private static void PlaceBlock(int blockId, int x, int y, ref int blockCount, ref int failCount)
    {
        try
        {
            World.PlaceBlock(x, y, (ushort)blockId);
            blockCount++;
        }
        catch (Exception ex)
        {
            Error("place_failed", x, y, ModLocale.Log("common.block"), blockId, ex.Message);
            failCount++;
        }
    }

    private static void PlaceItem(string itemId, int x, int y, ref int itemCount, ref int failCount)
    {
        try
        {
            World.PlaceItem(x, y, itemId);
            itemCount++;
        }
        catch (Exception ex)
        {
            Error("place_failed", x, y, ModLocale.Log("common.item"), itemId, ex.Message);
            failCount++;
        }
    }

    public static void ApplyBuildModeSave(string saveFilePath, int anchorX, int anchorY)
    {
        if (!File.Exists(saveFilePath))
        {
            Error("not_found_buildmode_save");
            return;
        }

        int width, height;
        ushort[,] blocks;
        byte[,] liquids;
        Dictionary<Vector2Int, string> backgrounds;

        using (FileStream input = new(saveFilePath, FileMode.Open))
        using (BinaryReader reader = new(input))
        {
            width = reader.ReadInt32();
            height = reader.ReadInt32();
            blocks = new ushort[width, height];
            liquids = new byte[width, height];
            backgrounds = new Dictionary<Vector2Int, string>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x, y] = reader.ReadUInt16();
                    liquids[x, y] = reader.ReadByte();
                    string bg = reader.ReadString();
                    if (!string.IsNullOrEmpty(bg))
                        backgrounds[new Vector2Int(x, y)] = bg;
                }
            }
        }

        int blockCount = 0;
        int liquidCount = 0;
        int bgCount = backgrounds.Count;
        int failCount = 0;

        int worldWidth = (int)WorldGeneration.world.width;
        int worldHeight = (int)WorldGeneration.world.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int worldX = anchorX + x;
                int worldY = anchorY + y;

                if (worldX < 0 || worldX >= worldWidth || worldY < 0 || worldY >= worldHeight)
                    continue;

                if (blocks[x, y] > 0)
                {
                    PlaceBlock(blocks[x, y], worldX, worldY, ref blockCount, ref failCount);
                }

                if (liquids[x, y] > 0 && FluidManager.main != null)
                {
                    try
                    {
                        FluidManager.main.SetLiquid(worldX, worldY, liquids[x, y]);
                        liquidCount++;
                    }
                    catch (Exception ex)
                    {
                        Error("place_failed", worldX, worldY, "liquid", liquids[x, y], ex.Message);
                        failCount++;
                    }
                }

                Vector2Int localPos = new(x, y);
                if (backgrounds.TryGetValue(localPos, out string bgId))
                {
                    World.PlaceBackground(new Vector2Int(worldX, worldY), bgId);
                }
            }
        }

        MoreLogs("build_mode_save_applied", blockCount, liquidCount, bgCount, failCount);
        for (int i = 0; i < 5; i++)
        {
            Player.Tp(FungameCheck.CurrentFungame.SpawnPosition);
        }

        PickItems(FungameCheck.CurrentFungame);
    }

    public static void ReloadFungameFromDisk(Fungame fungame)
    {
        if (fungame == null)
        {
            Error("no_current_fungame");
            return;
        }

        var directoryPath = fungame.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            Error("no_directory_path");
            return;
        }

        if (!Directory.Exists(directoryPath))
        {
            Error("fungame_json_not_found", directoryPath);
            return;
        }

        try
        {
            var reloadedFungame = FungameDirectoryLoader.LoadFromDirectory(directoryPath);

            if (reloadedFungame == null)
            {
                Error("fungame_deserialize_failed");
                return;
            }

            var index = FungameCheck.Fungames.FindIndex(f => f.Id == fungame.Id);
            if (index >= 0)
                FungameCheck.Fungames[index] = reloadedFungame;

            WorldGenerationPatch.CurrentFungame = reloadedFungame;

            MoreLogs("fungame_reloaded_from_disk", reloadedFungame.Name);
        }
        catch (Exception ex)
        {
            Error("fungame_reload_failed", ex.Message);
        }
    }

    public static void ReloadMap(Fungame fungame)
    {
        if (fungame == null)
        {
            Error("no_current_fungame");
            return;
        }

        World.CheckForWorld();
        Log.Divider();
        try
        {
            MoreLogs("restarting_scene");
            RestartScene();
        }
        catch (Exception ex)
        {
            Error("reload_failed", ex.Message);
        }

        PickItems(fungame);
    }

    private static void RestartScene()
    {
        try
        {
            var currentScene = SceneManager.GetActiveScene();
            MoreLogs("scene_reloading", currentScene.name);

            SceneManager.LoadScene(currentScene.buildIndex);

            MoreLogs("scene_reloaded");
        }
        catch (Exception ex)
        {
            Error("scene_reload_failed", ex.Message);
        }
    }

    public static void LogMapInfo()
    {
        var fungame = FungameCheck.CurrentFungame ?? FungameCheck.Fungames.FirstOrDefault();
        if (fungame == null)
        {
            Error("no_current_fungame");
            return;
        }

        Log.Divider();
        LogConsole("info.name", fungame.Name);
        LogConsole("info.id", fungame.Id);
        LogConsole("info.version", fungame.Version);
        LogConsole("info.authors", fungame.Authors);
        LogConsole("info.description", fungame.Description);
        LogConsole("info.features", fungame.ActiveFeatures);
        LogConsole("info.spawn", fungame.SpawnPosition);
        Log.Divider();
        Log.NewLine();
    }

    public static void LogFungameList()
    {
        var fungames = FungameCheck.Fungames;

        if (fungames == null || fungames.Count == 0)
        {
            LogConsole("list.empty");
            return;
        }

        Log.Divider();
        LogConsole("list.header", fungames.Count);

        for (int i = 0; i < fungames.Count; i++)
        {
            var fungame = fungames[i];
            var isCurrent = fungame.Id == FungameCheck.CurrentFungame?.Id;
            var marker = isCurrent ? "->" : "  ";

            LogConsole("list.item", marker, i + 1, fungame.Name, fungame.Id, fungame.Version, fungame.Authors);
        }

        Log.NewLine();
    }

    private static void PickItems(Fungame fungame)
    {
        var items = fungame.Items;
        foreach (var item in items)
        {
            Player.PickItem(item.Id, item.Slot, item.Force);
        }
    }

    private static void LogConsole(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"command.fungame.{key}", args);
        Log.Info(message, Logger);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Info(message, Logger);
    }

    private static void MoreLogs(string key, params object[] args)
    {
        if (ModConfigs.MoreLogs)
            Info(key, args);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Warning(message, Logger);
    }

    private static void NormalizeKey(JObject jsonObject, string pascalKey, string lowerKey)
    {
        if (!jsonObject.ContainsKey(lowerKey) && jsonObject.ContainsKey(pascalKey))
            jsonObject[lowerKey] = jsonObject[pascalKey];
    }
}