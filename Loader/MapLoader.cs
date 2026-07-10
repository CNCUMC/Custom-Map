using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx.Logging;
using CustomMap.Patch;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomMap.Loader;

public static class MapLoader
{
    private const string LocaleKeyPre = "map_loader.";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    public static void LoadAndApplyMapFromMap(Map map)
    {
        try
        {
            if (map == null)
            {
                Error("no_current_map");
                return;
            }

            var hasMapData = map.MapData != null;
            var hasCustomStructures = !string.IsNullOrEmpty(map.CustomStructures);

            switch (hasMapData)
            {
                case false when !hasCustomStructures:
                    Error("load_error");
                    return;
                case false:
                    Warning("custom_structures_not_supported", BetterLocale.GetCommand("map"));
                    return;
            }

            var mapData = map.MapData;

            if (mapData.Map == null || mapData.Map.Length == 0)
            {
                Error("invalid_format");
                return;
            }

            LogFeatureInfo(map);

            ParseAndApplyStringMap(map);

            MoreLogs("load_success", map.X, map.Y, mapData.Map.Length,
                mapData.Map.Max(row => row?.Length ?? 0));
        }
        catch (Exception ex)
        {
            Error("load_failed", ex.Message);
        }
    }

    private static void LogFeatureInfo(Map map)
    {
        var settings = map.WorldSettingsData;

        var hasAnyFeature = false;

        if (settings.FullBright)
        {
            MoreLogs("feature_enabled", BetterLocale.GetOther("feature.full_bright"));
            hasAnyFeature = true;
        }

        if (settings.ForgivingLevel)
        {
            MoreLogs("feature_enabled", BetterLocale.GetOther("feature.forgiving_level"));
            hasAnyFeature = true;
        }

        if (!Mathf.Approximately(settings.Gravity, Physics2D.gravity.y))
        {
            MoreLogs("feature_enabled_with_value", BetterLocale.GetOther("feature.gravity"), settings.Gravity);
            hasAnyFeature = true;
        }

        if (map.SkipTerrain)
        {
            MoreLogs("skip_generation", BetterLocale.GetOther("common.terrain"));
            hasAnyFeature = true;
        }

        if (map.SkipStructures)
        {
            MoreLogs("skip_generation", BetterLocale.GetOther("common.structure"));
            hasAnyFeature = true;
        }

        if (map.SkipBackground)
        {
            MoreLogs("skip_generation", BetterLocale.GetOther("common.background"));
            hasAnyFeature = true;
        }

        if (!hasAnyFeature) Warning("no_features_enabled");
    }

    private static void ParseAndApplyStringMap(Map map)
    {
        var mapData = map.MapData;
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            MoreLogs("validation.no_data", BetterLocale.GetOther("common.map"), "string map");
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

        // �����ܷ��������޳����кͿ��ַ���
        var totalBlocks = mapData.Map
            .Where(mapRow => !string.IsNullOrEmpty(mapRow))
            .Sum(mapRow => mapRow.Count(c => c != ' '));

        WorldGenerationPatch.TotalBlocks = totalBlocks;

        var successCount = 0;
        var failCount = 0;
        const int failLimit = 50;

        var startX = map.X;
        var startY = map.Y;

        var updateCounter = 0;
        var updateInterval = Plugin.ProgressUpdateInterval;

        for (var row = 0; row < rowCount; row++)
        {
            var mapRow = mapData.Map[row];
            if (string.IsNullOrEmpty(mapRow))
            {
                startY--;
                continue;
            }

            var worldX = startX;
            var worldY = startY;

            foreach (var charStr
                     in mapRow.Select(t => t.ToString()))
            {
                if (!mapData.Key.TryGetValue(charStr, out var value))
                {
                    worldX++;
                    continue;
                }

                WorldGenerationPatch.SuccessCount = successCount;
                WorldGenerationPatch.FailCount = failCount;
                ProcessValue(value, worldX, worldY, ref successCount, ref failCount, failLimit);
                worldX++;

                // ���ڸ��¼����ı���ʵ��ʵʱ������??
                if (++updateCounter % updateInterval == 0)
                    WorldGenerationPatch.RefreshLoadingText();
            }

            startY--;
        }

        WorldGenerationPatch.SuccessCount = successCount;
        WorldGenerationPatch.FailCount = failCount;
        MoreLogs("string_map_applied", successCount, failCount);
        PickItems(map);
    }

    private static IEnumerator ParseAndApplyStringMapAsync(Map map)
    {
        var mapData = map.MapData;
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            MoreLogs("validation.no_data", BetterLocale.GetOther("common.map"), "string map");
            yield break;
        }

        if (mapData.Key == null || mapData.Key.Count == 0)
        {
            Error("key_missing");
            yield break;
        }

