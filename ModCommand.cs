using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx.Logging;
using CUCoreLib.Helpers;
using CUCoreLib.Registries;
using CustomMap.Data;
using CustomMap.Data.Feature.World;
using CustomMap.Loader;
using CustomMap.Patch;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomMap;

[HarmonyPatch(typeof(ConsoleScript))]
public class ModCommand
{
    private const string LocaleKeyPre = "mod_command.";

    private const string DefaultVersion = "1.0.0";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    private static bool _autofillRegistered;
    private static readonly List<string> DefaultAuthor = ["Unknown"];

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
                        "info",
                        "spawn",
                        "select",
                        "list",
                        "feature",
                        "waypoint",
                        "save",
                        "config",
                        "exit"
                    ]
                }
            };

            var paramDescriptions = new[]
            {
                ("string", Map("string")),
                ("string", Map("parameter")),
                ("string", Map("parameter"))
            };

            ConsoleCommandRegistry.Register(
                "custommap",
                Map("description"),
                ExecuteMapCommand,
                argAutofill,
                paramDescriptions
            );

            ConsoleCommandRegistry.Register(
                "cm",
                Map("description"),
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
                    if (!EnsureWorldLoaded()) return;
                    CheckArg(args, 1);
                    MapLoader.ReloadMapFromDisk(MapCheck.CurrentMap);
                    MapLoader.ReloadMap(MapCheck.CurrentMap);
                    break;
                case "info":
                    CheckArg(args, 1);
                    MapLoader.LogMapInfo();
                    break;
                case "spawn":
                    if (!EnsureWorldLoaded()) return;
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
                    HandleFeature(args);
                    break;
                case "waypoint":
                    HandleWaypoint(args);
                    break;
                case "save":
                    HandleSave(args);
                    break;
                case "exit":
                    HandleExit(args);
                    break;
            }
    }

    private static void HandleWaypoint(string[] args)
    {
        if (!EnsureWorldLoaded()) return;

        var map = MapCheck.CurrentMap;
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
                InfoMap("waypoint.help");
                break;
            case "get":
                if (args.Length < 4)
                {
                    InfoMap("waypoint.get_no_id");
                    return;
                }

                TeleportToWaypointById(waypoints, args[3]);
                break;
            default:
                InfoMap("waypoint.unknown_subcommand", subCommand);
                break;
        }
    }

    private static void HandleSave(string[] args)
    {
        if (args.Length is 3 or 4 && args[2].Equals("as", StringComparison.OrdinalIgnoreCase))
        {
            var targetName = args.Length == 4 ? args[3] : null;
            HandleSaveAs(targetName);
            return;
        }

        var map = MapCheck.CurrentMap;

        string targetPath = null;

        switch (args.Length)
        {
            case 3 when !args[2].Contains(","):
            {
                targetPath = ResolveTargetPath(args[2]);
                if (targetPath == null)
                {
                    ErrorMap("save.target_not_found", args[2]);
                    return;
                }

                break;
            }
            case 3:
                ErrorMap("save.missing_end_position");
                return;
            case 5:
            {
                targetPath = ResolveTargetPath(args[4]);
                if (targetPath == null)
                {
                    ErrorMap("save.target_not_found", args[4]);
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
            ErrorMap("save.no_directory");
            return;
        }

        try
        {
            if (args.Length is 4 or 5)
            {
                SaveAreaAsMapData(map, directoryPath, args[2], args[3]);
                return;
            }

            CustomMapDirectoryLoader.SaveToDirectory(map, directoryPath);
            MapLocale.SaveToCurrentLang(map, directoryPath);

            InfoMap("save.success", MapLocale.GetName(map), directoryPath);
        }
        catch (Exception ex)
        {
            ErrorMap("save.failed", MapLocale.GetName(map), ex.Message);
        }
    }

    private static void HandleSaveAs(string targetName)
    {
        if (!EnsureWorldLoaded()) return;

        CUCoreUtils.StartCoroutine(SaveAsCoroutine(targetName));
    }

    private static IEnumerator SaveAsCoroutine(string targetName)
    {
        yield return null;

        var map = MapCheck.CurrentMap;
        string targetPath = null;

        if (!string.IsNullOrEmpty(targetName))
        {
            targetPath = ResolveTargetPath(targetName);
            if (targetPath == null)
            {
                ErrorMap("save.target_not_found", targetName);
                yield break;
            }

            map ??= LoadOrCreateDefaultMap(targetPath);
        }

        if (map == null)
        {
            Error("no_map");
            yield break;
        }

        var directoryPath = targetPath ?? map.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorMap("save.no_directory");
            yield break;
        }

        TipMap("save.as.start_position");

        var waiter1 = new LeftClickYieldInstruction();
        yield return waiter1;
        var startPos = waiter1.Result;

        yield return null;

        TipMap("save.as.end_position");

        var waiter2 = new LeftClickYieldInstruction();
        yield return waiter2;
        var endPos = waiter2.Result;

        var startStr = $"{startPos.x},{startPos.y}";
        var endStr = $"{endPos.x},{endPos.y}";

        SaveAreaAsMapData(map, directoryPath, startStr, endStr);
    }

    private static string ResolveTargetPath(string targetName)
    {
        foreach (var dir in
                 from dir in MapCheck.ValidDirectories
                 let folderName = Path.GetFileName(dir)
                 where string.Equals(folderName, targetName, StringComparison.OrdinalIgnoreCase)
                 select dir)
            return dir;

        var directPath = Path.Combine(MapCheck.MapsPath, targetName);
        if (!Directory.Exists(directPath))
            Directory.CreateDirectory(directPath);

        return directPath;
    }

    private static Map LoadOrCreateDefaultMap(string targetPath)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
            throw new ArgumentException(Other("map_load.empty_target_path"), nameof(targetPath));

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
            Logger.LogWarning(Other("map_load.unauthorized", jsonPath, ex.Message));
            return null;
        }
        catch (Exception ex) when (ex is IOException or PathTooLongException)
        {
            Logger.LogWarning(Other("map_load.io_error", jsonPath, ex.Message));
            return null;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            Logger.LogWarning(Other("map_load.file_empty", jsonPath));
            return null;
        }

        try
        {
            var loaded = JsonConvert.DeserializeObject<Map>(json);
            if (loaded == null)
            {
                Logger.LogWarning(Other("map_load.deserialize_null", jsonPath));
                return null;
            }

            loaded.DirectoryPath = targetPath;
            return loaded;
        }
        catch (JsonException ex)
        {
            Logger.LogWarning(Other("map_load.invalid_json", jsonPath, ex.Message));
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
                Name = folderName,
                Id = folderName.ToLowerInvariant(),
                Version = DefaultVersion,
                Author = DefaultAuthor,
                Description = Map("save.as.default_description"),
                DirectoryPath = targetPath
            };
        Logger.LogWarning(Other("map_load.no_folder_name", targetPath));
        return null;
    }

    private static void SaveAreaAsMapData(Map map, string directoryPath, string startStr, string endStr)
    {
        if (!EnsureWorldLoaded()) return;

        var startParts = startStr.Split(',');
        var endParts = endStr.Split(',');

        if (startParts.Length != 2
            || endParts.Length != 2
            || !float.TryParse(startParts[0].Trim(), out var wx1)
            || !float.TryParse(startParts[1].Trim(), out var wy1)
            || !float.TryParse(endParts[0].Trim(), out var wx2)
            || !float.TryParse(endParts[1].Trim(), out var wy2))
        {
            ErrorMap("save.invalid_position");
            return;
        }

        var world = WorldGeneration.world;
        var blockA = world.WorldToBlockPos(new Vector2(wx1, wy1));
        var blockB = world.WorldToBlockPos(new Vector2(wx2, wy2));

        var minX = Mathf.Min(blockA.x, blockB.x);
        var maxX = Mathf.Max(blockA.x, blockB.x);
        var minY = Mathf.Min(blockA.y, blockB.y);
        var maxY = Mathf.Max(blockA.y, blockB.y);

        var cMinX = Mathf.Clamp(minX, 0, (int)world.width - 1);
        var cMaxX = Mathf.Clamp(maxX, 0, (int)world.width - 1);
        var cMinY = Mathf.Clamp(minY, 0, (int)world.height - 1);
        var cMaxY = Mathf.Clamp(maxY, 0, (int)world.height - 1);

        var regionW = cMaxX - cMinX + 1;
        var regionH = cMaxY - cMinY + 1;

        if (regionW <= 0 || regionH <= 0)
        {
            ErrorMap("save.area_empty");
            return;
        }

        var blockIds = new ushort[regionW][];
        for (var index = 0; index < regionW; index++) blockIds[index] = new ushort[regionH];

        var uniqueBlockIds = new List<ushort>();
        var blockToChar = new Dictionary<ushort, string>();

        uniqueBlockIds.Add(0);
        blockToChar[0] = "0";

        for (var x = 0; x < regionW; x++)
        for (var y = 0; y < regionH; y++)
        {
            var bx = cMinX + x;
            var by = cMaxY - y;
            var id = world.GetBlock(new Vector2Int(bx, by));

            blockIds[x][y] = id;

            if (id <= 0 || blockToChar.ContainsKey(id)) continue;
            blockToChar[id] = EncodeBlockIndex(uniqueBlockIds.Count);
            uniqueBlockIds.Add(id);
        }

        var mapRows = new string[regionH];
        for (var y = 0; y < regionH; y++)
        {
            var chars = new char[regionW];
            for (var x = 0; x < regionW; x++)
            {
                var id = blockIds[x][y];
                chars[x] = blockToChar.TryGetValue(id, out var ch)
                    ? ch[0]
                    : '0';
            }

            mapRows[y] = new string(chars);
        }

        var keyDict = new Dictionary<string, object>();
        for (var i = 0; i < uniqueBlockIds.Count; i++) keyDict[EncodeBlockIndex(i)] = (long)uniqueBlockIds[i];

        var newLevel = map.CurrentLevel != null
            ? new LevelData
            {
                X = map.CurrentLevel.X,
                Y = map.CurrentLevel.Y,
                Spawn = map.CurrentLevel.Spawn,
                MapData = new MapData { Map = mapRows, Key = keyDict },
                CustomStructures = null,
                BuildModeSave = null,
                SceneType = map.CurrentLevel.SceneType,
                Items = map.CurrentLevel.Items
            }
            : new LevelData
            {
                X = cMinX,
                Y = cMinY,
                MapData = new MapData { Map = mapRows, Key = keyDict }
            };

        map.Levels = [newLevel];

        CustomMapDirectoryLoader.SaveToDirectory(map, directoryPath);
        MapLocale.SaveToCurrentLang(map, directoryPath);

        InfoMap("save.area_success",
            cMinX, cMinY, cMaxX, cMaxY,
            regionW, regionH, uniqueBlockIds.Count, directoryPath);
    }

    private static void HandleExit(string[] args)
    {
        if (!EnsureWorldLoaded()) return;

        if (args.Length < 3)
        {
            Error("exit_no_target");
            return;
        }

        var target = args[2].ToLower();

        if (target != "tutorial" && target != "none")
        {
            ErrorMap("exit.invalid_target", target);
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
            "tutorial" => Map("exit.target.tutorial"),
            "none" => Map("exit.target.none"),
            _ => target
        };
        InfoMap("exiting", targetName);

        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private static string EncodeBlockIndex(int index)
    {
        return index switch
        {
            < 10 => ((char)('0' + index)).ToString(),
            < 36 => ((char)('a' + index - 10)).ToString(),
            _ => ((char)('A' + index - 36)).ToString()
        };
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
                ErrorMap("waypoint.invalid_index", index, waypoints.Count);
                return;
            }

            target = waypoints[index - 1];
            if (target == null)
            {
                ErrorMap("waypoint.not_found", waypointId);
                return;
            }

            TeleportToWaypoint(target, target.Id ?? $"waypoint_{index}");
            return;
        }

        target = waypoints.Find(wp =>
            wp != null && wp.Id?.Equals(waypointId, StringComparison.OrdinalIgnoreCase) == true);

        if (target == null)
        {
            ErrorMap("waypoint.not_found", waypointId);
            return;
        }

        TeleportToWaypoint(target, target.Id);
    }

    private static void TeleportToWaypoint(WaypointData waypointData, string displayId)
    {
        InfoMap("waypoint.teleport", displayId, waypointData.Position);
        PlayerUtil.Tp(waypointData.Position);
    }

    private static List<WaypointData> GetWaypoints(Map map)
    {
        return map.Waypoints is { Count: > 0 }
            ? map.Waypoints
            : [];
    }

    private static void ListWaypoints(List<WaypointData> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Error("no_waypoints");
            return;
        }

        var header = Map("waypoint.list_header", waypoints.Count);
        var items = waypoints
            .Select((wp, i) => $"{wp.Id ?? $"waypoint_{i + 1}"}: ({wp.X}, {wp.Y})")
            .ToList();

        LogUtil.PrintNumberedList(header, items, Logger);
    }

    private static void HandleFeature(string[] args)
    {
        var map = MapCheck.CurrentMap;
        if (map == null)
        {
            Error("no_map");
            return;
        }

        switch (args.Length)
        {
            case < 3:
                InfoMap("help");
                ListFeatures(map);
                return;
            case >= 4:
                SetFeature(map, args[2], args[3]);
                break;
            default:
                InfoMap("feature.set_missing_params");
                break;
        }
    }

    private static void ListFeatures(Map map)
    {
        var groups = new List<(string groupName, IList<string> items)>();

        var settings = map.WorldSettingsData;
        if (settings != null)
        {
            var wsDisplay = Locale("feature.world_settings_data");
            var wsItems =
                (from prop in typeof(WorldSettingsData).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    where prop.Name != "Type"
                    where IsSimpleType(prop.PropertyType)
                    let value = prop.GetValue(settings)
                    let jsonName = prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name
                    let displayName = Locale($"feature.{GetFeatureDisplayName(jsonName)}")
                    select $"{displayName}({jsonName}): {value}").ToList();

            groups.Add(($"{wsDisplay}(world_settings)", wsItems));
        }

        var featureDataTypes = new Dictionary<string, object>
        {
            ["mine"] = map.MineData,
            ["jump_pad"] = map.JumpPadData,
            ["turret"] = map.TurretData,
            ["sound_cannon"] = map.SoundCannonData,
            ["spike_stabber"] = map.SpikeStabberData,
            ["geyser"] = map.GeyserData,
            ["beartrap"] = map.BearTrapData,
            ["xp"] = map.XpData
        };

        foreach (var kvp in featureDataTypes)
        {
            var value = kvp.Value;
            var displayName = Locale($"feature.{kvp.Key}_data");
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
                    let subDisplay = Locale($"feature.{GetFeatureDisplayName($"{kvp.Key}.{subJson}")}")
                    select $"{subDisplay}({subJson}): {subValue}")
                .ToList();
            groups.Add(($"{displayName}({kvp.Key})", items));
        }

        var header = Map("feature.list_header");
        LogUtil.PrintGroupedList(header, groups, Logger);
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
                ErrorMap("feature.not_found", featureName);
                return;
            }

            if (!IsSimpleType(dataProp.PropertyType))
            {
                var current = dataProp.GetValue(map);
                if (current == null)
                {
                    var instance = Activator.CreateInstance(dataProp.PropertyType);
                    dataProp.SetValue(map, instance);
                    InfoMap("feature.set_success", featureName, "enabled");
                }
                else
                {
                    dataProp.SetValue(map, null);
                    InfoMap("feature.set_success", featureName, "disabled");
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
                ErrorMap("feature.not_found", featureName);
                return;
            }

            target = dataProp.GetValue(map);
            if (target == null)
            {
                ErrorMap("feature.not_found", featureName);
                return;
            }

            targetProp = FindNestedProperty(target.GetType(), parts[1]);
            if (targetProp == null)
            {
                ErrorMap("feature.not_found", featureName);
                return;
            }
        }

        try
        {
            var convertedValue = Convert.ChangeType(valueStr, targetProp.PropertyType);
            targetProp.SetValue(target, convertedValue);
            var displayName = Locale($"feature.{GetFeatureDisplayName(featureName)}");
            InfoMap("feature.set_success", $"{displayName} ({featureName})", convertedValue);
        }
        catch (Exception)
        {
            ErrorMap("feature.invalid_value", featureName, valueStr);
        }
    }

    private static PropertyInfo FindMapFeatureProperty(string name)
    {
        var propName = name switch
        {
            "mine" => "MineData",
            "jump_pad" => "JumpPadData",
            "turret" => "TurretData",
            "sound_cannon" => "SoundCannonData",
            "spike_stabber" => "SpikeStabberData",
            "geyser" => "GeyserData",
            "beartrap" => "BearTrapData",
            "world_settings" => "WorldSettings",
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
        return string.IsNullOrEmpty(fieldName) ? fieldName : fieldName.ToLowerInvariant();
    }

    private static bool EnsureWorldLoaded()
    {
        if (HasWorldLoaded()) return true;
        Error("world_not_loaded");
        return false;
    }

    private static bool HasWorldLoaded()
    {
        try
        {
            return WorldGeneration.world && !WorldGeneration.world.generatingWorld;
        }
        catch
        {
            return false;
        }
    }

    private static void Select(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            InfoMap("select.no_key");
            return;
        }

        if (MapCheck.Maps == null || MapCheck.Maps.Count == 0)
        {
            InfoMap("list.empty");
            return;
        }

        Map map;

        if (int.TryParse(key, out var index))
        {
            if (index < 1 || index > MapCheck.Maps.Count)
            {
                InfoMap("select.invalid_index", index, MapCheck.Maps.Count);
                return;
            }

            map = MapCheck.Maps[index - 1];
        }
        else
        {
            map = MapCheck.Maps.Find(f =>
                f != null &&
                (f.Id?.Equals(key, StringComparison.OrdinalIgnoreCase) == true ||
                 f.Name?.Equals(key, StringComparison.OrdinalIgnoreCase) == true));
        }

        if (map == null)
        {
            InfoMap("select.not_found", key);
            return;
        }

        WorldGenerationPatch.CurrentMap = map;

        InfoMap("select.success", MapLocale.GetName(map), map.Id);

        if (HasWorldLoaded())
        {
            MapLoader.ReloadMap(map);
            MapLoader.LogMapInfo();
        }
        else
        {
            InfoMap("select.without_world", MapLocale.GetName(map));
        }
    }

    private static void PrintHelp()
    {
        var helpItems = new List<(string key, string value)>
        {
            ("help", Map("help.help")),
            ("reload", Map("help.reload")),
            ("info", Map("help.info")),
            ("spawn", Map("help.spawn")),
            ("select", Map("help.select")),
            ("list", Map("help.list")),
            ("feature", Map("help.feature")),
            ("waypoint", Map("help.waypoint")),
            ("save", Map("help.save")),
            ("exit", Map("help.exit"))
        };

        var header = Map("help.header");
        LogUtil.PrintKeyValueList(header, helpItems, Logger);
    }

    private static void CheckArg(string[] args, int index)
    {
        ToolsUtil.CheckArgumentCount(args, index);
    }

    private static void Spawn()
    {
        var map = MapCheck.CurrentMap;
        InfoMap("spawn", map.SpawnPosition);
        PlayerUtil.Tp(map.SpawnPosition);
    }
    
    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Logger);
    }

    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
    
    private static string LocaleCommand(string key, params object[] args) =>
        BetterLocale.GetCommand($"custommap.{key}", args);

    private static string Map(string key, params object[] args) =>
        LocaleCommand(key, args);

    private static void InfoMap(string key, params object[] args) =>
        LogUtil.Info(LocaleCommand(key, args), Logger);

    private static void ErrorMap(string key, params object[] args) =>
        LogUtil.Error(LocaleCommand(key, args), Logger);

    private static void TipMap(string key, params object[] args) =>
        LogUtil.Info(LocaleCommand(key, args), Logger);

    private static string Locale(string key) =>
        BetterLocale.GetOther(key);

    private static string Other(string key, params object[] args) =>
        BetterLocale.GetOther(key, args);

    private sealed class LeftClickYieldInstruction : CustomYieldInstruction
    {
        public Vector2 Result { get; private set; }

        public override bool keepWaiting
        {
            get
            {
                if (field)
                    return false;

                if (!Input.GetMouseButtonDown(0))
                    return true;

                var camera = Camera.main;
                if (camera != null)
                {
                    var worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
                    Result = new Vector2(worldPos.x, worldPos.y);
                }

                field = true;
                return false;
            }
        }
    }
}