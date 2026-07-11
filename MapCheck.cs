using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
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

    public static void Initialize()
    {
        if (Plugin.Logger != null) return;
        Plugin.Logger = Plugin.Logger;
        LoadMapDirectories();
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