        var rowCount = mapData.Map.Length;
        var maxColCount = mapData.Map.Max(row => row?.Length ?? 0);

        if (maxColCount == 0)
        {
            MoreLogs("validation.row_data_empty", "string map");
            yield break;
        }

        // �����ܷ��������޳����кͿ��ַ���
        var totalBlocks = mapData.Map
            .Where(mapRow => !string.IsNullOrEmpty(mapRow))
            .Sum(mapRow => mapRow.Count(c => c != ' '));

        WorldGenerationPatch.TotalBlocks = totalBlocks;
        WorldGenerationPatch._isSpawningMap = true;

        var successCount = 0;
        var failCount = 0;
        const int failLimit = 50;

        var startX = map.X;
        var startY = map.Y;

        var updateCounter = 0;
        var updateInterval = Plugin.ProgressUpdateInterval;

        for (var row = 0; row < rowCount; row++)
        {
            var mapRow = mapData.Map[row];
            if (string.IsNullOrEmpty(mapRow))
            {
                startY--;
                continue;
            }

            var worldX = startX;
            var worldY = startY;

            foreach (var charStr
                     in mapRow.Select(t => t.ToString()))
            {
                if (!mapData.Key.TryGetValue(charStr, out var value))
                {
                    worldX++;
                    continue;
                }

                WorldGenerationPatch.SuccessCount = successCount;
                WorldGenerationPatch.FailCount = failCount;
                ProcessValue(value, worldX, worldY, ref successCount, ref failCount, failLimit);
                worldX++;

                // ??0����??yield һ�Σ�??Unity ��Ⱦ���µĽ�����??
                if (++updateCounter % updateInterval != 0) continue;
                WorldGenerationPatch.SuccessCount = successCount;
                WorldGenerationPatch.FailCount = failCount;
                WorldGenerationPatch.RefreshLoadingText();
                yield return null;
            }

            startY--;
        }

