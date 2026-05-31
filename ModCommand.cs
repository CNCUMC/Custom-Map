using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using CustomFungamePack.Loader;
using CustomFungamePack.Patch;
using HarmonyLib;
using MossLib.Base;
using MossLib.Tool;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFungamePack;

[HarmonyPatch(typeof(ConsoleScript))]
public class ModCommand : ModCommandBase
{
    private new static readonly ManualLogSource Logger = Plugin.Logger;
    private const string LocaleKeyPre = "mod_command.";

    private static readonly PropertyInfo[] FeatureProperties =
        typeof(Feature).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    private static bool _autofillRegistered;
    private static List<string> _cachedFeatureNames = [];
    private static List<string> _cachedFungameIds = [];

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
                        "exit"
                    ]
                }
            };

            var paramDescriptions = new[]
            {
                ("string", Fungame("string")),
                ("string", Fungame("parameter")),
                ("string", Fungame("parameter"))
            };

            ConsoleScript.Commands.Add(new Command(
                "fungame",
                Fungame("description"),
                ExecuteFungameCommand,
                argAutofill,
                paramDescriptions)
            );

            ConsoleScript.Commands.Add(new Command(
                "fg",
                Fungame("description"),
                ExecuteFungameCommand,
                new Dictionary<int, List<string>>(argAutofill),
                paramDescriptions)
            );

            RegisterDynamicAutoFills();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to register custom commands: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void RegisterDynamicAutoFills()
    {
        if (_autofillRegistered)
            return;

        var targetCommands = ConsoleScript.Commands
            .Where(c => c != null && c.action == ExecuteFungameCommand)
            .ToList();

        if (targetCommands.Count == 0)
            return;

        var fungameIds = FungameCheck.Fungames?
            .Where(f => f != null)
            .Select(f => f.Id)
            .Where(name => !string.IsNullOrEmpty(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var featureNames = FeatureProperties
            .Select(p => p.Name.ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _cachedFeatureNames = featureNames;
        _cachedFungameIds = fungameIds ?? [];

        if (fungameIds is not { Count: > 0 } && featureNames.Count == 0)
        {
        }

        _autofillRegistered = true;
    }

    [HarmonyPatch("HandleDescriptionText")]
    [HarmonyPrefix]
    private static void PreHandleDescriptionText(string[] args)
    {
        UpdateAutofillContext(args);
    }

    [HarmonyPatch("TryFinishCommandPart")]
    [HarmonyPrefix]
    private static void PreTryFinishCommandPart(string[] args)
    {
        UpdateAutofillContext(args);
    }

    private static void UpdateAutofillContext(string[] args)
    {
        if (args == null || args.Length < 2)
            return;

        string cmdName = args[0];
        if (cmdName != "fungame" && cmdName != "fg")
            return;

        var cmd = ConsoleScript.SearchExact(cmdName);
        if (cmd?.argAutofill == null)
            return;

        int key = args.Length - 2;
        if (key != 1)
            return;

        var contextList = new List<string>();
        string subcommand = args[1].ToLower();

        for (int i = 0; i < _cachedFeatureNames.Count; i++)
        {
            string name = _cachedFeatureNames[i];
            if (name.EndsWith("data"))
            {
                _cachedFeatureNames[i] = name.Substring(0, name.Length - 4);
            }
        }

        switch (subcommand)
        {
            case "feature":
                contextList.AddRange(_cachedFeatureNames);
                break;
            case "select":
            case "list":
                if (_cachedFungameIds.Count > 0)
                    contextList.AddRange(_cachedFungameIds);
                break;
            case "waypoint":
                contextList.AddRange(["list", "get", "help"]);
                break;
            case "save":
                contextList.Add("as");
                if (_cachedFungameIds is { Count: > 0 })
                    contextList.AddRange(_cachedFungameIds);
                break;
            case "exit":
                contextList.AddRange(["tutorial", "none"]);
                break;
        }

        cmd.argAutofill[key] = contextList;
    }

    private static void ExecuteFungameCommand(string[] args)
    {
        if (args.Length == 1)
        {
            InfoFungame("help");
        }
        else
        {
            switch (args[1])
            {
                case "help":
                    InfoFungame("help");
                    break;
                case "reload":
                    if (!EnsureWorldLoaded()) return;
                    CheckArg(args, 1);
                    MapLoader.ReloadFungameFromDisk(FungameCheck.CurrentFungame);
                    MapLoader.ReloadMap(FungameCheck.CurrentFungame);
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
                        MapLoader.LogFungameList();
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
    }

    private static void HandleWaypoint(string[] args)
    {
        if (!EnsureWorldLoaded()) return;

        var fungame = FungameCheck.CurrentFungame;
        if (fungame == null)
        {
            Error("no_waypoints");
            return;
        }

        var waypoints = GetWaypoints(fungame);

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
                InfoFungame("waypoint.help");
                break;
            case "get":
                if (args.Length < 4)
                {
                    InfoFungame("waypoint.get_no_id");
                    return;
                }

                TeleportToWaypointById(waypoints, args[3]);
                break;
            default:
                InfoFungame("waypoint.unknown_subcommand", subCommand);
                break;
        }
    }

    private static void HandleSave(string[] args)
    {
        // fg save as / fg save as XXX
        if (args.Length is 3 or 4 && args[2].Equals("as", StringComparison.OrdinalIgnoreCase))
        {
            var targetName = args.Length == 4 ? args[3] : null;
            HandleSaveAs(targetName);
            return;
        }

        var fungame = FungameCheck.CurrentFungame;

        string targetPath = null;

        switch (args.Length)
        {
            // fg save XXX
            case 3 when !args[2].Contains(","):
            {
                targetPath = ResolveTargetPath(args[2]);
                if (targetPath == null)
                {
                    ErrorFungame("save.target_not_found", args[2]);
                    return;
                }

                break;
            }
            // fg save xx,xx
            case 3:
                ErrorFungame("save.missing_end_position");
                return;
            // fg save xx,xx xx,xx XXX
            case 5:
            {
                targetPath = ResolveTargetPath(args[4]);
                if (targetPath == null)
                {
                    ErrorFungame("save.target_not_found", args[4]);
                    return;
                }

                break;
            }
        }

        if (fungame == null)
        {
            if (targetPath == null)
            {
                Error("no_fungame");
                return;
            }

            fungame = LoadOrCreateDefaultFungame(targetPath);
        }

        var directoryPath = targetPath ?? fungame.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorFungame("save.no_directory");
            return;
        }

        try
        {
            var jsonPath = Path.Combine(directoryPath, "fungame.json");

            if (args.Length is 4 or 5)
            {
                SaveAreaAsMapData(fungame, jsonPath, args[2], args[3]);
                return;
            }

            using (FileStream output = new(jsonPath, FileMode.Create))
            using (StreamWriter streamWriter = new(output))
            using (JsonTextWriter jsonWriter = new(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;

                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
                serializer.Serialize(jsonWriter, fungame);

                jsonWriter.Flush();
                streamWriter.Flush();
                output.Flush();
            }

            InfoFungame("save.success", fungame.Name, jsonPath);
        }
        catch (Exception ex)
        {
            ErrorFungame("save.failed", fungame.Name, ex.Message);
        }
    }

    private static void HandleSaveAs(string targetName)
    {
        if (!EnsureWorldLoaded()) return;

        GameConsole.Instance.StartCoroutine(SaveAsCoroutine(targetName));
    }

    private static IEnumerator SaveAsCoroutine(string targetName)
    {
        yield return null;

        var fungame = FungameCheck.CurrentFungame;
        string targetPath = null;

        if (!string.IsNullOrEmpty(targetName))
        {
            targetPath = ResolveTargetPath(targetName);
            if (targetPath == null)
            {
                ErrorFungame("save.target_not_found", targetName);
                yield break;
            }

            fungame ??= LoadOrCreateDefaultFungame(targetPath);
        }

        if (fungame == null)
        {
            Error("no_fungame");
            yield break;
        }

        var directoryPath = targetPath ?? fungame.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorFungame("save.no_directory");
            yield break;
        }

        TipFungame("save.as.start_position");

        var waiter1 = Key.WaitForLeftClick();
        yield return waiter1;
        var startPos = waiter1.Result;

        yield return null;

        TipFungame("save.as.end_position");

        var waiter2 = Key.WaitForLeftClick();
        yield return waiter2;
        var endPos = waiter2.Result;

        var jsonPath = Path.Combine(directoryPath, "fungame.json");
        var startStr = $"{startPos.x},{startPos.y}";
        var endStr = $"{endPos.x},{endPos.y}";

        SaveAreaAsMapData(fungame, jsonPath, startStr, endStr);
    }

    private static string ResolveTargetPath(string targetName)
    {
        foreach (var dir in
                 from dir in FungameCheck.ValidDirectories
                 let folderName = Path.GetFileName(dir)
                 where string.Equals(folderName, targetName, StringComparison.OrdinalIgnoreCase)
                 select dir)
        {
            return dir;
        }

        var directPath = Path.Combine(FungameCheck.FungamesPath, targetName);
        if (!Directory.Exists(directPath))
            Directory.CreateDirectory(directPath);

        return directPath;
    }

    private const string DefaultVersion = "1.0.0";
    private static readonly List<string> DefaultAuthor = ["Unknown"];

    private static Fungame LoadOrCreateDefaultFungame(string targetPath)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
            throw new ArgumentException(ModLocale.Log("fungame_load.empty_target_path"), nameof(targetPath));

        var targetJsonPath = Path.Combine(targetPath, "fungame.json");

        var loaded = TryLoadFungame(targetJsonPath, targetPath);
        if (loaded != null)
            return loaded;

        return CreateDefaultFungame(targetPath);
    }

    private static Fungame TryLoadFungame(string jsonPath, string targetPath)
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
            Logger.LogWarning(ModLocale.Log("fungame_load.unauthorized", jsonPath, ex.Message));
            return null;
        }
        catch (Exception ex) when (ex is IOException or PathTooLongException)
        {
            Logger.LogWarning(ModLocale.Log("fungame_load.io_error", jsonPath, ex.Message));
            return null;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            Logger.LogWarning(ModLocale.Log("fungame_load.file_empty", jsonPath));
            return null;
        }

        try
        {
            var loaded = JsonConvert.DeserializeObject<Fungame>(json);
            if (loaded == null)
            {
                Logger.LogWarning(ModLocale.Log("fungame_load.deserialize_null", jsonPath));
                return null;
            }

            loaded.DirectoryPath = targetPath;
            return loaded;
        }
        catch (JsonException ex)
        {
            Logger.LogWarning(ModLocale.Log("fungame_load.invalid_json", jsonPath, ex.Message));
            return null;
        }
    }

    private static Fungame CreateDefaultFungame(string targetPath)
    {
        var folderName =
            Path.GetFileName(targetPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        if (string.IsNullOrWhiteSpace(folderName))
        {
            Logger.LogWarning(ModLocale.Log("fungame_load.no_folder_name", targetPath));
            return null;
        }

        return new Fungame
        {
            Name = folderName,
            Id = folderName.ToLowerInvariant(),
            Version = DefaultVersion,
            Author = DefaultAuthor,
            Description = Fungame("save.as.default_description"),
            DirectoryPath = targetPath
        };
    }

    private static void SaveAreaAsMapData(Fungame fungame, string jsonPath, string startStr, string endStr)
    {
        if (!EnsureWorldLoaded()) return;

        var startParts = startStr.Split(',');
        var endParts = endStr.Split(',');

        if (startParts.Length != 2 || endParts.Length != 2 ||
            !float.TryParse(startParts[0].Trim(), out float wx1) ||
            !float.TryParse(startParts[1].Trim(), out float wy1) ||
            !float.TryParse(endParts[0].Trim(), out float wx2) ||
            !float.TryParse(endParts[1].Trim(), out float wy2))
        {
            ErrorFungame("save.invalid_position");
            return;
        }

        var world = WorldGeneration.world;
        Vector2Int blockA = world.WorldToBlockPos(new Vector2(wx1, wy1));
        Vector2Int blockB = world.WorldToBlockPos(new Vector2(wx2, wy2));

        int minX = Mathf.Min(blockA.x, blockB.x);
        int maxX = Mathf.Max(blockA.x, blockB.x);
        int minY = Mathf.Min(blockA.y, blockB.y);
        int maxY = Mathf.Max(blockA.y, blockB.y);

        int cMinX = Mathf.Clamp(minX, 0, (int)world.width - 1);
        int cMaxX = Mathf.Clamp(maxX, 0, (int)world.width - 1);
        int cMinY = Mathf.Clamp(minY, 0, (int)world.height - 1);
        int cMaxY = Mathf.Clamp(maxY, 0, (int)world.height - 1);

        int regionW = cMaxX - cMinX + 1;
        int regionH = cMaxY - cMinY + 1;

        if (regionW <= 0 || regionH <= 0)
        {
            ErrorFungame("save.area_empty");
            return;
        }

        var blockIds = new ushort[regionW][];
        for (int index = 0; index < regionW; index++)
        {
            blockIds[index] = new ushort[regionH];
        }

        var uniqueBlockIds = new List<ushort>();
        var blockToChar = new Dictionary<ushort, string>();

        uniqueBlockIds.Add(0);
        blockToChar[0] = "0";

        for (int x = 0; x < regionW; x++)
        {
            for (int y = 0; y < regionH; y++)
            {
                int bx = cMinX + x;
                int by = cMaxY - y;
                ushort id = world.GetBlock(new Vector2Int(bx, by));

                blockIds[x][y] = id;

                if (id > 0 && !blockToChar.ContainsKey(id))
                {
                    blockToChar[id] = EncodeBlockIndex(uniqueBlockIds.Count);
                    uniqueBlockIds.Add(id);
                }
            }
        }

        var mapRows = new string[regionH];
        for (int y = 0; y < regionH; y++)
        {
            var chars = new char[regionW];
            for (int x = 0; x < regionW; x++)
            {
                ushort id = blockIds[x][y];
                chars[x] = blockToChar.TryGetValue(id, out string ch)
                    ? ch[0]
                    : '0';
            }

            mapRows[y] = new string(chars);
        }

        var keyDict = new Dictionary<string, object>();
        for (int i = 0; i < uniqueBlockIds.Count; i++)
        {
            keyDict[EncodeBlockIndex(i)] = (long)uniqueBlockIds[i];
        }

        fungame.MapData = new MapData
        {
            Map = mapRows,
            Key = keyDict
        };
        fungame.CustomStructures = null;
        fungame.BuildModeSave = null;

        var json = JsonConvert.SerializeObject(fungame, Formatting.Indented,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        File.WriteAllText(jsonPath, json);

        InfoFungame("save.area_success",
            cMinX, cMinY, cMaxX, cMaxY,
            regionW, regionH, uniqueBlockIds.Count, jsonPath);
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
            ErrorFungame("exit.invalid_target", target);
            return;
        }

        WorldGenerationPatch.ExitTargetScene = target switch
        {
            "tutorial" => WorldGeneration.OverrideSceneType.Tutorial,
            "none" => WorldGeneration.OverrideSceneType.None,
            _ => null
        };

        WorldGenerationPatch.CurrentFungame = null;

        var targetName = target switch
        {
            "tutorial" => Fungame("exit.target.tutorial"),
            "none" => Fungame("exit.target.none"),
            _ => target
        };
        InfoFungame("exiting", targetName);

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

        if (int.TryParse(waypointId, out int index))
        {
            if (index < 1 || index > waypoints.Count)
            {
                ErrorFungame("waypoint.invalid_index", index, waypoints.Count);
                return;
            }

            target = waypoints[index - 1];
            if (target == null)
            {
                ErrorFungame("waypoint.not_found", waypointId);
                return;
            }

            TeleportToWaypoint(target, target.Id ?? $"waypoint_{index}");
            return;
        }

        target = waypoints.Find(wp =>
            wp != null && wp.Id?.Equals(waypointId, StringComparison.OrdinalIgnoreCase) == true);

        if (target == null)
        {
            ErrorFungame("waypoint.not_found", waypointId);
            return;
        }

        TeleportToWaypoint(target, target.Id);
    }

    private static void TeleportToWaypoint(WaypointData waypointData, string displayId)
    {
        InfoFungame("waypoint.teleport", displayId, waypointData.Position);
        Player.Tp(waypointData.Position);
    }

    private static List<WaypointData> GetWaypoints(Fungame fungame)
    {
        if (fungame.Waypoints is { Count: > 0 })
            return fungame.Waypoints;

        if (fungame.WaypointData != null)
            return [fungame.WaypointData];

        return [];
    }

    private static void ListWaypoints(List<WaypointData> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Error("no_waypoints");
            return;
        }

        Log.Divider();
        InfoFungame("waypoint.list_header", waypoints.Count);

        for (int i = 0; i < waypoints.Count; i++)
        {
            var wp = waypoints[i];
            if (wp != null)
                InfoFungame("waypoint.list_item", i + 1, wp.Id ?? $"waypoint_{i + 1}", wp.Position);
        }

        Log.Divider();
    }

    private static void HandleFeature(string[] args)
    {
        var fungame = FungameCheck.CurrentFungame;
        if (fungame?.Feature == null)
        {
            Error("no_fungame");
            return;
        }

        switch (args.Length)
        {
            case < 3:
                InfoFungame("help");
                ListFeatures(fungame.Feature);
                return;
            case >= 4:
                SetFeature(fungame.Feature, args[2], args[3]);
                break;
            default:
                InfoFungame("feature.set_missing_params");
                break;
        }
    }

    private static void ListFeatures(Feature feature)
    {
        Log.Divider();
        InfoFungame("feature.list_header");

        foreach (var prop in FeatureProperties)
        {
            var value = prop.GetValue(feature);
            var jsonName = prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name;
            var displayName = Locale($"feature.{GetFeatureDisplayName(prop.Name)}");

            if (value != null && !IsSimpleType(prop.PropertyType))
            {
                InfoFungame("feature.parent_item", displayName, jsonName);
                foreach (var subProp in prop.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!IsSimpleType(subProp.PropertyType)) continue;
                    var subValue = subProp.GetValue(value);
                    var subJson = subProp.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? subProp.Name;
                    var subDisplay = Locale($"feature.{GetFeatureDisplayName($"{jsonName}.{subJson}")}");
                    InfoFungame("feature.item", $"    {subDisplay}", subJson, subValue);
                }
            }
            else
            {
                InfoFungame("feature.item", displayName, jsonName, value);
            }
        }

        Log.Divider();
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal)
               || type == typeof(float) || type == typeof(double) || type == typeof(bool);
    }

    // todo: 没搞好c

    private static void SetFeature(Feature feature, string featureName, string valueStr)
    {
        var parts = featureName.Split('.');
        object target = feature;
        PropertyInfo targetProp = null;

        foreach (var part in parts)
        {
            if (target == null) break;
            targetProp = FindNestedProperty(target.GetType(), part);
            if (targetProp == null) break;
            if (part != parts.Last())
                target = targetProp.GetValue(target);
        }

        if (targetProp == null)
        {
            ErrorFungame("feature.not_found", featureName);
            return;
        }

        try
        {
            var convertedValue = Convert.ChangeType(valueStr, targetProp.PropertyType);
            targetProp.SetValue(target, convertedValue);
            var displayName = Locale($"feature.{GetFeatureDisplayName(featureName)}");
            InfoFungame("feature.set_success", $"{displayName} ({featureName})", convertedValue);
        }
        catch (Exception)
        {
            ErrorFungame("feature.invalid_value", featureName, valueStr);
        }
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
        if (string.IsNullOrEmpty(fieldName))
            return fieldName;

        var parts = fieldName.Split('.');
        string result;

        if (parts.Length >= 2)
        {
            var parentProp = typeof(Feature).GetProperties()
                .FirstOrDefault(p => (p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? p.Name)
                    .Equals(parts[0], StringComparison.OrdinalIgnoreCase));

            if (parentProp != null)
            {
                var parentJson = parentProp.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ??
                                 parentProp.Name;
                var subProp = parentProp.PropertyType.GetProperties()
                    .FirstOrDefault(p => (p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? p.Name)
                        .Equals(parts[1], StringComparison.OrdinalIgnoreCase));
                var subJson = subProp?.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? parts[1];
                if (parentJson.EndsWith("data"))
                    parentJson = parentJson.Substring(0, parentJson.Length - 4);
                result = $"{parentJson}.{subJson}";
            }
            else
            {
                result = fieldName.ToLower().Replace(".", "_");
            }
        }
        else
        {
            var prop = typeof(Feature).GetProperty(fieldName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                var jsonName = prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name.ToLower();
                bool isDataClass = prop.PropertyType.Name.EndsWith("Data", StringComparison.Ordinal);
                result = isDataClass ? $"{jsonName}_data" : jsonName;
            }
            else
            {
                result = fieldName.ToLower();
            }
        }

        return result;
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
            InfoFungame("select.no_key");
            return;
        }

        if (FungameCheck.Fungames == null || FungameCheck.Fungames.Count == 0)
        {
            InfoFungame("list.empty");
            return;
        }

        Fungame fungame;

        if (int.TryParse(key, out int index))
        {
            if (index < 1 || index > FungameCheck.Fungames.Count)
            {
                InfoFungame("select.invalid_index", index, FungameCheck.Fungames.Count);
                return;
            }

            fungame = FungameCheck.Fungames[index - 1];
        }
        else
        {
            fungame = FungameCheck.Fungames.Find(f =>
                f != null &&
                (f.Id?.Equals(key, StringComparison.OrdinalIgnoreCase) == true ||
                 f.Name?.Equals(key, StringComparison.OrdinalIgnoreCase) == true));
        }

        if (fungame == null)
        {
            InfoFungame("select.not_found", key);
            return;
        }

        WorldGenerationPatch.CurrentFungame = fungame;

        InfoFungame("select.success", fungame.Name, fungame.Id);

        if (HasWorldLoaded())
        {
            MapLoader.ReloadMap(fungame);
            MapLoader.LogMapInfo();
        }
        else
            InfoFungame("select.without_world", fungame.Name);
    }

    private static void CheckArg(string[] args, int index)
    {
        Tools.CheckArgumentCount(args, index);
    }

    private static void Spawn()
    {
        var fungame = FungameCheck.CurrentFungame;
        InfoFungame("spawn", fungame.SpawnPosition);
        Player.Tp(fungame.SpawnPosition);
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.GetFormat(key, args);
    }

    private static string Command(string key, params object[] args)
    {
        return Locale($"command.{key}", args);
    }

    private static string Fungame(string key, params object[] args)
    {
        return Command($"fungame.{key}", args);
    }

    private static void TipFungame(string key, params object[] args)
    {
        var message = Fungame(key, args);
        Log.Alert(message, Logger, false);
    }

    private static void InfoFungame(string key, params object[] args)
    {
        var message = Fungame(key, args);
        Log.Info(message, Logger);
    }

    private static void ErrorFungame(string key, params object[] args)
    {
        var message = Fungame(key, args);
        Log.Error(message, Logger);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Info(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Warning(message, Logger);
    }
}