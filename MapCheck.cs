using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bark.BetterCCL;
using CustomMap.Loader;
using CustomMap.Patch;

namespace CustomMap;

public static class MapCheck
{
    public static readonly string MapsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps");
    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> CheckFailDirectories = [];
    public static readonly List<Map> Maps = [];
    public static Map CurrentMap => WorldGenerationPatch.CurrentMap;
    public static bool HasRunningMap => CurrentMap != null;
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        LoadMapDirectories();
    }

    /// <summary>
    /// 重新加载 Maps 文件夹中的所有地图
    /// </summary>
    public static void Reload()
    {
        _initialized = false;
        Initialize();
    }

    private static void LoadMapDirectories()
    {
        if (!Directory.Exists(MapsPath))
            Directory.CreateDirectory(MapsPath);

        var directories = Directory.GetDirectories(MapsPath);

        Plugin.Logger.LogInfo($"Read {directories.Length} Map folders");

        ValidDirectories.Clear();
        CheckFailDirectories.Clear();
        Maps.Clear();

        foreach (var mapsDirectory in directories)
        {
            var mapJsonPath = Path.Combine(mapsDirectory, "map.json");
            if (!File.Exists(mapJsonPath)) continue;

            ValidDirectories.Add(mapsDirectory);
        }

        Maps.Add(Plugin.TemplateMap);

        if (ValidDirectories.Count == 0) return;
        Plugin.Logger.LogInfo($"Valid directories: {ValidDirectories.Count}, loading...");

        var directoriesToValidate = ValidDirectories.ToList();
        foreach (var directory in directoriesToValidate)
        {
            var map = CustomMapDirectoryLoader.LoadFromDirectory(directory);
            if (map != null)
            {
                // 检测缺失的模组
                DetectMissingMods(map);
                Maps.Add(map);
            }
            else
            {
                UninitializedWarning($"{Path.GetFileName(directory)} Loading failed!");
                CheckFailDirectories.Add(directory);
            }
        }

        if (CheckFailDirectories.Count == 0) return;
        Plugin.Logger.LogInfo($"Directory validation failed: {CheckFailDirectories.Count}:");
        foreach (var failDirectory in CheckFailDirectories)
        {
            Plugin.Logger.LogInfo($"- {Path.GetFileName(failDirectory)}");
            ValidDirectories.Remove(failDirectory);
        }
    }

    public static void DetectMissingMods(Map map)
    {
        if (map == null) return;

        var missingMods = map.MissingMods;
        var hasBuildModeSave = !string.IsNullOrEmpty(map.BuildModeSave);
        var hasCustomStructures = !string.IsNullOrEmpty(map.CustomStructures);

        if (hasBuildModeSave && !Plugin.BuildModeLoaded)
        {
            missingMods.Add("Build Mode");
            Plugin.Logger.LogWarning(BetterLocale.GetLog("map_check.missing_build_mode_mod", map.Name));
        }

        if (hasCustomStructures && !Plugin.CustomStructuresLoaded)
        {
            missingMods.Add("Custom Structures");
            Plugin.Logger.LogWarning(BetterLocale.GetLog("map_check.missing_custom_structures_mod", map.Name));
        }
    }

    public static string GetMapPath(Map map = null)
    {
        var target = map ?? CurrentMap;
        return string.IsNullOrEmpty(target?.DirectoryPath)
            ? null
            : target.DirectoryPath;
    }

    private static void UninitializedWarning(string key) =>
        Plugin.Logger.LogWarning($"{key}");
}