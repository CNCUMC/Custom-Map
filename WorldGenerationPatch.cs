using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using MossLib;
using System.IO;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace CustomFungamePack;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LogPrefix = "log.";
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
                    Info("world_generation.scene_type_set", fungame.MapData.Type);
                }
                else
                {
                    SetDefaultSceneType(WorldGeneration);
                }
            }
            else
            {
                Error("error.no_fungame_file", fungameFilePath);
                SetDefaultSceneType(WorldGeneration);
            }
        }
        else
        {
            Error("error.no_valid_directories");
            SetDefaultSceneType(WorldGeneration);
        }
    }

    private static void ApplyFeaturesEarly(Dictionary<string, float> features)
    {
        if (features == null || features.Count == 0)
        {
            Info("world_generation.no_features_enabled");
            return;
        }

        foreach (var feature in features)
        {
            switch (feature.Key.ToLower())
            {
                case "fullbright":
                    Info("world_generation.feature_enabled", ModLocale.GetFormat("feature.fullbright"));
                    Tools.RunCommand("fullbright");
                    break;
                case "forgiving_level":
                    Info("world_generation.feature_enabled", ModLocale.GetFormat("log.common.forgiving_level_mode"));
                    break;
                case "gravity":
                    Info("world_generation.feature_enabled_with_value", ModLocale.GetFormat("feature.gravity"),
                        feature.Value);
                    break;
                default:
                    Warning("world_generation.unknown_feature", feature.Key);
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
                Tools.Tp(new Vector2(main.transform.position.x, mapTop));
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
        Info("world_generation.skip_generation", ModLocale.GetFormat("log.common.background"));
        return false;
    }
    
    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentFungame?.MapData?.SkipStructures != true) return true;
        Info("world_generation.skip_generation", ModLocale.GetFormat("log.common.structure"));
        return false;
    }
    
    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentFungame?.MapData?.SkipTerrain != true) return true;
        Info("world_generation.skip_generation", ModLocale.GetFormat("log.common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld()
    {
        WorldGeneration.loadingText.text = Locale("world_generation.initializing_world");

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
                    Info("world_generation.loading_fungame_map", fungame.Name);
                    WorldGeneration.loadingText.text = Locale("world_generation.loading_fungame_map", fungame.Name);
                    MapLoader.LoadAndApplyMapFromFungame(fungame);
                    ExecuteCommands(fungame);
                    Tools.Tp(fungame.SpawnPosition);

                    string authors = fungame.Author is { Count: > 0 }
                        ? string.Join(", ", fungame.Author)
                        : "Unknown";
                    Tools.LogCla($"{fungame.Name} v{fungame.Version}\nby {authors}", Logger, true);
                    Tools.LogCla($"{fungame.Description}", Logger, false, 6f);
                }
                else
                {
                    Warning("world_generation.no_map_data", fungame?.Name ?? "Unknown");
                }
            }
            else
            {
                Error("error.no_fungame_file", fungameFilePath);
            }
        }
        else
        {
            Error("error.no_valid_directories");
        }
    }

    private static void ExecuteCommands(Fungame fungame)
    {
        var commands = fungame.Command;
        if (commands == null || commands.Count == 0)
        {
            Info("world_generation.no_commands", ModLocale.GetFormat("log.common.startup_command"));
            return;
        }

        foreach (var command in commands)
        {
            Info("world_generation.executing_command", ModLocale.GetFormat("log.common.startup_command"), command);
            Tools.RunCommand(command);
        }
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LogPrefix}{key}", args);
        Tools.LogInfo(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LogPrefix}{key}", args);
        Tools.LogError(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LogPrefix}{key}", args);
        Tools.LogWarning(message, Logger);
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.GetFormat($"{LogPrefix}{key}", args);
    }
}