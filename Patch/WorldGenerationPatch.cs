using System;
using System.Collections;
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
    private static bool _loading;
    internal static int SuccessCount;
    internal static int FailCount;
    internal static int TotalBlocks;

    private static string _generationPhase = "";
    internal static bool _isSpawningMap;
    private static bool _hasShownFungameLoading;

    public static WorldGeneration.OverrideSceneType? ExitTargetScene;

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void Awake(WorldGeneration __instance)
    {
        WorldGeneration = __instance;
        _hasShownFungameLoading = false;

        if (FungameCheck.HasRunningFungame)
        {
            var fungame = FungameCheck.CurrentFungame;

            CurrentFungame = fungame;
            StartFungameLoading(fungame);

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
                StartFungameLoading(fungame);

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

    private static void StartFungameLoading(Fungame fungame)
    {
        _loading = true;
        _isSpawningMap = false;
        SuccessCount = 0;
        FailCount = 0;
        TotalBlocks = 0;
        SetPhase("preparing");
        Info("loading_start", FungameLocale.GetName(fungame));
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

        var settings = CurrentFungame?.WorldSettingsData;
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

    /// <summary>
    /// 强制立即更新加载文本（用于同步操作期间，Update() 不会触发的情况）
    /// </summary>
    internal static void RefreshLoadingText()
    {
        if (WorldGeneration == null || CurrentFungame == null) return;

        var name = FungameLocale.GetName(CurrentFungame);

        string text;
        if (_isSpawningMap && TotalBlocks > 0)
        {
            var total = SuccessCount + FailCount;
            // 安全保护：当 total==0 时强制 pct=0，防止 TotalBlocks 尚未设置时除零导致 NaN→100
            var pct = total > 0
                ? Mathf.Clamp((int)((float)total / TotalBlocks * 100f), 0, 100)
                : 0;
            text = Locale("phase.placing_blocks", name, SuccessCount, FailCount, TotalBlocks, pct);
        }
        else
            text = _generationPhase switch
            {
                "preparing" => Locale("phase.preparing", name),
                "skipping" => Locale("phase.skipping", name, _phaseArg),
                "spawning_map" => Locale("phase.spawning_map", name),
                "spawning_custom_structures" => Locale("phase.spawning_custom_structures", name),
                "spawning_build_mode_save" => Locale("phase.spawning_build_mode_save", name),
                "applying_settings" => Locale("phase.applying_settings", name),
                _ => Locale("phase.generating", name)
            };

        WorldGeneration.loadingText.text = text;
    }

    private static void UpdateLoadingText()
    {
        RefreshLoadingText();
    }

    private static string _phaseArg = "";

    private static void SetPhase(string phaseKey, string arg = "")
    {
        _generationPhase = phaseKey;
        _phaseArg = arg;
    }

    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (CurrentFungame is not { SkipBackground: true }) return true;
        SetPhase("skipping", ModLocale.Log("common.background"));
        MoreLogs("skip_generation", ModLocale.Log("common.background"));
        return false;
    }

    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateStructures()
    {
        if (CurrentFungame is not { SkipStructures: true }) return true;
        SetPhase("skipping", ModLocale.Log("common.structure"));
        MoreLogs("skip_generation", ModLocale.Log("common.structure"));
        return false;
    }

    [HarmonyPatch("SetLoadingText")]
    [HarmonyPrefix]
    public static bool SetLoadingTextPrefix(string localetext)
    {
        return !_loading || CurrentFungame == null;
        // 正在加载Fungame时，阻塞游戏自身的进度文本，防止覆盖我们的实时进度显示
        // UpdateLoadingText 会在 Update 中持续设置正确的文本
    }


    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipWorldGenerateTerrain()
    {
        if (CurrentFungame is not { SkipTerrain: true }) return true;
        SetPhase("skipping", ModLocale.Log("common.terrain"));
        MoreLogs("skip_generation", ModLocale.Log("common.terrain"));
        return false;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPrefix]
    public static bool InitializationWorld(WorldGeneration __instance)
    {
        // 阻塞原始 FinishWorldGeneration 协程，改用我们自己的协程
        // 这样可以在方块放置过程中 yield 让 Unity 渲染进度文本
        __instance.StartCoroutine(ContentLoadingCoroutine(__instance));
        return false;
    }

    private static IEnumerator ContentLoadingCoroutine(WorldGeneration instance)
    {
        if (_hasShownFungameLoading)
        {
            // 通过 StartFungameLoading 启动的，_loading 已经是 true
        }
        else
        {
            _loading = true;
        }

        if (ExitTargetScene.HasValue)
        {
            ExitTargetScene = null;
            _loading = false;
            Info("exited_fungame");
            yield break;
        }

        var fungame = FungameCheck.CurrentFungame;

        if (fungame == null)
        {
            _loading = false;
            if (FungameCheck.Fungames.Count > 0)
                Warning("no_fungame_selected");
            else
                Error("no_valid_directories");
            yield break;
        }

        // 应用设置覆盖
        if (fungame.WorldSettingsData?.SettingsOverrides is { Count: > 0 } overrides)
        {
            SetPhase("applying_settings");
            MoreLogs("applying_settings_overrides", $"count={overrides.Count}");
            ApplySettingsOverrides(overrides);
        }

        var hasMapData = fungame.MapData != null;
        var hasCustomStructures = !string.IsNullOrEmpty(fungame.CustomStructures);
        var hasBuildModeSave = !string.IsNullOrEmpty(fungame.BuildModeSave);

        // 支持所有内容类型共存，按顺序执行
        if (hasMapData)
        {
            // 提前计算 TotalBlocks，确保第一次 RefreshLoadingText 就能显示正确百分比
            var mapData = fungame.MapData;
            if (mapData?.Map is { Length: > 0 })
            {
                TotalBlocks = mapData.Map
                    .Where(row => !string.IsNullOrEmpty(row))
                    .Sum(row => row.Count(c => c != ' '));
            }

            SetPhase("spawning_map");
            _isSpawningMap = true;
            RefreshLoadingText(); // 此时 TotalBlocks 已设置，显示 0%

            // 异步加载地图：每30个方块 yield 一次，让 Unity 渲染进度文本
            yield return MapLoader.LoadAndApplyMapFromFungameAsync(fungame);

            _isSpawningMap = false;

            // 地图加载完成后执行启动命令和提示（仍在加载界面下）
            ExecuteCommands(fungame);

            var modInfo = FungameLocale.GetFormattedNameVersion(fungame);
            var authorInfo = FungameLocale.GetFormattedAuthor(fungame);
            var description = FungameLocale.GetDescription(fungame);

            Player.Alert($"{modInfo}\n{authorInfo}", true);
            Player.Alert(description, false, 6f);
            MapLoader.LogMapInfo();
            Player.Tp(fungame.SpawnPosition);
        }

        if (hasCustomStructures)
        {
            SetPhase("spawning_custom_structures");
            var hasCustomStructuresMod = Type.GetType(
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
            SetPhase("spawning_build_mode_save");
            BuildModeSaveLoader.SpawnBuildModeSave(fungame);
        }

        if (!hasMapData && !hasCustomStructures && !hasBuildModeSave)
        {
            Warning("no_content_type", FungameLocale.GetName(fungame));
        }

        // === 复用原始 FinishWorldGeneration 的清理逻辑 ===
        GlobalDark.main.Darken();
        yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());

        // 应用层修饰（原始逻辑中仅在 biomeOverride == None 时调用）
        instance.ApplyLayerModifiers();

        // 标记世界生成完成（必要：游戏检查此标志判断世界是否可交互）
        instance.generatingWorld = false;

        // 强制刷新全部方块视觉（确保我们放置的方块正确渲染）
        instance.UpdateWorld();

        // 停用所有块、更新可见性
        instance.DisableAllChunks();
        instance.UpdateChunkVisibility();

        // 隐藏加载界面
        instance.loadingObject.SetActive(false);

        _loading = false;
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