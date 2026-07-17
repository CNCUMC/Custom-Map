using System.IO;
using System.Linq;
using Bark.Tool;
using CustomMap.Loader;

namespace CustomMap;

public static class MapCheck
{
    private static bool _initialized;

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
        if (!Directory.Exists(MapUtils.MapsPath))
            Directory.CreateDirectory(MapUtils.MapsPath);

        var directories = Directory.GetDirectories(MapUtils.MapsPath);

        LogUtil.Info($"Read {directories.Length} Map folders", Plugin.Logger);

        MapUtils.ValidDirectories.Clear();
        MapUtils.CheckFailDirectories.Clear();
        MapUtils.Maps.Clear();

        foreach (var mapsDirectory in directories)
        {
            var mapJsonPath = Path.Combine(mapsDirectory, "map.json");
            if (!File.Exists(mapJsonPath)) continue;

            MapUtils.ValidDirectories.Add(mapsDirectory);
        }

        MapUtils.Maps.Add(Plugin.TemplateMap);

        if (MapUtils.ValidDirectories.Count == 0) return;
        LogUtil.Warning($"Valid directories: {MapUtils.ValidDirectories.Count}, loading...", Plugin.Logger);

        var directoriesToValidate = MapUtils.ValidDirectories.ToList();
        foreach (var directory in directoriesToValidate)
        {
            var map = CustomMapDirectoryLoader.LoadFromDirectory(directory);
            if (map != null)
            {
                // 检测缺失的模组
                DetectMissingMods(map);
                MapUtils.Maps.Add(map);
            }
            else
            {
                LogUtil.Warning($"{Path.GetFileName(directory)} Loading failed!", Plugin.Logger);
                MapUtils.CheckFailDirectories.Add(directory);
            }
        }

        if (MapUtils.CheckFailDirectories.Count == 0) return;
        LogUtil.Warning($"Directory validation failed: {MapUtils.CheckFailDirectories.Count}:", Plugin.Logger);
        foreach (var failDirectory in MapUtils.CheckFailDirectories)
        {
            LogUtil.Warning($"- {Path.GetFileName(failDirectory)}", Plugin.Logger);
            MapUtils.ValidDirectories.Remove(failDirectory);
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
}