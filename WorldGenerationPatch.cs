using BepInEx.Logging;
using HarmonyLib;
using MossLib;
using System.IO;

namespace CustomFungamePack;

[HarmonyPatch(typeof(WorldGeneration))]
public class WorldGenerationPatch
{
    private static readonly ManualLogSource Logger = Plugin.Logger;
    internal static WorldGeneration WorldGeneration;

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void ForceEnableTutorial(WorldGeneration __instance)
    {
        WorldGeneration = __instance;
        __instance.biomeOverride = WorldGeneration.OverrideSceneType.Tutorial;
        Info("已强制修改为教程世界");
    }

    [HarmonyPatch("FinishWorldGeneration")]
    [HarmonyPostfix]
    public static void InitializationWorld(WorldGeneration __instance)
    {
        __instance.loadingText.text = "初始化Fungame地图...";

        const int startX = -76;
        const int endX = 74;
        const int startY = -120;
        const int endY = 58;
            
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tools.SetBlock(x, y, 0);
            }
        }

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
                    __instance.loadingText.text = $"正在加载Fungame地图: {fungame.Name}";
                    MapLoader.LoadAndApplyMapFromFungame(fungame);
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