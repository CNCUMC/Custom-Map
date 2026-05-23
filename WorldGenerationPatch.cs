using System;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using MossLib.Tool;
using UnityEngine;
using Console = MossLib.Tool.Console;

namespace CustomFungamePack;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LocaleKeyPre = "world_generation.";
    private static readonly ManualLogSource Logger = Plugin.Logger;
    public static WorldGeneration WorldGeneration;
    internal static Fungame CurrentFungame;

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void Awake(WorldGeneration __instance)
    {
        WorldGeneration = __instance;

        if (FungameCheck.HasRunningFungame)
        {
            var fungame = FungameCheck.CurrentFungame;

            CurrentFungame = fungame;

            if (fungame.MapData != null)
            {
                WorldGeneration.biomeOverride = fungame.MapData.Type;
                MoreLogs("scene_type_set", fungame.MapData.Type);
            }
            else
            {
                SetDefaultSceneType(WorldGeneration);
            }
        }
        else if (FungameCheck.ValidDirectories.Count > 0)
        {
            var firstFungameDir = FungameCheck.ValidDirectories[0];
            var fungameFilePath = Path.Combine(firstFungameDir, "fungame.json");

            if (File.Exists(fungameFilePath))
            {
                var jsonContent = File.ReadAllText(fungameFilePath);
                var fungame = Newtonsoft.Json.JsonConvert.DeserializeObject<Fungame>(jsonContent);

                if (fungame == null) return;
                CurrentFungame = fungame;

                if (fungame.MapData != null)
                {
                    WorldGeneration.biomeOverride = fungame.MapData.Type;
                    MoreLogs("scene_type_set", fungame.MapData.Type);
                }
                else
                {
                    SetDefaultSceneType(WorldGeneration);
                }
            }
            else
            {
                Log.Warning($"Cannot find fungame.json file: {fungameFilePath}", Logger);
                SetDefaultSceneType(WorldGeneration);
            }
        }
        else
        {
            Log.Warning("No valid Fungame directories, please check the Fungames folder", Logger);
            SetDefaultSceneType(WorldGeneration);
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void Update()
    {
        if (CurrentFungame == null) return;

        var features = CurrentFungame.Feature;
        if (features.ForgivingLevel)
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

        Physics2D.gravity = new Vector2(0, features.Gravity);
    }

    private static void SetDefaultSceneType(WorldGeneration __instance)
    {
        __instance.biomeOverride = WorldGeneration.OverrideSceneType.Debug;
    }

    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (!CurrentFungame.Feature.SkipBackground) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (!CurrentFungame.Feature.SkipStructures) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.structure"));
        return false;
    }

    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (!CurrentFungame.Feature.SkipTerrain) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld()
    {
        WorldGeneration.loadingText.text = Locale("initializing_world");

        var fungame = FungameCheck.GetRunningFungame();

        if (fungame is { MapData: not null })
        {
            SpawnMap(fungame);
        }
        else if (FungameCheck.ValidDirectories.Count > 0)
        {
            var firstFungameDir = FungameCheck.ValidDirectories[0];
            var fungameFilePath = Path.Combine(firstFungameDir, "fungame.json");

            if (!File.Exists(fungameFilePath)) return;
            var jsonContent = File.ReadAllText(fungameFilePath);
            var fallbackFungame = Newtonsoft.Json.JsonConvert.DeserializeObject<Fungame>(jsonContent);

            if (fallbackFungame?.MapData != null)
            {
                SpawnMap(fallbackFungame);
            }
            else if (fallbackFungame?.CustomStructures != null)
            {
                bool hasCustomStructuresMod = Type.GetType("Custom_Structures.Plugin, Custom Structures") != null;
                if (hasCustomStructuresMod)
                {
                    CustomStructuresLoader.SpawnCustomStructures(fallbackFungame);
                }
                else
                {
                    Error("custom_structures_mod_not_loaded", fallbackFungame.Name);
                }
            }
            else
            {
                Warning("no_map_data", fallbackFungame?.Name ?? "Unknown");
            }
        }
        else
        {
            Error("no_valid_directories");
        }
    }

    [HarmonyPatch(typeof(ConsoleScript), "Update")]
    [HarmonyPostfix]
    public static void ConsoleScriptUpdate(ConsoleScript __instance)
    {
        if (CurrentFungame?.Feature == null) return;
        if (CurrentFungame.Feature.Fullbright) __instance.fullBright = true;
    }

    private static void SpawnMap(Fungame fungame)
    {
        MoreLogs("loading_fungame_map", fungame.Name);
        WorldGeneration.loadingText.text = Locale("loading_fungame_map", fungame.Name);
        MapLoader.LoadAndApplyMapFromFungame(fungame);
        ExecuteCommands(fungame);
        Player.Tp(fungame.SpawnPosition);

        string modInfo = $"{fungame.Name} v{fungame.Version}";
        string authorInfo = $"by {fungame.Authors}";
        string description = fungame.Description;

        Player.Alert($"{modInfo}\n{authorInfo}", true);
        Player.Alert(description, false, 6f);
    }

    private static void ExecuteCommands(Fungame fungame)
    {
        var commands = fungame.Command;
        if (commands == null || commands.Count == 0)
        {
            MoreLogs("no_commands", ModLocale.Log("common.startup_command"));
            return;
        }

        foreach (var command in commands)
        {
            MoreLogs("executing_command", ModLocale.Log("common.startup_command"), command);
            Console.RunCommand(command);
        }
    }
        
    private static void MoreLogs(string key, params object[] args)
    {
        if (Configs.MoreLogs)
            Info(key, args);
    }

    private static void Info(string key, params object[] args)
    {
        var message = Locale(key, args);
        Log.Info(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.Log($"error.{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = Locale(key, args);
        Log.Warning(message, Logger);
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.Log($"{LocaleKeyPre}{key}", args);
    }
}