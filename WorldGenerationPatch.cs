using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using MossLib;
using System.IO;
using MossLib.Tool;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace CustomFungamePack;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LocaleKeyPre = "log.world_generation.";
    private static readonly ManualLogSource Logger = Plugin.Logger;
    internal static WorldGeneration WorldGeneration;
    internal static Fungame CurrentFungame;

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void ForceEnableTutorial(WorldGeneration __instance)
    {
        WorldGeneration = __instance;

        if (FungameCheck.ValidDirectories.Count > 0)
        {
            var firstFungameDir = FungameCheck.ValidDirectories[0];
            var fungameFilePath = Path.Combine(firstFungameDir, "fungame.json");

            if (File.Exists(fungameFilePath))
            {
                var jsonContent = File.ReadAllText(fungameFilePath);
                var fungame = Newtonsoft.Json.JsonConvert.DeserializeObject<Fungame>(jsonContent);

                if (fungame == null) return;
                CurrentFungame = fungame;

                ApplyFeaturesEarly(fungame.Feature);

                if (fungame.MapData != null)
                {
                    WorldGeneration.biomeOverride = fungame.MapData.Type;
                    Info("scene_type_set", fungame.MapData.Type);
                }
                else
                {
                    SetDefaultSceneType(WorldGeneration);
                }
            }
            else
            {
                Error("no_fungame_file", fungameFilePath);
                SetDefaultSceneType(WorldGeneration);
            }
        }
        else
        {
            Error("no_valid_directories");
            SetDefaultSceneType(WorldGeneration);
        }
    }

    private static void ApplyFeaturesEarly(Dictionary<string, float> features)
    {
        if (features == null || features.Count == 0)
        {
            Info("no_features_enabled");
            return;
        }

        foreach (var feature in features)
        {
            switch (feature.Key.ToLower())
            {
                case "fullbright":
                    Info("feature_enabled", ModLocale.GetFormat("feature.fullbright"));
                    Console.RunCommand("fullbright");
                    break;
                case "forgiving_level":
                    Info("feature_enabled", ModLocale.GetFormat("log.common.forgiving_level_mode"));
                    break;
                case "gravity":
                    Info("feature_enabled_with_value", ModLocale.GetFormat("feature.gravity"),
                        feature.Value);
                    break;
                default:
                    Warning("unknown_feature", feature.Key);
                    break;
            }
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void ForgivingLevel()
    {
        if (IsFeatureEnabled("forgiving_level") && CurrentFungame?.MapData != null)
        {
            var mapBottom = -WorldGeneration.halfHeight + 10;
            var mapTop = WorldGeneration.halfHeight - 10;

            var main = PlayerCamera.main;
            if (main.body.transform.position.y <= mapBottom
                || main.transform.position.y <= mapBottom)
            {
                Player.Tp(new Vector2(main.transform.position.x, mapTop));
            }
        }

        if (!IsFeatureEnabled("gravity")) return;
        var gravityScale = GetFeatureValue("gravity");
        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);
    }

    private static bool IsFeatureEnabled(string featureName)
    {
        return CurrentFungame?.Feature != null && CurrentFungame.Feature.ContainsKey(featureName);
    }

    private static float GetFeatureValue(string featureName)
    {
        if (CurrentFungame?.Feature != null && CurrentFungame.Feature.TryGetValue(featureName, out var value))
        {
            return value;
        }

        return FeatureConfig.GetDefault(featureName);
    }

    private static void SetDefaultSceneType(WorldGeneration __instance)
    {
        __instance.biomeOverride = WorldGeneration.OverrideSceneType.Debug;
    }

    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (CurrentFungame?.MapData?.SkipBackground != true) return true;
        Info("skip_generation", ModLocale.GetFormat("log.common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentFungame?.MapData?.SkipStructures != true) return true;
        Info("skip_generation", ModLocale.GetFormat("log.common.structure"));
        return false;
    }

    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentFungame?.MapData?.SkipTerrain != true) return true;
        Info("skip_generation", ModLocale.GetFormat("log.common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld()
    {
        WorldGeneration.loadingText.text = Locale("initializing_world");

        if (FungameCheck.ValidDirectories.Count > 0)
        {
            var firstFungameDir = FungameCheck.ValidDirectories[0];
            var fungameFilePath = Path.Combine(firstFungameDir, "fungame.json");

            if (File.Exists(fungameFilePath))
            {
                var jsonContent = File.ReadAllText(fungameFilePath);
                var fungame = Newtonsoft.Json.JsonConvert.DeserializeObject<Fungame>(jsonContent);

                if (fungame?.MapData != null)
                {
                    Info("loading_fungame_map", fungame.Name);
                    WorldGeneration.loadingText.text = Locale("loading_fungame_map", fungame.Name);
                    MapLoader.LoadAndApplyMapFromFungame(fungame);
                    ExecuteCommands(fungame);
                    Player.Tp(fungame.SpawnPosition);

                    string modInfo = $"{fungame.Name} v{fungame.Version}";
                    string authorInfo = $"by {fungame.Authors}";
                    string description = fungame.Description;

                    Player.Alert($"{modInfo}\n{authorInfo}", true);
                    Player.Alert(description, false, 6f);
                    Log.Info(modInfo, Logger);
                    Log.Info(authorInfo, Logger);
                    Log.Info(description, Logger);
                }
                else
                {
                    Warning("no_map_data", fungame?.Name ?? "Unknown");
                }
            }
            else
            {
                Error("no_fungame_file", fungameFilePath);
            }
        }
        else
        {
            Error("no_valid_directories");
        }
    }

    private static void ExecuteCommands(Fungame fungame)
    {
        var commands = fungame.Command;
        if (commands == null || commands.Count == 0)
        {
            Info("no_commands", ModLocale.GetFormat("log.common.startup_command"));
            return;
        }

        foreach (var command in commands)
        {
            Info("executing_command", ModLocale.GetFormat("log.common.startup_command"), command);
            Console.RunCommand(command);
        }
    }

    private static void Info(string key, params object[] args)
    {
        var message = Locale(key, args);
        Log.Info(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"log.error.{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = Locale(key, args);
        Log.Warning(message, Logger);
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
    }
}