using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx.Logging;
using CUCoreLib.Helpers;
using CustomMap.Loader;
using HarmonyLib;
using UnityEngine;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LocaleKeyPre = "world_generation.";
    private static readonly ManualLogSource Logger = Plugin.Logger;
    public static WorldGeneration WorldGeneration;
    internal static Map CurrentMap;
    private static float _sLoopTimer;
    private static bool _loading;
    internal static int SuccessCount;
    internal static int FailCount;
    internal static int TotalBlocks;

    private static string _generationPhase = "";
    internal static bool _isSpawningMap;
    private static bool _hasShownMapLoading;

    public static WorldGeneration.OverrideSceneType? ExitTargetScene;

    private static string _phaseArg = "";

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void Awake(WorldGeneration __instance)
    {
        WorldGeneration = __instance;
        _hasShownMapLoading = false;

        if (MapCheck.HasRunningMap)
        {
            var map = MapCheck.CurrentMap;

            CurrentMap = map;
            StartMapLoading(map);

            if (map.MapData != null)
            {
                SuppressCustomStructuresForMapData();
                WorldGeneration.biomeOverride = map.Type;
                MoreLogs("scene_type_set", map.Type);
            }
            else
            {
                SetDefaultSceneType(WorldGeneration, map);
            }
        }
        else if (ExitTargetScene.HasValue)
        {
            WorldGeneration.biomeOverride = ExitTargetScene.Value;
            MoreLogs("scene_type_set", ExitTargetScene.Value);
        }
        else if (Plugin.StartGameUseMap)
        {
            Map map = null;

            if (!string.IsNullOrEmpty(Plugin.FirstUseMap))
            {
                var targetId = Plugin.FirstUseMap;
                map = MapCheck.Maps.FirstOrDefault(f =>
                    f != null &&
                    (f.Id?.Equals(targetId, StringComparison.OrdinalIgnoreCase) == true ||
                     f.Name?.Equals(targetId, StringComparison.OrdinalIgnoreCase) == true));

                if (map != null)
                    Info("start_game_map", MapLocale.GetName(map), map.Id);
                else
                    Warning("start_game_map_not_found", targetId);
            }

            map ??= MapCheck.Maps.FirstOrDefault();

            if (map != null)
            {
                CurrentMap = map;
                StartMapLoading(map);

                if (map.MapData != null)
                {
                    SuppressCustomStructuresForMapData();
                    WorldGeneration.biomeOverride = map.Type;
                    MoreLogs("scene_type_set", map.Type);
                }
                else
                {
                    SetDefaultSceneType(WorldGeneration, map);
                }
            }
            else
            {
                LogUtil.Warning("No valid Map directories, please check the Maps folder", Logger);
                SetDefaultSceneType(WorldGeneration);
            }
        }
    }

    private static void StartMapLoading(Map map)
    {
        _loading = true;
        _isSpawningMap = false;
        SuccessCount = 0;
        FailCount = 0;
        TotalBlocks = 0;
        SetPhase("preparing");
        Info("loading_start", MapLocale.GetName(map));
    }

    private static void SetPhase(string phaseKey)
    {
        _generationPhase = phaseKey;
    }

    private static void SuppressCustomStructuresForMapData()
    {
        CustomStructuresLoader.SuppressAutoGeneration();
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void Update()
    {
        if (_loading)
            UpdateLoadingText();

        var settings = CurrentMap?.WorldSettingsData;
        if (settings == null) return;

        if (settings.ForgivingLevel)
        {
            var mapBottom = -WorldGeneration.halfHeight + 10;
            var mapTop = WorldGeneration.halfHeight - 10;

            var main = PlayerCamera.main;
            if (main != null && main.body != null
                             && (main.body.transform.position.y <= mapBottom
                                 || main.transform.position.y <= mapBottom))
                PlayerUtil.Tp(new Vector2(main.transform.position.x, mapTop));
        }

        Physics2D.gravity = new Vector2(0, settings.Gravity);
        if (settings.FullBright && ConsoleScript.instance != null)
            ConsoleScript.instance.fullBright = settings.FullBright;

        HandleLoopCommands();
    }

    private static void SetDefaultSceneType(WorldGeneration __instance, Map map = null)
    {
        __instance.biomeOverride = map?.Type ?? WorldGeneration.OverrideSceneType.Debug;
        MoreLogs("scene_type_set", __instance.biomeOverride);
    }

    internal static void RefreshLoadingText()
    {
        if (!WorldGeneration || CurrentMap == null) return;

        var name = MapLocale.GetName(CurrentMap);

        string text;
        if (_isSpawningMap && TotalBlocks > 0)
        {
            var total = SuccessCount + FailCount;
            var pct = total > 0
                ? Mathf.Clamp((int)((float)total / TotalBlocks * 100f), 0, 100)
                : 0;
            text = LocaleLog("phase.placing_blocks", name, SuccessCount, FailCount, TotalBlocks, pct);
        }
        else
        {
            text = _generationPhase switch
            {
                "preparing" => LocaleLog("phase.preparing", name),
                "skipping" => LocaleLog("phase.skipping", name, _phaseArg),
                "spawning_map" => LocaleLog("phase.spawning_map", name),
                "spawning_custom_structures" => LocaleLog("phase.spawning_custom_structures", name),
                "spawning_build_mode_save" => LocaleLog("phase.spawning_build_mode_save", name),
                "applying_settings" => LocaleLog("phase.applying_settings", name),
                _ => LocaleLog("phase.generating", name)
            };
        }

        WorldGeneration.loadingText.text = text;
    }

    private static void UpdateLoadingText()
    {
        RefreshLoadingText();
    }

    private static void SetPhase(string phaseKey, string arg)
    {
        _generationPhase = phaseKey;
        _phaseArg = arg;
    }

    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (CurrentMap is not { SkipBackground: true }) return true;
        SetPhase("skipping", BetterLocale.GetLog("common.background"));
        MoreLogs("skip_generation", BetterLocale.GetOther("common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentMap is not { SkipStructures: true }) return true;
        SetPhase("skipping", BetterLocale.GetLog("common.structure"));
        MoreLogs("skip_generation", BetterLocale.GetOther("common.structure"));
        return false;
    }

    [HarmonyPatch("SetLoadingText")]
    [HarmonyPrefix]
    public static bool SetLoadingTextPrefix(string localetext)
    {
        return !_loading || CurrentMap == null;
        // ���ڼ���Mapʱ��������Ϸ����Ľ����ı�����ֹ�������ǵ�ʵʱ������ʾ
        // UpdateLoadingText ���� Update �г���������ȷ���ı�
    }


    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentMap is not { SkipTerrain: true }) return true;
        SetPhase("skipping", BetterLocale.GetOther("common.terrain"));
        MoreLogs("skip_generation", BetterLocale.GetOther("common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPrefix]
    public static bool InitializationWorld(WorldGeneration __instance)
    {
        // ����ԭʼ FinishWorldGeneration Э�̣����������Լ���Э��
        // ���������ڷ�����ù����� yield Unity ��Ⱦ�����ı�
        __instance.StartCoroutine(ContentLoadingCoroutine(__instance));
        return false;
    }

    private static IEnumerator ContentLoadingCoroutine(WorldGeneration instance)
    {
        if (_hasShownMapLoading)
        {
            // ͨ�� StartMapLoading �����ģ�_loading �Ѿ� true
        }
        else
        {
            _loading = true;
        }

        if (ExitTargetScene.HasValue)
        {
            ExitTargetScene = null;
            _loading = false;
            Info("exited_map");
            yield break;
        }

        var map = MapCheck.CurrentMap;

        if (map == null)
        {
            _loading = false;
            if (MapCheck.Maps.Count > 0)
                Warning("no_map_selected");
            else
                Error("no_valid_directories");
            yield break;
        }

        // Ӧ�����ø���
        if (map.WorldSettingsData?.SettingsOverrides is { Count: > 0 } overrides)
        {
            SetPhase("applying_settings");
            MoreLogs("applying_settings_overrides", $"count={overrides.Count}");
            ApplySettingsOverrides(overrides);
        }

        var hasMapData = map.MapData != null;
        var hasCustomStructures = !string.IsNullOrEmpty(map.CustomStructures);
        var hasBuildModeSave = !string.IsNullOrEmpty(map.BuildModeSave);

        if (hasMapData)
        {
            var mapData = map.MapData;
            if (mapData?.Map is { Length: > 0 })
                TotalBlocks = mapData.Map
                    .Where(row => !string.IsNullOrEmpty(row))
                    .Sum(row => row.Count(c => c != ' '));

            SetPhase("spawning_map");
            _isSpawningMap = true;
            RefreshLoadingText();

            SetWorldExists();
            WorldGeneration.world.generatingWorld = false;
            yield return MapLoader.LoadAndApplyMapFromMapAsync(map);

            _isSpawningMap = false;

            ExecuteCommands(map);

            var modInfo = MapLocale.GetFormattedNameVersion(map);
            var authorInfo = MapLocale.GetFormattedAuthor(map);
            var description = MapLocale.GetDescription(map);

            PlayerUtil.Alert($"{modInfo}\n{authorInfo}", true);
            PlayerUtil.Alert(description, false, 6f);
            MapLoader.LogMapInfo();
            PlayerUtil.Tp(map.SpawnPosition);
        }

        if (hasCustomStructures)
        {
            SetPhase("spawning_custom_structures");
            var hasCustomStructuresMod = Type.GetType(
                "Custom_Structures.Plugin, Custom Structures") != null;
            if (hasCustomStructuresMod)
                CustomStructuresLoader.SpawnCustomStructures(map);
            else
                Error("custom_structures_mod_not_loaded", MapLocale.GetName(map));
        }

        if (hasBuildModeSave)
        {
            SetPhase("spawning_build_mode_save");
            BuildModeSaveLoader.SpawnBuildModeSave(map);
        }

        if (!hasMapData && !hasCustomStructures && !hasBuildModeSave)
            Warning("no_content_type", MapLocale.GetName(map));

        GlobalDark.main.Darken();
        yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());

        instance.ApplyLayerModifiers();

        instance.generatingWorld = false;

        instance.UpdateWorld();

        instance.DisableAllChunks();
        instance.UpdateChunkVisibility();

        yield return new WaitForSeconds(9f);
        instance.loadingObject.SetActive(false);

        _loading = false;
    }

    private static void SetWorldExists()
    {
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        var field = typeof(WorldGeneration).GetField("worldExists", flags)
                    ?? typeof(WorldGeneration).GetField("<worldExists>k__BackingField", flags);
        field?.SetValue(WorldGeneration.world, true);
    }

    private static void ExecuteCommands(Map map)
    {
        var commands = map.CommandData;
        if (commands == null || ((commands.OnceCommands == null || commands.OnceCommands.Count == 0) &&
                                 (commands.LoopCommands == null || commands.LoopCommands.Count == 0)))
        {
            MoreLogs("no_commands", BetterLocale.GetLog("common.startup_command"));
            return;
        }

        if (commands.OnceCommands != null)
            foreach (var command in commands.OnceCommands)
            {
                MoreLogs("executing_command", BetterLocale.GetLog("common.startup_command"), command);
                CUCoreUtils.ConsoleRunCommand(ConsoleScript.instance, command);
            }

        _sLoopTimer = 0f;
    }

    private static void HandleLoopCommands()
    {
        var loopCommands = CurrentMap?.CommandData?.LoopCommands;
        if (loopCommands == null || loopCommands.Count == 0) return;

        var interval = CurrentMap.CommandData.LoopInterval > 0
            ? CurrentMap.CommandData.LoopInterval
            : 10f;

        _sLoopTimer += Time.deltaTime;

        if (!(_sLoopTimer >= interval)) return;
        _sLoopTimer = 0f;

        foreach (var command in loopCommands)
        {
            MoreLogs("executing_loop_command", BetterLocale.GetLog("common.loop_command"), command);
            CUCoreUtils.ConsoleRunCommand(ConsoleScript.instance, command);
        }
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
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void Error(string key, params object[] args) =>
        LogUtil.Error(BetterLocale.GetLog($"error.{key}", args), Logger);
    private static void Info(string key, params object[] args) =>
        LogUtil.Info(LocaleLog(key, args), Logger);
    private static void Warning(string key, params object[] args) =>
        LogUtil.Warning(LocaleLog(key, args), Logger);
    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
}