using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using CUCoreLib.Helpers;
using CustomMap.Data.Feature.World;
using CustomMap.Loader;
using HarmonyLib;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(WorldGeneration))]
public static class WorldGenerationPatch
{
    private const string LocaleKeyPre = "world_generation_patch";
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

        if (MapUtils.HasMap && !ExitTargetScene.HasValue)
        {
            CurrentMap = MapUtils.CurrentMap;
            return;
        }

        if (!Plugin.StartGameUseMap)
            return;

        if (string.IsNullOrEmpty(Plugin.FirstUseMap)) return;
        var targetId = Plugin.FirstUseMap;
        var map = MapUtils.Maps.FirstOrDefault(f =>
            f != null
            && (f.Id?.Equals(targetId, StringComparison.OrdinalIgnoreCase) == true ||
                MapLocale.GetName(f)?.Equals(targetId, StringComparison.OrdinalIgnoreCase) == true));

        if (map != null)
        {
            Info("start_game_map", MapLocale.GetName(map), map.Id);
            CurrentMap = map;
        }
        else
        {
            Warning("start_game_map_not_found", targetId);
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

            // Initialize fluid array early to prevent NullReferenceException in FluidManager
            if (FluidManager.main == null) return;
            const uint chunkWidth = 16u;
            const uint chunkHeight = 16u;
            __instance.width = chunkWidth * (uint)WorldGeneration.CHUNKSIZE;
            __instance.height = chunkHeight * (uint)WorldGeneration.CHUNKSIZE;
            FluidManager.main.fluid = new byte[(int)__instance.width, (int)__instance.height];

            return;
        }

        if (ExitTargetScene.HasValue)
        {
            WorldGeneration.biomeOverride = ExitTargetScene.Value;
            MoreLogs("type_set", ExitTargetScene.Value);
            return;
        }

        if (!Plugin.StartGameUseMap) return;
        Warning("no_valid_directories");
        SetDefaultSceneType(WorldGeneration);
    }

    private static void ApplyWorldSettingsToRunSettings()
    {
        var map = CurrentMap;
        if (map?.CurrentLayer?.WorldSettingsData == null) return;

        var settings = map.CurrentLayer.WorldSettingsData;

        var properties = typeof(WorldSettingsData).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var jsonAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();
            var jsonName = jsonAttr?.PropertyName ?? prop.Name;

            if (!WorldSettingsData.RunSettingsKeyMap.ContainsKey(jsonName)) continue;

            var runSettingsKey = WorldSettingsData.GetRunSettingsKey(jsonName);
            var value = prop.GetValue(settings);
            if (value == null) continue;
            WorldGeneration.runSettings[runSettingsKey] = value;
        }

        if (settings.SettingsOverrides == null) return;
        foreach (var kvp in settings.SettingsOverrides) WorldGeneration.runSettings[kvp.Key] = kvp.Value;
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

        var settings = CurrentMap?.CurrentLayer.WorldSettingsData;
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
        __instance.biomeOverride = map?.CurrentLayer.Type ?? WorldGeneration.OverrideSceneType.Debug;
        MoreLogs("type_set", __instance.biomeOverride);
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
        if (CurrentMap is not { CurrentLayer.SkipBackground: true }) return true;
        MoreLogs("skip_generation", LocaleLog("background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentMap is not { CurrentLayer.SkipStructures: true }) return true;
        MoreLogs("skip_generation", LocaleLog("structure"));
        return false;
    }

    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentMap is not { CurrentLayer.SkipTerrain: true }) return true;
        MoreLogs("skip_generation", LocaleLog("terrain"));
        return false;
    }

    [HarmonyPatch("SetLoadingText")]
    [HarmonyPrefix]
    public static bool SetLoadingTextPrefix(string localetext)
    {
        return !_loading || CurrentMap == null;
    }

    [HarmonyPatch("WorldPlacePlayer")]
    [HarmonyPostfix]
    public static void AfterWorldPlacePlayer()
    {
        if (CurrentMap == null) return;
        var pos = CurrentMap.CurrentLayer.SpawnPosition;
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
            ApplyWorldSettingsToRunSettings();
            __instance.StartCoroutine(ContentLoadingCoroutine(__instance));
            return false;
        }

        if (!ExitTargetScene.HasValue) return true;
        ExitTargetScene = null;
        Info("exited_map");

        return true;
    }

    private static IEnumerator ContentLoadingCoroutine(WorldGeneration instance)
    {
        var map = CurrentMap;
        CreateLoadingCover();
        instance.loadingObject.SetActive(false);
        _loading = true;

        var hasCustomStructures = map.CurrentLayer.Structures is { Count: > 0 };
        var hasBuildModeSave = !string.IsNullOrEmpty(map.CurrentLayer.BuildModeSave);

        WorldGeneration.world.generatingWorld = false;

        if (hasCustomStructures) CustomStructuresLoader.SpawnCustomStructures(map);

        if (hasBuildModeSave)
            LogUtil.Warning("Coming soon :)", Plugin.Logger);
        // BuildModeSaveLoader.SpawnBuildModeSave(map);

        GlobalDark.main.Darken();
        yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());

        instance.ApplyLayerModifiers();

        instance.generatingWorld = false;

        instance.UpdateWorld();

        instance.DisableAllChunks();
        instance.UpdateChunkVisibility();

        var safePos = map.CurrentLayer.SpawnPosition;
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
        _coverText.text = $"{MapLocale.GetName(CurrentMap)}\nLoading...";
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
        _coverText.font = TextUtil.UnifontTMP;
        _coverText.alignment = TextAlignmentOptions.Center;
        _coverText.fontSize = 36;
        _coverText.color = Color.white;
        _coverText.text = "Loading...";
    }

    private static void ExecuteCommands(Map map)
    {
        var commands = map.CurrentLayer.CommandData;
        if (commands?.OnceCommands == null
            || commands.OnceCommands.Count == 0
           )
            return;

        if (commands.OnceCommands != null)
            foreach (var command in commands.OnceCommands)
            {
                MoreLogs("executing_command", LocaleLog("startup_command"), command);
                CUCoreUtils.ConsoleRunCommand(ConsoleScript.instance, command);
            }

        _sLoopTimer = 0f;
    }

    private static void HandleLoopCommands()
    {
        var loopCommands = CurrentMap?.CurrentLayer.CommandData?.LoopCommands;
        if (loopCommands == null || loopCommands.Count == 0) return;

        var interval = CurrentMap.CurrentLayer.CommandData.LoopInterval > 0
            ? CurrentMap.CurrentLayer.CommandData.LoopInterval
            : 10f;

        _sLoopTimer += Time.deltaTime;

        if (!(_sLoopTimer >= interval)) return;
        _sLoopTimer = 0f;

        foreach (var command in loopCommands)
        {
            MoreLogs("executing_command", LocaleLog("loop_command"), command);
            CUCoreUtils.ConsoleRunCommand(ConsoleScript.instance, command);
        }
    }

    private static void MoreLogs(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void Info(string key, params object[] args)
    {
        LogUtil.Info(LocaleLog(key, args), Plugin.Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        LogUtil.Warning(LocaleLog(key, args), Plugin.Logger);
    }

    private static string LocaleLog(string key, params object[] args)
    {
        return BetterLocale.GetLog($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }
}