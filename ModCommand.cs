using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using CUCoreLib.Helpers;
using CUCoreLib.Registries;
using CustomMap.Data;
using CustomMap.Data.Feature.World;
using CustomMap.Loader;
using CustomMap.Patch;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace CustomMap;

[HarmonyPatch(typeof(ConsoleScript))]
public class ModCommand
{
    private const string LocaleKeyPre = "mod_command";
    private static bool _autofillRegistered;

    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void RegisterCustomCommands(ConsoleScript __instance)
    {
        try
        {
            var argAutofill = new Dictionary<int, List<string>>
            {
                {
                    0,
                    [
                        "help",
                        "reload",
                        "load",
                        "savereload",
                        "info",
                        "spawn",
                        "select",
                        "list",
                        "feature",
                        "waypoint",
                        "save",
                        "layer",
                        "exit"
                    ]
                }
            };

            var paramDescriptions = new[]
            {
                ("string", LocaleCommand("string")),
                ("string", LocaleCommand("parameter")),
                ("string", LocaleCommand("parameter"))
            };

            ConsoleCommandRegistry.Register(
                "cm",
                LocaleCommand("description"),
                ExecuteMapCommand,
                new Dictionary<int, List<string>>(argAutofill),
                paramDescriptions
            );

            RegisterDynamicAutoFills();
        }
        catch (Exception ex)
        {
            Error("register_failed", ex.Message, ex.StackTrace);
        }
    }

    private static void RegisterDynamicAutoFills()
    {
        if (_autofillRegistered)
            return;

        var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddFeatureSubProperties<WorldSettingsData>("world_settings", allNames);
        AddFeatureSubProperties<MineData>("mine", allNames);
        AddFeatureSubProperties<JumpPadData>("jump_pad", allNames);
        AddFeatureSubProperties<TurretData>("turret", allNames);
        AddFeatureSubProperties<SoundCannonData>("sound_cannon", allNames);
        AddFeatureSubProperties<SpikeStabberData>("spike_stabber", allNames);
        AddFeatureSubProperties<GeyserData>("geyser", allNames);
        AddFeatureSubProperties<BearTrapData>("beartrap", allNames);

        _autofillRegistered = true;
    }

    private static void AddFeatureSubProperties<T>(string baseName, HashSet<string> names)
    {
        names.Add(baseName);
        foreach (var subProp in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (subProp.Name == "Type") continue;
            if (!IsSimpleType(subProp.PropertyType))
                continue;
            var subJson = subProp.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? subProp.Name;
            names.Add($"{baseName}.{subJson}".ToLowerInvariant());
        }
    }

    private static void ExecuteMapCommand(string[] args)
    {
        if (args.Length == 1)
            PrintHelp();
        else
            switch (args[1])
            {
                case "help":
                    PrintHelp();
                    break;
                case "reload":
                    CheckArg(args, 1);
                    MapCheck.Reload();
                    if (MapUtils.IsInMapWorld)
                    {
                        MapLoader.ReloadMapFromDisk(MapUtils.CurrentMap);
                        MapLoader.ReloadMap(MapUtils.CurrentMap);
                    }

                    InfoCommand("reload.success");
                    break;
                case "load":
                    CheckArg(args, 1);
                    MapCheck.Reload();
                    InfoCommand("load.success");
                    break;
                case "savereload":
                    HandleSaveReload();
                    break;
                case "info":
                    if (!MapUtils.HasMap) return;
                    CheckArg(args, 1);
                    MapLoader.LogMapInfo();
                    break;
                case "spawn":
                    if (!MapUtils.IsInMapWorld) return;
                    CheckArg(args, 1);
                    Spawn();
                    break;
                case "select":
                    CheckArg(args, 2);
                    Select(string.Join(" ", args, 2, args.Length - 2));
                    break;
                case "list":
                    CheckArg(args, 1);
                    if (args.Length > 2)
                        Select(string.Join(" ", args, 2, args.Length - 2));
                    else
                        MapLoader.LogMapList();
                    break;
                case "feature":
                    if (!MapUtils.HasMap) return;
                    HandleFeature(args);
                    break;
                case "waypoint":
                    if (!MapUtils.HasMap) return;
                    HandleWaypoint(args);
                    break;
                case "save":
                    if (!MapUtils.IsInMapWorld) return;
                    HandleSave(args);
                    break;
                case "layer":
                    if (!MapUtils.IsInMapWorld) return;
                    HandleLayer(args);
                    break;
                case "exit":
                    HandleExit(args);
                    break;
            }
    }

