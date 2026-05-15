// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using BepInEx.Logging;
// using HarmonyLib;
// using MossLib;
// using MossLib.Base;
// using UnityEngine;
//
// namespace CustomFungamePack;
//
// [HarmonyPatch(typeof(ConsoleScript))]
// public class ModCommand : ModCommandBase
// {
//     private const string LocalePre = "command.mosslib.";
//     private static ManualLogSource _logger;
//
//     private static ModCommand Instance { get; set; } = new ModCommand();
//
//     public static void Initialize(ManualLogSource logger)
//     {
//         if (Instance != null)
//             return;
//         Instance = new ModCommand();
//         _logger = logger;
//         Instance.Initialize(logger, Assembly.GetExecutingAssembly());
//     }
//
//     [HarmonyPatch("RegisterAllCommands")]
//     public class ConsoleScriptRegisterAllCommandsPatcher
//     {
//         // [HarmonyPostfix]
//         // public static void RegisterCustomCommands(ConsoleScript __instance)
//         // {
//         //     ConsoleScript.Commands.Add(new Command("testhello", ModLocale.GetFormat("command.mosslib.testhello.description"), (Command.Action) (_ => Tools.LogCla(ModLocale.GetFormat("command.mosslib.testhello.text", (object) "command.mosslib."), ModCommand._logger, (bool) (UnityEngine.Object) __instance)), (Dictionary<int, List<string>>) null, Array.Empty<(string, string)>()));
//         // }
//     }
//
//     [HarmonyPatch("Awake")]
//     public new class ConsoleScriptAwakePatcher
//     {
//         [HarmonyPostfix]
//         public static void AddCustomLogCallback()
//         {
//             Application.logMessageReceived += new Application.LogCallback(((ModCommandBase) ModCommand.Instance).ApplicationLogCallback);
//         }
//     }
// }

