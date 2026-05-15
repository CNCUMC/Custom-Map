using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using MossLib;
using System.IO;
using UnityEngine;

namespace CustomFungamePack;

[HarmonyPatch(typeof(WorldGeneration))]
public class WorldGenerationPatch
{
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

                if (fungame != null)
                {
                    CurrentFungame = fungame;

                    ApplyFeaturesEarly(fungame.Feature);

                    if (fungame.Map != null)
                    {
                        WorldGeneration.biomeOverride = fungame.Map.Type;
                        Info($"设置场景类型为: {fungame.Map.Type}");
                    }
                    else
                    {
                        SetDefaultSceneType(WorldGeneration);
                    }
                }
            }
            else
            {
                Error($"找不到 fungame.json 文件: {fungameFilePath}");
                SetDefaultSceneType(WorldGeneration);
            }
        }
        else
        {
            Error("没有有效的Fungame目录，请检查 Fungames 文件夹");
            SetDefaultSceneType(WorldGeneration);
        }
    }

    private static void ApplyFeaturesEarly(Dictionary<string, float> features)
    {
        if (features == null || features.Count == 0)
        {
            Info("未启用任何特性");
            return;
        }

        foreach (var feature in features)
        {
            switch (feature.Key.ToLower())
            {
                case "fullbright":
                    Tools.RunCommand("fullbright");
                    break;
                case "forgivingLevel":
                    Info("已启用宽容关卡模式");
                    break;
                case "gravity":
                    Info($"已启用重力调整: {feature.Value}");
                    break;
                default:
                    Warning($"未知的特性: {feature.Key}");
                    break;
            }
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void ForgivingLevel()
    {
        if (IsFeatureEnabled("forgiving_level") && CurrentFungame?.Map != null)
        {
            var mapBottom = -WorldGeneration.halfHeight + 10;
            var mapTop = WorldGeneration.halfHeight - 10;

            if (PlayerCamera.main.body.transform.position.y <= mapBottom
                || PlayerCamera.main.transform.position.y <= mapBottom)
            {
                Tools.Tp(new Vector2(PlayerCamera.main.transform.position.x, mapTop));
            }
        }

        if (IsFeatureEnabled("gravity"))
        {
            var gravityScale = GetFeatureValue("gravity", FeatureConfig.GetDefault("gravity"));
            Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);
        }
    }

    private static bool IsFeatureEnabled(string featureName)
    {
        return CurrentFungame?.Feature != null && CurrentFungame.Feature.ContainsKey(featureName);
    }

    private static float GetFeatureValue(string featureName, float defaultValue = 1.0f)
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

    // 跳过地形生成
    [HarmonyPatch("WorldGenerateTerrain")]
    [HarmonyPrefix]
    public static bool SkipTerrainGeneration()
    {
        if (CurrentFungame?.Map?.SkipTerrain != false)
        {
            Info("已跳过地形生成");
            return false;
        }

        return true;
    }

    // 跳过结构生成
    [HarmonyPatch("WorldGenerateStructures")]
    [HarmonyPrefix]
    public static bool SkipStructureGeneration()
    {
        if (CurrentFungame?.Map?.SkipStructures != false)
        {
            Info("已跳过结构生成");
            return false;
        }

        return true;
    }

    // 跳过背景生成
    [HarmonyPatch("WorldCreateBackground")]
    [HarmonyPrefix]
    public static bool SkipWorldCreateBackground()
    {
        if (CurrentFungame?.Map?.SkipBackground != false)
        {
            Info("已跳过背景生成");
            return false;
        }

        return true;
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld()
    {
        WorldGeneration.loadingText.text = "初始化Fungame地图...";

        if (FungameCheck.ValidDirectories.Count > 0)
        {
            var firstFungameDir = FungameCheck.ValidDirectories[0];
            var fungameFilePath = Path.Combine(firstFungameDir, "fungame.json");

            if (File.Exists(fungameFilePath))
            {
                var jsonContent = File.ReadAllText(fungameFilePath);
                var fungame = Newtonsoft.Json.JsonConvert.DeserializeObject<Fungame>(jsonContent);

                if (fungame?.Map != null)
                {
                    Info($"正在加载Fungame地图: {fungame.Name}");
                    WorldGeneration.loadingText.text = $"正在加载Fungame地图: {fungame.Name}";
                    MapLoader.LoadAndApplyMapFromFungame(fungame);
                    ExecuteStartupCommands(fungame.Command);

                    string authors = fungame.Author != null && fungame.Author.Count > 0
                        ? string.Join(", ", fungame.Author)
                        : "Unknown";
                    Tools.LogCla($"{fungame.Name} v{fungame.Version}\nby {authors}", Logger, true);
                    Tools.LogCla($"{fungame.Description}", Logger, false, 6f);
                    Tools.SetBlock(0, 0, 6);
                }
                else
                {
                    Warning($"Fungame {fungame?.Name ?? "未知"} 不包含地图数据");
                }
            }
            else
            {
                Error($"找不到 fungame.json 文件: {fungameFilePath}");
            }
        }
        else
        {
            Error("没有有效的Fungame目录，请检查 Fungames 文件夹");
        }
    }

    private static void ExecuteStartupCommands(List<string> commands)
    {
        if (commands == null || commands.Count == 0)
        {
            Info("未启用任何启动命令");
            return;
        }

        foreach (var command in commands)
        {
            Info($"执行启动命令: {command}");
            Tools.RunCommand(command);
        }
    }

    private static void Info(string text)
    {
        Tools.LogInfo(text, Logger);
    }

    private static void Error(string text)
    {
        Tools.LogError(text, Logger);
    }

    private static void Warning(string text)
    {
        Tools.LogWarning(text, Logger);
    }
}