    private static void HandleSaveReload()
    {
        if (!MapUtils.IsInMapWorld) return;

        var map = MapUtils.CurrentMap;

        var directoryPath = map.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorCommand("save.no_directory");
            return;
        }

        CustomMapDirectoryLoader.SaveToDirectory(map, directoryPath);
        MapLocale.SaveToCurrentLang(map, directoryPath);
        InfoCommand("save.success", MapLocale.GetName(map), directoryPath);

        MapLoader.ReloadMapFromDisk(map);
        MapLoader.ReloadMap(map);
        InfoCommand("reload.success");
    }

    private static void HandleWaypoint(string[] args)
    {
        if (!MapUtils.IsInMapWorld) return;

        var map = MapUtils.CurrentMap;
        if (map == null)
        {
            Error("no_waypoints");
            return;
        }

        var waypoints = GetWaypoints(map);

        if (args.Length < 3)
        {
            ListWaypoints(waypoints);
            return;
        }

        var subCommand = args[2].ToLower();

        switch (subCommand)
        {
            case "list":
                ListWaypoints(waypoints);
                break;
            case "help":
                InfoCommand("waypoint.help");
                break;
            case "get":
                if (args.Length < 4)
                {
                    InfoCommand("waypoint.get_no_id");
                    return;
                }

                TeleportToWaypointById(waypoints, args[3]);
                break;
            default:
                InfoCommand("waypoint.unknown_subcommand", subCommand);
                break;
        }
    }

    private static void HandleSave(string[] args)
    {
        var map = MapUtils.CurrentMap;

        string targetPath = null;

        switch (args.Length)
        {
            case 3 when !args[2].Contains(","):
            {
                targetPath = ResolveTargetPath(args[2]);
                if (targetPath == null)
                {
                    ErrorCommand("save.target_not_found", args[2]);
                    return;
                }

                break;
            }
            case 3:
                ErrorCommand("save.missing_end_position");
                return;
            case 5:
            {
                targetPath = ResolveTargetPath(args[4]);
                if (targetPath == null)
                {
                    ErrorCommand("save.target_not_found", args[4]);
                    return;
                }

                break;
            }
        }

        if (map == null)
        {
            if (targetPath == null)
            {
                Error("no_map");
                return;
            }

            map = LoadOrCreateDefaultMap(targetPath);
        }

        var directoryPath = targetPath ?? map.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorCommand("save.no_directory");
            return;
        }

        try
        {
            CustomMapDirectoryLoader.SaveToDirectory(map, directoryPath);
            MapLocale.SaveToCurrentLang(map, directoryPath);

            InfoCommand("save.success", MapLocale.GetName(map), directoryPath);
        }
        catch (Exception ex)
        {
            ErrorCommand("save.failed", MapLocale.GetName(map), ex.Message);
        }
    }

    private static string ResolveTargetPath(string targetName)
    {
        foreach (var dir in
                 from dir in MapUtils.ValidDirectories
                 let folderName = Path.GetFileName(dir)
                 where string.Equals(folderName, targetName, StringComparison.OrdinalIgnoreCase)
                 select dir)
            return dir;

        var directPath = Path.Combine(MapUtils.MapsPath, targetName);
        if (!Directory.Exists(directPath))
            Directory.CreateDirectory(directPath);

        return directPath;
    }

    private static Map LoadOrCreateDefaultMap(string targetPath)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
            throw new ArgumentException(LocaleOther("map_load.empty_target_path"), nameof(targetPath));

        var targetJsonPath = Path.Combine(targetPath, "map.json");

        var loaded = TryLoadMap(targetJsonPath, targetPath);
        return loaded ?? CreateDefaultMap(targetPath);
    }

    private static Map TryLoadMap(string jsonPath, string targetPath)
    {
        string json;
        try
        {
            json = File.ReadAllText(jsonPath);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
        catch (UnauthorizedAccessException ex)
        {
            Warning("map_load.unauthorized", jsonPath, ex.Message);
            return null;
        }
        catch (Exception ex) when (ex is IOException or PathTooLongException)
        {
            Warning("map_load.io_error", jsonPath, ex.Message);
            return null;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            Warning("map_load.file_empty", jsonPath);
            return null;
        }

        try
        {
            var loaded = JsonConvert.DeserializeObject<Map>(json);
            if (loaded == null)
            {
                Warning("map_load.deserialize_null", jsonPath);
                return null;
            }

            loaded.DirectoryPath = targetPath;
            return loaded;
        }
        catch (JsonException ex)
        {
            Warning("map_load.invalid_json", jsonPath, ex.Message);
            return null;
        }
    }

    private static Map CreateDefaultMap(string targetPath)
    {
        var folderName =
            Path.GetFileName(targetPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        if (!string.IsNullOrWhiteSpace(folderName))
            return new Map
            {
                Id = folderName.ToLowerInvariant(),
                Version = "1.0.0",
                Author = ["Unknown"],
                DirectoryPath = targetPath
            };
        Warning("map_load.no_folder_name", targetPath);
        return null;
    }

    private static void HandleLayer(string[] args)
    {
        var map = MapUtils.CurrentMap;
        if (map == null)
        {
            Error("no_current_map");
            return;
        }

        var totalLayers = map.Layers.Count;
        if (totalLayers == 0)
        {
            ErrorCommand("layer.no_layers");
            return;
        }

        if (args.Length < 3)
        {
            var current = map.CurrentLayerIndex + 1;
            InfoCommand("layer.current", current, totalLayers);
            return;
        }

        if (!int.TryParse(args[2], out var target) || target < 1 || target > totalLayers)
        {
            ErrorCommand("layer.invalid", totalLayers);
            return;
        }

        var newIndex = target - 1;
        if (newIndex == map.CurrentLayerIndex)
        {
            InfoCommand("layer.already", target);
            return;
        }

        map.CurrentLayerIndex = newIndex;

        if (MapUtils.IsInMapWorld)
        {
            MapLoader.ReloadMap(map);
            MapLoader.LogMapInfo();
        }

        InfoCommand("layer.switched", target);
    }

    private static void HandleExit(string[] args)
    {
        if (args.Length < 3)
        {
            Error("exit_no_target");
            return;
        }

        var target = args[2].ToLower();

        if (target != "tutorial" && target != "none")
        {
            ErrorCommand("exit.invalid_target", target);
            return;
        }

        WorldGenerationPatch.ExitTargetScene = target switch
        {
            "tutorial" => WorldGeneration.OverrideSceneType.Tutorial,
            "none" => WorldGeneration.OverrideSceneType.None,
            _ => null
        };

        WorldGenerationPatch.CurrentMap = null;

        var targetName = target switch
        {
            "tutorial" => LocaleCommand("exit.target.tutorial"),
            "none" => LocaleCommand("exit.target.none"),
            _ => target
        };
        InfoCommand("exiting", targetName);

        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private static void TeleportToWaypointById(List<WaypointData> waypoints, string waypointId)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Error("no_waypoints");
            return;
        }

        WaypointData target;

        if (int.TryParse(waypointId, out var index))
        {
            if (index < 1 || index > waypoints.Count)
            {
                ErrorCommand("waypoint.invalid_index", index, waypoints.Count);
                return;
            }

            target = waypoints[index - 1];
            if (target == null)
            {
                ErrorCommand("waypoint.not_found", waypointId);
                return;
            }

            TeleportToWaypoint(target, target.Id ?? $"waypoint_{index}");
            return;
        }

        target = waypoints.Find(wp =>
            wp != null && wp.Id?.Equals(waypointId, StringComparison.OrdinalIgnoreCase) == true);

        if (target == null)
        {
            ErrorCommand("waypoint.not_found", waypointId);
            return;
        }

        TeleportToWaypoint(target, target.Id);
    }

    private static void TeleportToWaypoint(WaypointData waypointData, string displayId)
    {
        InfoCommand("waypoint.teleport", displayId, waypointData.Position);
        PlayerUtil.Tp(waypointData.Position);
    }

    private static List<WaypointData> GetWaypoints(Map map)
    {
        return map.CurrentLayer.Waypoints is { Count: > 0 }
            ? map.CurrentLayer.Waypoints
            : [];
    }

    private static void ListWaypoints(List<WaypointData> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Error("no_waypoints");
            return;
        }

        var header = LocaleCommand("waypoint.list_header", waypoints.Count);
        var items = waypoints
            .Select((wp, i) => $"{wp.Id ?? $"waypoint_{i + 1}"}: ({wp.X}, {wp.Y})")
            .ToList();

        LogUtil.PrintNumberedList(header, items, Plugin.Logger);
    }

    private static void HandleFeature(string[] args)
    {
        var map = MapUtils.CurrentMap;
        if (map == null)
        {
            Error("no_map");
            return;
        }

        switch (args.Length)
        {
            case < 3:
                ListFeatures(map);
                return;
            case >= 4:
                SetFeature(map, args[2], args[3]);
                break;
            default:
                InfoCommand("feature.set_missing_params");
                break;
        }
    }

    private static void ListFeatures(Map map)
    {
        var groups = new List<(string groupName, IList<string> items)>();

        var settings = map.CurrentLayer.WorldSettingsData;
        if (settings != null)
        {
            var wsDisplay = LocaleOther("feature.world_settings");
            var wsItems =
                (from prop in typeof(WorldSettingsData).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where prop.Name != "Type"
                    where IsSimpleType(prop.PropertyType)
                    let value = prop.GetValue(settings)
                    let jsonName = prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name
                    let displayName = LocaleOther($"feature.{GetFeatureDisplayName(jsonName)}")
                    select $"{displayName}({jsonName}): {value}").ToList();

            groups.Add(($"{wsDisplay}(world_settings)", wsItems));
        }

        var featureDataTypes = new Dictionary<string, object>
        {
            ["full_bright"] = map.CurrentLayer.WorldSettingsData.FullBright,
            ["forgiving_level"] = map.CurrentLayer.WorldSettingsData.ForgivingLevel,
            ["gravity"] = map.CurrentLayer.WorldSettingsData.Gravity,
            ["jump_limit"] = map.CurrentLayer.WorldSettingsData.JumpLimit,
            ["climb_limit"] = map.CurrentLayer.WorldSettingsData.ClimbLimit,
            ["skip_terrain"] = map.CurrentLayer.SkipTerrain,
            ["skip_background"] = map.CurrentLayer.SkipBackground,
            ["mine"] = map.CurrentLayer.MineData,
            ["jump_pad"] = map.CurrentLayer.JumpPadData,
            ["turret"] = map.CurrentLayer.TurretData,
            ["sound_cannon"] = map.CurrentLayer.SoundCannonData,
            ["spike_stabber"] = map.CurrentLayer.SpikeStabberData,
            ["geyser"] = map.CurrentLayer.GeyserData,
            ["beartrap"] = map.CurrentLayer.BearTrapData,
            ["xp"] = map.XpData
        };

        foreach (var kvp in featureDataTypes)
        {
            var value = kvp.Value;
            var displayName = LocaleOther($"feature.{kvp.Key}");
            if (value == null)
            {
                groups.Add(($"{displayName}({kvp.Key})", ["null"]));
                continue;
            }

            var items = (from subProp in value
                        .GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where subProp.Name != "Type"
                    where IsSimpleType(subProp.PropertyType)
                    let subValue = subProp.GetValue(value)
                    let subJson = subProp.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? subProp.Name
                    let subDisplay = LocaleOther($"feature.{GetFeatureDisplayName($"{kvp.Key}.{subJson}")}")
                    select $"{subDisplay}({subJson}): {subValue}")
                .ToList();
            groups.Add(($"{displayName}({kvp.Key})", items));
        }

        var header = LocaleCommand("feature.list_header");
        LogUtil.PrintGroupedList(header, groups, Plugin.Logger);
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(float)
               || type == typeof(double)
               || type == typeof(bool);
    }

    private static void SetFeature(Map map, string featureName, string valueStr)
    {
        var parts = featureName.Split('.');

        object target;
        PropertyInfo targetProp;

        if (parts.Length == 1)
        {
            var dataProp = FindMapFeatureProperty(parts[0]);
            if (dataProp == null)
            {
                ErrorCommand("feature.not_found", featureName);
                return;
            }

            if (!IsSimpleType(dataProp.PropertyType))
            {
                var current = dataProp.GetValue(map);
                if (current == null)
                {
                    var instance = Activator.CreateInstance(dataProp.PropertyType);
                    dataProp.SetValue(map, instance);
                    InfoCommand("feature.set_success", featureName, "enabled");
                }
                else
                {
                    dataProp.SetValue(map, null);
                    InfoCommand("feature.set_success", featureName, "disabled");
                }

                return;
            }

            target = map;
            targetProp = dataProp;
        }
        else
        {
            var dataProp = FindMapFeatureProperty(parts[0]);
            if (dataProp == null)
            {
                ErrorCommand("feature.not_found", featureName);
                return;
            }

            target = dataProp.GetValue(map);
            if (target == null)
            {
                ErrorCommand("feature.not_found", featureName);
                return;
            }

            targetProp = FindNestedProperty(target.GetType(), parts[1]);
            if (targetProp == null)
            {
                ErrorCommand("feature.not_found", featureName);
                return;
            }
        }

        try
        {
            var convertedValue = Convert.ChangeType(valueStr, targetProp.PropertyType);
            targetProp.SetValue(target, convertedValue);
            var displayName = LocaleOther($"feature.{GetFeatureDisplayName(featureName)}");
            InfoCommand("feature.set_success", $"{displayName} ({featureName})", convertedValue);
        }
        catch (Exception)
        {
            ErrorCommand("feature.invalid_value", featureName, valueStr);
        }
    }

    private static PropertyInfo FindMapFeatureProperty(string name)
    {
        var propName = name switch
        {
            "full_bright" => "FullBright",
            "forgiving_level" => "ForgivingLevel",
            "gravity" => "Gravity",
            "jump_limit" => "JumpLimit",
            "climb_limit" => "ClimbLimit",
            "skip_terrain" => "SkipTerrain",
            "skip_background" => "SkipBackground",
            "mine" => "MineData",
            "jump_pad" => "JumpPadData",
            "turret" => "TurretData",
            "sound_cannon" => "SoundCannonData",
            "spike_stabber" => "SpikeStabberData",
            "geyser" => "GeyserData",
            "beartrap" => "BearTrapData",
            "xp" => "XpData",
            _ => null
        };

        return propName == null
            ? null
            : typeof(Map).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
    }

    private static PropertyInfo FindNestedProperty(Type type, string name)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return prop;
            var jsonAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonAttr?.PropertyName != null &&
                jsonAttr.PropertyName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return prop;
        }

        return null;
    }

    private static string GetFeatureDisplayName(string fieldName)
    {
        return string.IsNullOrEmpty(fieldName)
            ? fieldName
            : fieldName.ToLowerInvariant();
    }

    private static void Select(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            InfoCommand("select.no_key");
            return;
        }

        if (MapUtils.Maps == null || MapUtils.Maps.Count == 0)
        {
            InfoCommand("list.empty");
            return;
        }

        Map map;

        if (int.TryParse(key, out var index))
        {
            if (index < 1 || index > MapUtils.Maps.Count)
            {
                InfoCommand("select.invalid_index", index, MapUtils.Maps.Count);
                return;
            }

            map = MapUtils.Maps[index - 1];
        }
        else
        {
            map = MapUtils.Maps.Find(f =>
                f != null &&
                (f.Id?.Equals(key, StringComparison.OrdinalIgnoreCase) == true ||
                 MapLocale.GetName(f)?.Equals(key, StringComparison.OrdinalIgnoreCase) == true));
        }

        if (map == null)
        {
            InfoCommand("select.not_found", key);
            return;
        }

        WorldGenerationPatch.CurrentMap = map;

        InfoCommand("select.success", MapLocale.GetName(map), map.Id);

        if (MapUtils.IsInMapWorld)
        {
            MapLoader.ReloadMap(map);
            MapLoader.LogMapInfo();
        }
        else
        {
            InfoCommand("select.without_world", MapLocale.GetName(map));
        }
    }

    private static void PrintHelp()
    {
        var helpItems = new List<(string key, string value)>
        {
            ("help", LocaleCommand("help.help")),
            ("reload", LocaleCommand("help.reload")),
            ("load", LocaleCommand("help.load")),
            ("savereload", LocaleCommand("help.savereload")),
            ("info", LocaleCommand("help.info")),
            ("spawn", LocaleCommand("help.spawn")),
            ("select", LocaleCommand("help.select")),
            ("list", LocaleCommand("help.list")),
            ("feature", LocaleCommand("help.feature")),
            ("waypoint", LocaleCommand("help.waypoint")),
            ("save", LocaleCommand("help.save")),
            ("layer", LocaleCommand("help.layer")),
            ("exit", LocaleCommand("help.exit"))
        };

        var header = LocaleCommand("help.header");
        LogUtil.PrintKeyValueList(header, helpItems, Plugin.Logger);
    }

    private static void CheckArg(string[] args, int index)
    {
        CheckUtil.CheckArgumentCount(args, index, Plugin.Logger);
    }

    private static void Spawn()
    {
        var map = MapUtils.CurrentMap;
        InfoCommand("spawn", map.CurrentLayer.SpawnPosition);
        PlayerUtil.Tp(map.CurrentLayer.SpawnPosition);
    }

    private static string LocaleLog(string key, params object[] args)
    {
        return BetterLocale.GetLog($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }

    private static string LocaleOther(string key, params object[] args)
    {
        return BetterLocale.GetOther($"{Plugin.NameSpace}.{key}", args);
    }

    private static string LocaleCommand(string key, params object[] args)
    {
        return BetterLocale.GetCommand($"{Plugin.NameSpace}.cm.{key}", args);
    }

    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Plugin.Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        LogUtil.Warning(LocaleLog(key, args), Plugin.Logger);
    }

    private static void InfoCommand(string key, params object[] args)
    {
        LogUtil.Info(LocaleCommand(key, args), Plugin.Logger);
    }

    private static void ErrorCommand(string key, params object[] args)
    {
        LogUtil.Error(LocaleCommand(key, args), Plugin.Logger);
    }
}