using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
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
    private static float _sLoopTimer;

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
                WorldGeneration.biomeOverride = fungame.Type;
                MoreLogs("scene_type_set", fungame.Type);
            }
            else
            {
                SetDefaultSceneType(WorldGeneration, fungame);
            }
        }
        else
        {
            var fungame = FungameCheck.Fungames.FirstOrDefault();
            if (fungame != null)
            {
                CurrentFungame = fungame;

                if (fungame.MapData != null)
                {
                    WorldGeneration.biomeOverride = fungame.Type;
                    MoreLogs("scene_type_set", fungame.Type);
                }
                else
                {
                    SetDefaultSceneType(WorldGeneration, fungame);
                }
            }
            else
            {
                Log.Warning("No valid Fungame directories, please check the Fungames folder", Logger);
                SetDefaultSceneType(WorldGeneration);
            }
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
        if (CurrentFungame.Feature.Fullbright)
            Console.ConsoleScript.fullBright = CurrentFungame.Feature.Fullbright;

        HandleLoopCommands();
    }

    private static void SetDefaultSceneType(WorldGeneration __instance, Fungame fungame = null)
    {
        __instance.biomeOverride = fungame?.Type ?? WorldGeneration.OverrideSceneType.Debug;
        MoreLogs("scene_type_set", __instance.biomeOverride);
    }

    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (!CurrentFungame.SkipBackground) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (!CurrentFungame.SkipStructures) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.structure"));
        return false;
    }

    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (!CurrentFungame.SkipTerrain) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld()
    {
        WorldGeneration.loadingText.text = Locale("initializing_world");

        var fungame = FungameCheck.CurrentFungame
                      ?? FungameCheck.Fungames.FirstOrDefault();

        if (fungame == null)
        {
            Error("no_valid_directories");
            return;
        }

        bool hasMapData = fungame.MapData != null;
        bool hasCustomStructures = !string.IsNullOrEmpty(fungame.CustomStructures);
        bool hasBuildModeSave = !string.IsNullOrEmpty(fungame.BuildModeSave);

        int contentTypeCount = (hasMapData ? 1 : 0)
                               + (hasCustomStructures ? 1 : 0)
                               + (hasBuildModeSave ? 1 : 0);

        if (contentTypeCount > 1)
        {
            Error("multiple_content_types", fungame.Name);
            return;
        }

        if (contentTypeCount == 0)
        {
            Warning("no_content_type", fungame.Name);
            return;
        }

        if (hasMapData)
        {
            SpawnMap(fungame);
        }
        else if (hasCustomStructures)
        {
            bool hasCustomStructuresMod = Type.GetType(
                "Custom_Structures.Plugin, Custom Structures") != null;
            if (hasCustomStructuresMod)
            {
                CustomStructuresLoader.SpawnCustomStructures(fungame);
            }
            else
            {
                Error("custom_structures_mod_not_loaded", fungame.Name);
            }
        }
        else if (hasBuildModeSave)
        {
            BuildModeSaveLoader.SpawnBuildModeSave(fungame);
        }
    }

    private static void SpawnMap(Fungame fungame)
    {
        MoreLogs("loading_fungame_map", fungame.Name);
        WorldGeneration.loadingText.text = Locale("loading_fungame_map", fungame.Name);
        MapLoader.LoadAndApplyMapFromFungame(fungame);
        ExecuteCommands(fungame);

        string modInfo = $"{fungame.Name} v{fungame.Version}";
        string authorInfo = $"by {fungame.Authors}";
        string description = fungame.Description;

        Player.Alert($"{modInfo}\n{authorInfo}", true);
        Player.Alert(description, false, 6f);
        MapLoader.LogMapInfo();
        Player.Tp(fungame.SpawnPosition);
    }

    private static void ExecuteCommands(Fungame fungame)
    {
        var commands = fungame.CommandData;
        if (commands == null || (commands.OnceCommands == null || commands.OnceCommands.Count == 0) &&
            (commands.LoopCommands == null || commands.LoopCommands.Count == 0))
        {
            MoreLogs("no_commands", ModLocale.Log("common.startup_command"));
            return;
        }

        if (commands.OnceCommands != null)
        {
            foreach (var command in commands.OnceCommands)
            {
                MoreLogs("executing_command", ModLocale.Log("common.startup_command"), command);
                Console.RunCommand(command);
            }
        }

        _sLoopTimer = 0f;
    }

    private static void HandleLoopCommands()
    {
        var loopCommands = CurrentFungame?.CommandData?.LoopCommands;
        if (loopCommands == null || loopCommands.Count == 0) return;

        var interval = CurrentFungame.CommandData.LoopInterval > 0
            ? CurrentFungame.CommandData.LoopInterval
            : 10f;

        _sLoopTimer += Time.deltaTime;

        if (!(_sLoopTimer >= interval)) return;
        _sLoopTimer = 0f;

        foreach (var command in loopCommands)
        {
            MoreLogs("executing_loop_command", ModLocale.Log("common.loop_command"), command);
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