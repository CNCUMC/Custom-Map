using System;
using System.Collections;
using System.Linq;
using BepInEx.Logging;
using MossLib.Tool;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

namespace CustomFungamePack;

public static class MapLoader
{
    private const string LocaleKeyPre = "log.map_loader.";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    public static void LoadAndApplyMapFromFungame(Fungame fungame)
    {
        try
        {
            if (fungame?.MapData == null)
            {
                Error("load_error");
                return;
            }

            var mapData = fungame.MapData;

            if (mapData.Map == null || mapData.Map.Length == 0)
            {
                Error("invalid_format");
                return;
            }

            ParseAndApplyStringMap(fungame);

            Info("load_success", mapData.X, mapData.Y, mapData.Map.Length,
                mapData.Map.Max(row => row?.Length ?? 0));
        }
        catch (Exception ex)
        {
            Error("load_failed", ex.Message);
        }
    }

    private static void ParseAndApplyStringMap(Fungame fungame)
    {
        var mapData = fungame.MapData;
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            Warning("validation.no_data", ModLocale.GetFormat("log.common.map"), "string map");
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
            Warning("validation.row_data_empty", "string map");
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

        Info("string_map_applied", blockCount, itemCount, failCount);
    }

    private static void ProcessValue(object value, ref int worldX, ref int worldY, ref int blockCount, ref int itemCount, ref int failCount)
    {
        switch (value)
        {
            case JArray jArray:
            {
                ProcessListValue(jArray, ref worldX, ref worldY, ref blockCount, ref itemCount, ref failCount);
                break;
            }
            case long intValue:
            {
                PlaceBlock(intValue, worldX, worldY, ref blockCount, ref failCount);
                break;
            }
            case double doubleValue:
            {
                PlaceBlock((long)doubleValue, worldX, worldY, ref blockCount, ref failCount);
                break;
            }
            case string stringValue:
            {
                PlaceItem(stringValue, worldX, worldY, ref itemCount, ref failCount);
                break;
            }
        }
    }

    private static void ProcessListValue(JArray jArray, ref int worldX, ref int worldY, ref int blockCount, ref int itemCount, ref int failCount)
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
                            else if (longVal > 0)
                            {
                                PlaceBlock(longVal, worldX, worldY, ref blockCount, ref failCount);
                                hasPlacedBlock = true;
                            }
                            break;
                        case double doubleVal:
                            if (hasPlacedBlock)
                            {
                                Warning("multiple_blocks_in_list", worldX, worldY);
                            }
                            else if (doubleVal > 0)
                            {
                                PlaceBlock((long)doubleVal, worldX, worldY, ref blockCount, ref failCount);
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

    private static void PlaceBlock(long blockId, int x, int y, ref int blockCount, ref int failCount)
    {
        try
        {
            World.SetBlock(x, y, (ushort)blockId);
            blockCount++;
        }
        catch (Exception ex)
        {
            Error("place_failed", x, y, ModLocale.GetFormat("log.common.block"), blockId, ex.Message);
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
            Error("place_failed", x, y, ModLocale.GetFormat("log.common.item"), itemId, ex.Message);
            failCount++;
        }
    }
    
    public static void ReloadMap()
    {
        World.CheckForWorld();
        Log.Divider();
        try
        {
            var currentFungame = WorldGenerationPatch.CurrentFungame;

            if (currentFungame == null)
            {
                Error("no_current_fungame");
                return;
            }

            Info("restarting_scene");
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
            Info("scene_reloading", currentScene.name);

            SceneManager.LoadScene(currentScene.buildIndex);

            Info("scene_reloaded");
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
        LogConsole("info.feature", fungame.Features);
        LogConsole("info.spawn", fungame.SpawnPosition);
        Log.Divider();
        Log.NewLine();
    }
    
    private static void LogConsole(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"command.fungame.{key}", args);
        Log.Info(message, Logger);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
        Log.Info(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
        Log.Warning(message, Logger);
    }
}