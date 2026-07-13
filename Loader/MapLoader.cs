using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using CustomMap.Patch;
using UnityEngine.SceneManagement;

namespace CustomMap.Loader;

public static class MapLoader
{
    private const string LocaleKeyPre = "map_loader.";

    public static void ApplyBuildModeSave(string saveFilePath)
    {
        if (!Plugin.BuildModeLoaded)
        {
            Warning("build_mode_mod_not_loaded");
            return;
        }

        if (!File.Exists(saveFilePath))
        {
            Error("not_found_buildmode_save");
            return;
        }

        try
        {
            var loadModeType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType("LoadMode"))
                .FirstOrDefault(t => t != null);

            if (loadModeType == null)
            {
                Error("not_found", "Build Mode mod");
                return;
            }

            var assembly = loadModeType.Assembly;
            var buildModeModType = assembly.GetType("BuildModeMod");
            if (buildModeModType == null)
            {
                Error("not_found", "BuildModeMod");
                return;
            }

            loadModeType.GetMethod("LoadFileAndStartPlacement",
                    BindingFlags.Public | BindingFlags.Static)
                ?.Invoke(null, [saveFilePath]);

            var applyLoadMethod = loadModeType.GetMethod("ApplyLoad",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (applyLoadMethod == null)
            {
                Error("not_found", "LoadMode.ApplyLoad");
                return;
            }

            applyLoadMethod.Invoke(null, null);

            buildModeModType.GetField("_loadBuffer", BindingFlags.Public | BindingFlags.Static)
                ?.SetValue(null, null);
            buildModeModType.GetField("_isLoading", BindingFlags.Public | BindingFlags.Static)
                ?.SetValue(null, false);
        }
        catch (Exception ex)
        {
            Error("build_mode_save_load_failed", ex.Message);
        }
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
        LocaleCommand("info.name", MapLocale.GetName(map));
        LocaleCommand("info.id", map.Id);
        LocaleCommand("info.version", map.Version);
        LocaleCommand("info.authors", MapLocale.GetAuthor(map));
        LocaleCommand("info.description", MapLocale.GetDescription(map));
        LocaleCommand("info.features", map.ActiveFeatures);
        LocaleCommand("info.spawn", map.SpawnPosition);
        LogUtil.Divider();
        LogUtil.NewLine();
    }

    public static void LogMapList()
    {
        var maps = MapCheck.Maps;

        if (maps == null
            || maps.Count == 0)
        {
            LocaleCommand("list.empty");
            return;
        }

        LogUtil.Divider();
        LocaleCommand("list.header", maps.Count);

        for (var i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            var isCurrent = map.Id == MapCheck.CurrentMap?.Id;
            var marker = isCurrent ? "->" : "  ";

            var displayName = MapLocale.GetName(map);

            if (map.MissingMods.Count > 0)
            {
                var missingInfo = BetterLocale.GetLog("map_check.requires_mod", string.Join(", ", map.MissingMods));
                displayName = $"<color=grey>{displayName} {missingInfo}";
            }

            LocaleCommand("list.item", marker, i + 1, displayName, map.Id, map.Version, map.Authors);
        }

        LogUtil.NewLine();
    }

    internal static void PickItems(Map map)
    {
        var items = map.Items;
        foreach (var item in items) PlayerUtil.PickItem(item.Id, item.Slot, item.Force);
    }

    private static void MoreLogs(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void LocaleCommand(string key, params object[] args)
    {
        LogUtil.Info(BetterLocale.GetCommand($"custommap.{key}", args), Plugin.Logger);
    }

    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Plugin.Logger);
    }

    private static void Info(string key, params object[] args)
    {
        LogUtil.Info(LocaleLog(key, args), Plugin.Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        LogUtil.Warning(LocaleLog(key, args), Plugin.Logger);
    }

    private static string LocaleLog(string key, params object[] args)
    {
        return BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
    }
}