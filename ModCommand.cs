using System;
using System.Globalization;
using BepInEx.Logging;
using HarmonyLib;
using KrokoshaCasualtiesMP;
using MossLib;
using MossLib.Base;
using UnityEngine;
using System.IO;

namespace CustomFungamePack;

[HarmonyPatch(typeof(ConsoleScript))]
public class ModCommand : ModCommandBase
{
    private static ModCommand _instance;
    private static readonly ManualLogSource Logger = Plugin.Logger;
    private static readonly WorldGeneration WorldGeneration = WorldGenerationPatch.WorldGeneration;

    private static ModCommand Instance { get; set; } = new();

    public void Initialize()
    {
        if (_instance != null)
            return;
        _instance = new ModCommand();
        Instance = _instance;
        _instance.Initialize();
    }
    
    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void RegisterCustomCommands()
    { 
        // ConsoleScript.Commands.Add(new Command("tiletest", 
        //         "testhello.description", _ => 
        //         { 
        //             Tools.CheckForWorld();
        //             for (int x = 0; x < 10; x++)
        //             {
        //                 for (int y = 0; y < 10; y++)
        //                 {
        //                     var pos = new Vector2(x,y);
        //                     Info(pos.ToString());
        //                     var blockPos = WorldGeneration.world.WorldToBlockPos(pos);
        //                     WorldGeneration.world.SetBlock(blockPos, 6);
        //                 }
        //             }
        //         }, 
        //         null
        //         )
        // );

        // ConsoleScript.Commands.Add(new Command(
        //     "resetmap", 
        //     "重新生成当前地图", _ =>
        //     {
        //         Tools.CheckForWorld();
        //         Info("正在重新生成地图");
        //
        //         // 设置自动开始标记
        //         PreRunScriptPatch.ShouldAutoStart = true;
        //         
        //         // 返回主菜单，这会触发场景重载，PreRunScript 会重新创建
        //         if (PlayerCameraPatch.PlayerCamera != null)
        //         {
        //             PlayerCameraPatch.PlayerCamera.ToMainMenu();
        //         }
        //     },
        //     null
        //     )
        // );
    }
    
    // private static System.Collections.IEnumerator DelayedStartRun()
    // {
    //     // 等待多帧，确保主菜单场景完全加载
    //     for (int i = 0; i < 10; i++)
    //     {
    //         yield return null;
    //     }
    //     
    //     if (PreRunScriptPatch.PreRunScript != null)
    //     {
    //         PreRunScriptPatch.PreRunScript.StartRun();
    //     }
    //     else
    //     {
    //         Error("PreRunScript 未初始化，可能需要更长的延迟时间");
    //     }
    // }
    
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
