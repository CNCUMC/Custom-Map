using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using CustomFungamePack.Loader;
using HarmonyLib;
using MossLib.Tool;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LocaleKeyPre = "world_generation.";
    private static readonly ManualLogSource Logger = Plugin.Logger;
    public static WorldGeneration WorldGeneration;
    internal static Fungame CurrentFungame;
    private static float _sLoopTimer;
    public static WorldGeneration.OverrideSceneType? ExitTargetScene;

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
                SuppressCustomStructuresForMapData();
                WorldGeneration.biomeOverride = fungame.Type;
                MoreLogs("scene_type_set", fungame.Type);
            }
            else
            {
                SetDefaultSceneType(WorldGeneration, fungame);
            }
        }
        else if (ExitTargetScene.HasValue)
        {
            WorldGeneration.biomeOverride = ExitTargetScene.Value;
            MoreLogs("scene_type_set", ExitTargetScene.Value);
        }
        else if (ModConfigs.StartGameUseFungame)
        {
            Fungame fungame = null;

            if (!string.IsNullOrEmpty(ModConfigs.FirstUseFungame))
            {
                var targetId = ModConfigs.FirstUseFungame;
                fungame = FungameCheck.Fungames.FirstOrDefault(f =>
                    f != null &&
                    (f.Id?.Equals(targetId, StringComparison.OrdinalIgnoreCase) == true ||
                     f.Name?.Equals(targetId, StringComparison.OrdinalIgnoreCase) == true));

                if (fungame != null)
                {
                    Info("start_game_fungame", FungameLocale.GetName(fungame), fungame.Id);
                }
                else
                {
                    Warning("start_game_fungame_not_found", targetId);
                }
            }

            fungame ??= FungameCheck.Fungames.FirstOrDefault();

            if (fungame != null)
            {
                CurrentFungame = fungame;

                if (fungame.MapData != null)
                {
                    SuppressCustomStructuresForMapData();
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

    private static void SuppressCustomStructuresForMapData()
    {
        CustomStructuresLoader.SuppressAutoGeneration();
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void Update()
    {
        var settings = CurrentFungame?.WorldSettings;
        if (settings == null) return;

        if (settings.ForgivingLevel)
        {
            var mapBottom = -WorldGeneration.halfHeight + 10;
            var mapTop = WorldGeneration.halfHeight - 10;

            var main = PlayerCamera.main;
            if (main != null && main.body != null
                             && (main.body.transform.position.y <= mapBottom
                                 || main.transform.position.y <= mapBottom))
            {
                Player.Tp(new Vector2(main.transform.position.x, mapTop));
            }
        }

        Physics2D.gravity = new Vector2(0, settings.Gravity);
        if (settings.FullBright && GameConsole.Instance != null)
            GameConsole.Instance.fullBright = settings.FullBright;

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
        if (CurrentFungame is not { SkipBackground: true }) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentFungame is not { SkipStructures: true }) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.structure"));
        return false;
    }

    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentFungame is not { SkipTerrain: true }) return true;
        MoreLogs("skip_generation", ModLocale.Log("common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld()
    {
        WorldGeneration.loadingText.text = Locale("initializing_world");

        if (ExitTargetScene.HasValue)
        {
            ExitTargetScene = null;
            Info("exited_fungame");
            return;
        }

        var fungame = FungameCheck.CurrentFungame;

        if (fungame == null)
        {
            if (FungameCheck.Fungames.Count > 0)
                Warning("no_fungame_selected");
            else
                Error("no_valid_directories");
            return;
        }

        if (fungame.WorldSettings?.SettingsOverrides is { Count: > 0 } overrides)
        {
            MoreLogs("applying_settings_overrides", $"count={overrides.Count}");
            ApplySettingsOverrides(overrides);
        }

        bool hasMapData = fungame.MapData != null;
        bool hasCustomStructures = !string.IsNullOrEmpty(fungame.CustomStructures);
        bool hasBuildModeSave = !string.IsNullOrEmpty(fungame.BuildModeSave);

        // 支持所有内容类型共存 按顺序执行
        if (hasMapData)
        {
            SpawnMap(fungame);
        }

        if (hasCustomStructures)
        {
            bool hasCustomStructuresMod = Type.GetType(
                "Custom_Structures.Plugin, Custom Structures") != null;
            if (hasCustomStructuresMod)
            {
                CustomStructuresLoader.SpawnCustomStructures(fungame);
            }
            else
            {
                Error("custom_structures_mod_not_loaded", FungameLocale.GetName(fungame));
            }
        }

        if (hasBuildModeSave)
        {
            BuildModeSaveLoader.SpawnBuildModeSave(fungame);
        }

        if (!hasMapData && !hasCustomStructures && !hasBuildModeSave)
        {
            Warning("no_content_type", FungameLocale.GetName(fungame));
        }
    }

    private static void SpawnMap(Fungame fungame)
    {
        var localizedName = FungameLocale.GetName(fungame);
        MoreLogs("loading_fungame_map", localizedName);
        WorldGeneration.loadingText.text = Locale("loading_fungame_map", localizedName);
        MapLoader.LoadAndApplyMapFromFungame(fungame);
        ExecuteCommands(fungame);

        var modInfo = FungameLocale.GetFormattedNameVersion(fungame);
        var authorInfo = FungameLocale.GetFormattedAuthor(fungame);
        var description = FungameLocale.GetDescription(fungame);

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
                GameConsole.RunCommand(command);
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
            GameConsole.RunCommand(command);
        }
    }

    [HarmonyPatch(typeof(Skills), "Awake")]
    private static void SetXp(Skills __instance)
    {
        var xpData = FungameCheck.CurrentFungame.XpData;

        __instance.INT = xpData.IntXp;
        __instance.RES = xpData.ResXp;
        __instance.STR = xpData.StrXp;

        __instance.expINT = xpData.ExpInt;
        __instance.expRES = xpData.ExpRes;
        __instance.expSTR = xpData.ExpStr;

        __instance.minINT = xpData.MinInt;
        __instance.minRES = xpData.MinRes;
        __instance.minSTR = xpData.MinStr;

        __instance.maxINT = xpData.MaxInt;
        __instance.maxRES = xpData.MaxRes;
        __instance.maxSTR = xpData.MaxStr;
    }

    private static void ApplySettingsOverrides(Dictionary<string, object> overrides)
    {
        Settings.EnsureLoaded();
        var allSettings = Settings.GetAllSettings();
        foreach (var kvp in overrides)
        {
            var setting = allSettings.Find(s =>
                string.Equals(s.name, kvp.Key, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                MoreLogs("settings_override_not_found", kvp.Key);
                continue;
            }

            try
            {
                setting.SetValue(kvp.Value);
                setting.Apply();
                MoreLogs("settings_override_applied", kvp.Key, kvp.Value);
            }
            catch (Exception ex)
            {
                Warning("settings_override_failed", $"key={kvp.Key} error={ex.Message}");
            }
        }
    }

    private static void MoreLogs(string key, params object[] args)
    {
        if (ModConfigs.MoreLogs)
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