        WorldGenerationPatch.SuccessCount = successCount;
        WorldGenerationPatch.FailCount = failCount;
        MoreLogs("string_map_applied", successCount, failCount);
        PickItems(map);
    }

    public static IEnumerator LoadAndApplyMapFromMapAsync(Map map)
    {
        // ע�⣺C# ��ֹ??try-catch ��ʹ??yield return??
        // �����֤??LogFeatureInfo ���� try ����ִ�У��쳣�ɵ����߲�??
        if (map == null)
        {
            Error("no_current_map");
            yield break;
        }

        var hasMapData = map.MapData != null;
        var hasCustomStructures = !string.IsNullOrEmpty(map.CustomStructures);

        switch (hasMapData)
        {
            case false when !hasCustomStructures:
                Error("load_error");
                yield break;
            case false:
                Warning("custom_structures_not_supported", BetterLocale.GetOther("common.map"));
                yield break;
        }

        var mapData = map.MapData;

        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            Error("invalid_format");
            yield break;
        }

        LogFeatureInfo(map);

        yield return ParseAndApplyStringMapAsync(map);

        MoreLogs("load_success", map.X, map.Y, mapData.Map.Length,
            mapData.Map.Max(row => row?.Length ?? 0));
    }

    private static void ProcessValue(object value, int x, int y, ref int blockCount, ref int failCount, int failLimit)
    {
        if (failCount >= failLimit)
            return;

        switch (value)
        {
            case long longVal and >= 0:
                PlaceBlock((int)longVal, x, y, ref blockCount, ref failCount);
                break;
            case int intVal and >= 0:
                PlaceBlock(intVal, x, y, ref blockCount, ref failCount);
                break;
            case ushort ushortVal:
                PlaceBlock(ushortVal, x, y, ref blockCount, ref failCount);
                break;
            case string stringVal when !string.IsNullOrEmpty(stringVal):
                PlaceItem(x, y, stringVal);
                break;
            case JArray jArray:
                ProcessListValue(jArray, x, y, ref blockCount, ref failCount, failLimit);
                break;
        }
    }

    private static void ProcessListValue(JArray jArray, int x, int y, ref int blockCount, ref int failCount,
        int failLimit)
    {
        if (jArray == null || jArray.Count == 0)
            return;

        foreach (var token in jArray)
        {
            if (token is not JValue jValue) continue;
            ProcessValue(jValue.Value, x, y, ref blockCount, ref failCount, failLimit);
        }
    }

    private static void PlaceBlock(int blockId, int x, int y, ref int blockCount, ref int failCount)
    {
        try
        {
            WorldUtil.PlaceBlock(x, y, (ushort)blockId);
            blockCount++;
        }
        catch (Exception ex)
        {
            Error("place_failed", x, y, BetterLocale.GetOther("common.block"), blockId, ex.Message);
            failCount++;
        }
    }

    private static void PlaceItem(int x, int y, string id)
    {
        try
        {
            WorldUtil.PlaceItem(x, y, id);
        }
        catch (Exception ex)
        {
            Error("place_failed", x, y, BetterLocale.GetOther("common.item"), id, ex.Message);
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

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                blocks[x, y] = reader.ReadUInt16();
                liquids[x, y] = reader.ReadByte();
                var bg = reader.ReadString();
                if (!string.IsNullOrEmpty(bg))
                    backgrounds[new Vector2Int(x, y)] = bg;
            }
        }

        var blockCount = 0;
        var liquidCount = 0;
        var bgCount = backgrounds.Count;
        var failCount = 0;

        var worldWidth = (int)WorldGeneration.world.width;
        var worldHeight = (int)WorldGeneration.world.height;

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var worldX = anchorX + x;
            var worldY = anchorY + y;

            if (worldX < 0 || worldX >= worldWidth || worldY < 0 || worldY >= worldHeight)
                continue;

            if (blocks[x, y] > 0) PlaceBlock(blocks[x, y], worldX, worldY, ref blockCount, ref failCount);

            if (liquids[x, y] <= 0 || FluidManager.main == null) continue;
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

        MoreLogs("build_mode_save_applied", blockCount, liquidCount, bgCount, failCount);
        for (var i = 0; i < 5; i++) PlayerUtil.Tp(MapCheck.CurrentMap.SpawnPosition);

        PickItems(MapCheck.CurrentMap);
    }

    public static void ReloadMapFromDisk(Map map)
    {
        if (map == null)
        {
            Error("no_current_map");
            return;
        }

        var directoryPath = map.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            Error("no_directory_path");
            return;
        }

        if (!Directory.Exists(directoryPath))
        {
            Error("map_json_not_found", directoryPath);
            return;
        }

        try
        {
            var reloadedMap = CustomMapDirectoryLoader.LoadFromDirectory(directoryPath);

            if (reloadedMap == null)
            {
                Error("map_deserialize_failed");
                return;
            }

            var index = MapCheck.Maps.FindIndex(f => f.Id == map.Id);
            if (index >= 0)
                MapCheck.Maps[index] = reloadedMap;

            WorldGenerationPatch.CurrentMap = reloadedMap;

            MoreLogs("map_reloaded_from_disk", reloadedMap.Name);
        }
        catch (Exception ex)
        {
            Error("map_reload_failed", ex.Message);
        }
    }

    public static void ReloadMap(Map map)
    {
        if (map == null)
        {
            Error("no_current_map");
            return;
        }

        WorldUtil.CheckForWorld();
        LogUtil.Divider();
        try
        {
            MoreLogs("restarting_scene");
            RestartScene();
        }
        catch (Exception ex)
        {
            Error("reload_failed", ex.Message);
        }

        PickItems(map);
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
        var map = MapCheck.CurrentMap ?? MapCheck.Maps.FirstOrDefault();
        if (map == null)
        {
            Error("no_current_map");
            return;
        }

        LogUtil.Divider();
        LogConsole("info.name", MapLocale.GetName(map));
        LogConsole("info.id", map.Id);
        LogConsole("info.version", map.Version);
        LogConsole("info.authors", MapLocale.GetAuthor(map));
        LogConsole("info.description", MapLocale.GetDescription(map));
        LogConsole("info.features", map.ActiveFeatures);
        LogConsole("info.spawn", map.SpawnPosition);
        LogUtil.Divider();
        LogUtil.NewLine();
    }

    public static void LogMapList()
    {
        var maps = MapCheck.Maps;

        if (maps == null
            || maps.Count == 0)
        {
            LogConsole("list.empty");
            return;
        }

        LogUtil.Divider();
        LogConsole("list.header", maps.Count);

        for (var i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            var isCurrent = map.Id == MapCheck.CurrentMap?.Id;
            var marker = isCurrent ? "->" : "  ";

            var displayName = MapLocale.GetName(map);
            LogConsole("list.item", marker, i + 1, displayName, map.Id, map.Version, map.Authors);
        }

        LogUtil.NewLine();
    }

    private static void PickItems(Map map)
    {
        var items = map.Items;
        foreach (var item in items) PlayerUtil.PickItem(item.Id, item.Slot, item.Force);
    }

    private static void LogConsole(string key, params object[] args)
    {
        var message = BetterLocale.GetCommand($"custommap.{key}", args);
        LogUtil.Info(message, Logger);
    }

    private static void MoreLogs(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Logger);
    }
    
    private static void Info(string key, params object[] args)
    {
        LogUtil.Info(LocaleLog(key, args), Logger);
    }
    
    private static void Warning(string key, params object[] args)
    {
        LogUtil.Warning(LocaleLog(key, args), Logger);
    }

    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
}