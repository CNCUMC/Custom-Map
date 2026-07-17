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
    private const string LocaleKeyPre = "map_loader";
    
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

            var index = MapUtils.Maps.FindIndex(f => f.Id == map.Id);
            if (index >= 0)
                MapUtils.Maps[index] = reloadedMap;

            WorldGenerationPatch.CurrentMap = reloadedMap;

            MoreLogs("map_reloaded_from_disk", MapLocale.GetName(reloadedMap));
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

        CheckUtil.CheckWorld(Plugin.Logger);
        LogUtil.Divider();
        try
        {
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
        var map = MapUtils.CurrentMap ?? MapUtils.Maps.FirstOrDefault();
        if (map == null)
        {
            Error("no_current_map");
            return;
        }

        LogUtil.Divider();
        InfoCommand("info.name", MapLocale.GetName(map));
        InfoCommand("info.id", map.Id);
        InfoCommand("info.version", map.Version);
        InfoCommand("info.authors", MapLocale.GetAuthor(map));
        InfoCommand("info.description", MapLocale.GetDescription(map));
        InfoCommand("info.features", map.CurrentLayer.ActiveFeatures);
        InfoCommand("info.spawn", map.CurrentLayer.SpawnPosition);
        LogUtil.Divider();
        LogUtil.NewLine();
    }

    public static void LogMapList()
    {
        var maps = MapUtils.Maps;

        if (maps == null
            || maps.Count == 0)
        {
            InfoCommand("list.empty");
            return;
        }

        LogUtil.Divider();
        InfoCommand("list.header", maps.Count);

        for (var i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            var isCurrent = map.Id == MapUtils.CurrentMap?.Id;
            var marker = isCurrent
                ? "->" 
                : "  ";

            var displayName = MapLocale.GetName(map);

            if (map.MissingMods.Count > 0)
            {
                var missingInfo = BetterLocale.GetLog($"{Plugin.NameSpace}.{LocaleKeyPre}.requires_mod", string.Join(", ", map.MissingMods));
                displayName = $"<color=grey>{displayName} {missingInfo}";
            }

            InfoCommand("list.item", marker, i + 1, displayName, map.Id, map.Version, map.Authors);
        }

        LogUtil.NewLine();
    }

    internal static void PickItems(Map map)
    {
        var items = map.CurrentLayer.Items;
        if (items == null) return;
        foreach (var item in items) PlayerUtil.PickItem(item.Id, item.Slot, item.Force);
    }

    private static void MoreLogs(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void InfoCommand(string key, params object[] args)
    {
        LogUtil.Info(BetterLocale.GetCommand($"{Plugin.NameSpace}.cm.{key}", args), Plugin.Logger);
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
        return BetterLocale.GetLog($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }
}