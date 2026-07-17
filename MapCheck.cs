using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using CustomMap.Loader;
using CustomMap.Patch;

namespace CustomMap;

public static class MapCheck
{
    public static readonly string MapsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps");
    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> CheckFailDirectories = [];
    public static readonly List<Map> Maps = [];
    private static bool _initialized;
    public static Map CurrentMap => WorldGenerationPatch.CurrentMap;
    public static bool HasRunningMap => CurrentMap != null;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        LoadMapDirectories();
    }

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

        LogUtil.Info($"Read {directories.Length} Map folders", Plugin.Logger);

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
        LogUtil.Warning($"Valid directories: {ValidDirectories.Count}, loading...", Plugin.Logger);

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
                LogUtil.Warning($"{Path.GetFileName(directory)} Loading failed!", Plugin.Logger);
                CheckFailDirectories.Add(directory);
            }
        }

        if (CheckFailDirectories.Count == 0) return;
        LogUtil.Warning($"Directory validation failed: {CheckFailDirectories.Count}:", Plugin.Logger);
        foreach (var failDirectory in CheckFailDirectories)
        {
            LogUtil.Warning($"- {Path.GetFileName(failDirectory)}", Plugin.Logger);
            ValidDirectories.Remove(failDirectory);
        }
    }

    public static void DetectMissingMods(Map map)
    {
        if (map == null) return;

        var hasBuildModeSave = !string.IsNullOrEmpty(map.CurrentLayer.BuildModeSave);

        if (hasBuildModeSave && !Plugin.BuildModeLoaded)
        {
            map.MissingMods.Add("Build Mode Mod");
        }

        foreach (var mod in map.MissingMods)
        {
            LogUtil.Warning($"{MapLocale.GetName(map)} need '{mod}' mod", Plugin.Logger);
        }
    }

    public static string GetMapPath(Map map = null)
    {
        var target = map ?? CurrentMap;
        return string.IsNullOrEmpty(target?.DirectoryPath)
            ? null
            : target.DirectoryPath;
    }
}