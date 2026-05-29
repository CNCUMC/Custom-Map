using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
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
    private static List<string> _cachedFeatureNames = new();
    private static List<string> _cachedFungameNames = new();

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

        var fungameNames = FungameCheck.Fungames?
            .Where(f => f != null)
            .Select(f => f.Name)
            .Where(name => !string.IsNullOrEmpty(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var featureNames = FeatureProperties
            .Select(p => p.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _cachedFeatureNames = featureNames;
        _cachedFungameNames = fungameNames ?? new List<string>();

        if ((fungameNames is not { Count: > 0 }) && featureNames.Count == 0)
        {
            _autofillRegistered = true;
            return;
        }

        foreach (var cmd in targetCommands)
        {
            if (featureNames.Count > 0 || fungameNames is { Count: > 0 })
            {
                if (!cmd.argAutofill.ContainsKey(1))
                    cmd.argAutofill[1] = new List<string>();

                if (featureNames.Count > 0)
                    cmd.argAutofill[1].AddRange(featureNames);

                if (fungameNames is { Count: > 0 })
                    cmd.argAutofill[1].AddRange(fungameNames);
            }
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

        switch (subcommand)
        {
            case "feature":
                contextList.AddRange(_cachedFeatureNames);
                break;
            case "select":
            case "list":
                if (_cachedFungameNames.Count > 0)
                    contextList.AddRange(_cachedFungameNames);
                break;
            default:
                contextList.AddRange(_cachedFeatureNames);
                if (_cachedFungameNames.Count > 0)
                    contextList.AddRange(_cachedFungameNames);
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
        var fungame = FungameCheck.CurrentFungame;

        string targetPath = null;

        if (args.Length == 3)
        {
            if (!args[2].Contains(","))
            {
                targetPath = ResolveTargetPath(args[2]);
                if (targetPath == null)
                {
                    ErrorFungame("save.target_not_found", args[2]);
                    return;
                }
            }
            else
            {
                ErrorFungame("save.missing_end_coord");
                return;
            }
        }
        else if (args.Length == 5)
        {
            targetPath = ResolveTargetPath(args[4]);
            if (targetPath == null)
            {
                ErrorFungame("save.target_not_found", args[4]);
                return;
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

    private static string ResolveTargetPath(string targetName)
    {
        foreach (var dir in FungameCheck.ValidDirectories)
        {
            var folderName = Path.GetFileName(dir);
            if (string.Equals(folderName, targetName, StringComparison.OrdinalIgnoreCase))
                return dir;
        }

        var directPath = Path.Combine(FungameCheck.FungamesPath, targetName);
        if (Directory.Exists(directPath))
            return directPath;

        return null;
    }

    private static Fungame LoadOrCreateDefaultFungame(string targetPath)
    {
        var targetJsonPath = Path.Combine(targetPath, "fungame.json");
        if (File.Exists(targetJsonPath))
        {
            try
            {
                var json = File.ReadAllText(targetJsonPath);
                var loaded = JsonConvert.DeserializeObject<Fungame>(json);
                if (loaded != null)
                {
                    loaded.DirectoryPath = targetPath;
                    return loaded;
                }
            }
            catch
            {
            }
        }

        var folderName = Path.GetFileName(targetPath);
        return new Fungame
        {
            Name = folderName,
            Id = folderName.ToLower(),
            Version = "1.0.0",
            Author = ["Unknown"],
            Description = $"Saved from area scan",
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
            ErrorFungame("save.invalid_coordinates");
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

        var blockIds = new ushort[regionW, regionH];

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

                blockIds[x, y] = id;

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
                ushort id = blockIds[x, y];
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

        WorldGenerationPatch.ExitTargetScene = WorldGeneration.OverrideSceneType.None;

        if (args.Length >= 3)
        {
            var target = args[2].ToLower();
            switch (target)
            {
                case "tutorial":
                    WorldGenerationPatch.ExitTargetScene = WorldGeneration.OverrideSceneType.Tutorial;
                    break;
                case "none":
                    WorldGenerationPatch.ExitTargetScene = WorldGeneration.OverrideSceneType.None;
                    break;
                default:
                    ErrorFungame("exit.invalid_target", target);
                    WorldGenerationPatch.ExitTargetScene = null;
                    return;
            }
        }

        WorldGenerationPatch.CurrentFungame = null;
        InfoFungame("exit", WorldGenerationPatch.ExitTargetScene.Value);

        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private static string EncodeBlockIndex(int index)
    {
        if (index < 10)
            return ((char)('0' + index)).ToString();
        if (index < 36)
            return ((char)('a' + index - 10)).ToString();
        return ((char)('A' + index - 36)).ToString();
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

        if (args.Length < 3)
        {
            InfoFungame("feature.help");
            ListFeatures(fungame.Feature);
            return;
        }

        var subCommand = args[2].ToLower();

        if (args.Length < 5)
        {
            if (args.Length < 4)
            {
                InfoFungame("feature.set_missing_params");
                return;
            }

            SetFeature(fungame.Feature, args[2], args[3]);
        }
        else
        {
            InfoFungame("feature.unknown_subcommand", subCommand);
        }
    }

    private static void ListFeatures(Feature feature)
    {
        Log.Divider();
        InfoFungame("feature.list_header");

        foreach (var prop in FeatureProperties)
        {
            var value = prop.GetValue(feature);
            var displayName = GetFeatureDisplayName(prop.Name);
            InfoFungame("feature.item", $"{displayName} ({prop.Name})", value);
        }

        Log.Divider();
    }

    private static void GetFeature(Feature feature, string featureName)
    {
        var prop = FindFeatureProperty(featureName);

        if (prop == null)
        {
            ErrorFungame("feature.not_found", featureName);
            return;
        }

        var value = prop.GetValue(feature);
        var displayName = GetFeatureDisplayName(prop.Name);
        InfoFungame("feature.get_success", $"{displayName} ({prop.Name})", value);
    }

    private static void SetFeature(Feature feature, string featureName, string valueStr)
    {
        var prop = FindFeatureProperty(featureName);

        if (prop == null)
        {
            ErrorFungame("feature.not_found", featureName);
            return;
        }

        try
        {
            var convertedValue = Convert.ChangeType(valueStr, prop.PropertyType);
            prop.SetValue(feature, convertedValue);

            ApplyFeatureSideEffects(prop, convertedValue);

            var displayName = GetFeatureDisplayName(prop.Name);
            InfoFungame("feature.set_success", $"{displayName} ({prop.Name})", convertedValue);
        }
        catch (Exception)
        {
            ErrorFungame("feature.invalid_value", featureName, valueStr);
        }
    }

    private static void ApplyFeatureSideEffects(PropertyInfo prop, object value)
    {
        if (prop.PropertyType == typeof(float)
            && prop.Name.Equals("Gravity", StringComparison.OrdinalIgnoreCase))
        {
            Physics2D.gravity = new Vector2(0, (float)value);
        }
    }

    private static PropertyInfo FindFeatureProperty(string featureName)
    {
        foreach (var prop in FeatureProperties)
        {
            if (prop.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase))
                return prop;

            var displayName = GetFeatureDisplayName(prop.Name);
            if (displayName.Equals(featureName, StringComparison.OrdinalIgnoreCase))
                return prop;
        }

        return null;
    }

    private static string GetFeatureDisplayName(string fieldName)
    {
        var localeKey = $"feature.{ConvertToSnakeCase(fieldName)}";
        var localized = ModLocale.GetFormat(localeKey);

        return localized.StartsWith("feature.", StringComparison.Ordinal)
            ? fieldName
            : localized;
    }

    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLower(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
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
            return WorldGeneration.world != null && !WorldGeneration.world.generatingWorld;
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