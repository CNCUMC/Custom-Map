using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using CUCoreLib.Helpers;
using CustomMap.Loader;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LocaleKeyPre = "world_generation.";
    public static WorldGeneration WorldGeneration;
    internal static Map CurrentMap;
    private static float _sLoopTimer;
    private static bool _loading;

    private static TextMeshProUGUI _coverText;
    private static GameObject _coverRoot;

    public static WorldGeneration.OverrideSceneType? ExitTargetScene;

    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    public static void AwakePrefix(WorldGeneration __instance)
    {
        WorldGeneration = __instance;

        if (MapCheck.HasRunningMap)
        {
            CurrentMap = MapCheck.CurrentMap;
            return;
        }

        if (!Plugin.StartGameUseMap)
            return;

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
            Plugin.Logger.LogInfo($"[CustomMap.Debug] AwakePrefix: Set CurrentMap to '{map.Name}' (ID: {map.Id}), ExitTargetScene={ExitTargetScene}");
        }
        else
        {
            Plugin.Logger.LogWarning($"[CustomMap.Debug] AwakePrefix: No map found! FirstUseMap='{Plugin.FirstUseMap}', Maps.Count={MapCheck.Maps.Count}");
        }
    }

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void AwakePostfix(WorldGeneration __instance)
    {
        var map = CurrentMap;

        if (map != null)
        {
            StartMapLoading(map);
            SuppressCustomStructuresForMap();

            SetDefaultSceneType(WorldGeneration, map);

            return;
        }

        if (ExitTargetScene.HasValue)
        {
            WorldGeneration.biomeOverride = ExitTargetScene.Value;
            MoreLogs("scene_type_set", ExitTargetScene.Value);
            return;
        }

        if (!Plugin.StartGameUseMap) return;
        Warning("no_valid_directories");
        SetDefaultSceneType(WorldGeneration);
    }

    private static void StartMapLoading(Map map)
    {
        _loading = true;
        Info("loading_start", MapLocale.GetName(map));
    }

    private static void SuppressCustomStructuresForMap()
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

        if (settings.Forgivinglayer)
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
        UpdateCoverText();
    }

    private static void UpdateLoadingText()
    {
        RefreshLoadingText();
    }

    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (CurrentMap is not { SkipBackground: true }) return true;
        MoreLogs("skip_generation", BetterLocale.GetOther("common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentMap is not { SkipStructures: true }) return true;
        MoreLogs("skip_generation", BetterLocale.GetOther("common.structure"));
        return false;
    }

    [HarmonyPatch("SetLoadingText")]
    [HarmonyPrefix]
    public static bool SetLoadingTextPrefix(string localetext)
    {
        return !_loading || CurrentMap == null;
    }

    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentMap is not { SkipTerrain: true }) return true;
        MoreLogs("skip_generation", BetterLocale.GetOther("common.terrain"));
        return false;
    }

    [HarmonyPatch("WorldPlacePlayer")]
    [HarmonyPostfix]
    public static void AfterWorldPlacePlayer()
    {
        if (CurrentMap == null) return;
        var pos = CurrentMap.SpawnPosition;
        if (!PlayerCamera.main || !PlayerCamera.main.body) return;
        PlayerCamera.main.body.transform.position = new Vector3(pos.x, pos.y, 0);
        if (ConsoleScript.instance) ConsoleScript.instance.noClip = true;
        PlayerCamera.main.body.rb.simulated = false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPrefix]
    public static bool InitializationWorld(WorldGeneration __instance)
    {
        if (CurrentMap != null)
        {
            __instance.StartCoroutine(ContentLoadingCoroutine(__instance));
            return false;
        }

        if (ExitTargetScene.HasValue)
        {
            ExitTargetScene = null;
            Info("exited_map");
            return true;
        }

        return true;
    }

    private static IEnumerator ContentLoadingCoroutine(WorldGeneration instance)
    {
        var map = CurrentMap;
        CreateLoadingCover();
        instance.loadingObject.SetActive(false);
        _loading = true;

        if (map.WorldSettingsData?.SettingsOverrides is { Count: > 0 } overrides)
        {
            MoreLogs("applying_settings_overrides", $"count={overrides.Count}");
            ApplySettingsOverrides(overrides);
        }

        var hasCustomStructures = map.Structures is { Count: > 0 };
        var hasBuildModeSave = !string.IsNullOrEmpty(map.BuildModeSave);
        var hasItems = map.Items is { Count: > 0 };

        WorldGeneration.world.generatingWorld = false;

        if (hasCustomStructures)
        {
            CustomStructuresLoader.SpawnCustomStructures(map);
        }

        if (hasBuildModeSave) BuildModeSaveLoader.SpawnBuildModeSave(map);

        if (!hasCustomStructures && !hasBuildModeSave && !hasItems)
            Warning("no_content_type", MapLocale.GetName(map));

        GlobalDark.main.Darken();
        yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());

        instance.ApplyLayerModifiers();

        instance.generatingWorld = false;

        instance.UpdateWorld();

        instance.DisableAllChunks();
        instance.UpdateChunkVisibility();

        var safePos = map.SpawnPosition;
        if (PlayerCamera.main && PlayerCamera.main.body)
        {
            var body = PlayerCamera.main.body;
            body.transform.position = new Vector3(safePos.x, safePos.y, 0);
        }

        yield return null;
        if (PlayerCamera.main && PlayerCamera.main.body)
        {
            PlayerCamera.main.body.rb.simulated = true;
            if (ConsoleScript.instance) ConsoleScript.instance.noClip = false;
        }

        MapLoader.LogMapInfo();
        ExecuteCommands(map);
        MapLoader.PickItems(map);

        _loading = false;
        yield return null;
        _coverRoot.SetActive(false);
    }

    private static void UpdateCoverText()
    {
        if (!_coverText || CurrentMap == null) return;
        _coverText.text = $"{CurrentMap.Name}\nLoading...";
    }

    private static void CreateLoadingCover()
    {
        _coverRoot = new GameObject("CustomMapLoadingCover", typeof(RectTransform));
        var canvas = _coverRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        var bg = _coverRoot.AddComponent<Image>();
        bg.color = Color.black;
        var rect = _coverRoot.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var textGo = new GameObject("ProgressText", typeof(RectTransform));
        textGo.transform.SetParent(_coverRoot.transform, false);
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.3f);
        textRect.anchorMax = new Vector2(0.9f, 0.7f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        _coverText = textGo.AddComponent<TextMeshProUGUI>();
        _coverText.font = TextUtil.TMPUnifont;
        _coverText.alignment = TextAlignmentOptions.Center;
        _coverText.fontSize = 36;
        _coverText.color = Color.white;
        _coverText.text = "Loading...";
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

    private static void Info(string key, params object[] args) =>
        LogUtil.Info(LocaleLog(key, args), Plugin.Logger);

    private static void Warning(string key, params object[] args) =>
        LogUtil.Warning(LocaleLog(key, args), Plugin.Logger);

    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
}

