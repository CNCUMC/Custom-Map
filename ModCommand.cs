using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using MossLib.Base;
using MossLib.Tool;
using UnityEngine;

namespace CustomFungamePack;

[HarmonyPatch(typeof(ConsoleScript))]
public class ModCommand : ModCommandBase
{
    private new static readonly ManualLogSource Logger = Plugin.Logger;
    private const string LocaleKeyPre = "mod_command.";

    // Feature uses auto-properties, not public fields — must use GetProperties()
    private static readonly PropertyInfo[] FeatureProperties =
        typeof(Feature).GetProperties(BindingFlags.Public | BindingFlags.Instance);

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
                        "waypoint"
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
                argAutofill,
                paramDescriptions)
            );
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to register custom commands: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void ExecuteFungameCommand(string[] args)
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
                Select(args[2]);
                break;
            case "list":
                CheckArg(args, 1);
                if (args.Length > 2)
                    Select(args[2]);
                else
                    MapLoader.LogFungameList(); 
                break;
            case "feature":
                HandleFeature(args);
                break;
            case "waypoint":
                HandleWaypoint(args);
                break;
            default:
                InfoFungame("help");
                break;
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
            ListFeatures(fungame.Feature);
            return;
        }

        var subCommand = args[2].ToLower();

        switch (subCommand)
        {
            case "list":
                ListFeatures(fungame.Feature);
                break;
            case "help":
                InfoFungame("feature.help");
                break;
            case "get":
                if (args.Length < 4)
                {
                    InfoFungame("feature.get_no_name");
                    return;
                }

                GetFeature(fungame.Feature, args[3]);
                break;
            case "set":
                if (args.Length < 5)
                {
                    InfoFungame("feature.set_missing_params");
                    return;
                }

                SetFeature(fungame.Feature, args[3], args[4]);
                break;
            default:
                InfoFungame("feature.unknown_subcommand", subCommand);
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
        } else
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