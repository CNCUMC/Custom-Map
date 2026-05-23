using System;
using System.Linq;
using BepInEx.Logging;
using MossLib.Tool;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFungamePack;

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

            MoreLogs("load_success", mapData.X, mapData.Y, mapData.Map.Length,
                mapData.Map.Max(row => row?.Length ?? 0));
        }
        catch (Exception ex)
        {
            Error("load_failed", ex.Message);
        }
    }

    private static void LogFeatureInfo(Fungame fungame)
    {
        var feature = fungame.Feature;

        if (feature == null)
        {
            Warning("no_features_enabled");
            return;
        }

        var hasAnyFeature = false;

        if (feature.Fullbright)
        {
            MoreLogs("feature_enabled", ModLocale.GetFormat("feature.fullbright"));
            hasAnyFeature = true;
        }

        if (feature.ForgivingLevel)
        {
            MoreLogs("feature_enabled", ModLocale.GetFormat("feature.forgiving_level"));
            hasAnyFeature = true;
        }

        if (!Mathf.Approximately(feature.Gravity, Physics2D.gravity.y))
        {
            MoreLogs("feature_enabled_with_value", ModLocale.GetFormat("feature.gravity"), feature.Gravity);
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

        var worldY = mapData.Y;
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

            var worldX = mapData.X;

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
            World.SetBlock(x, y, (ushort)blockId);
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
            World.SetItem(x, y, itemId);
            itemCount++;
        }
        catch (Exception ex)
        {
            Error("place_failed", x, y, ModLocale.Log("common.item"), itemId, ex.Message);
            failCount++;
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
        var fungame = FungameCheck.CurrentFungame;
        Log.Divider();
        LogConsole("info.name", fungame.Name);
        LogConsole("info.id", fungame.Id);
        LogConsole("info.version", fungame.Version);
        LogConsole("info.authors", fungame.Authors);
        LogConsole("info.description", fungame.Description);
        LogConsole("info.features", fungame.Features);
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
        if (Configs.MoreLogs)
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
}