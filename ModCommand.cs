using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using MossLib.Base;
using MossLib.Tool;

namespace CustomFungamePack;

[HarmonyPatch(typeof(ConsoleScript))]
public class ModCommand : ModCommandBase
{
    private new static readonly ManualLogSource Logger = Plugin.Logger;
    private const string LocaleKeyPre = "log.modcommand.";

    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void RegisterCustomCommands(ConsoleScript __instance)
    {
        void Action(string[] args)
        {
            World.CheckForWorld();
            Tools.CheckArgumentCount(args, 1);
            switch (args[1])
            {
                case "reload":
                    LogConsole("reload");
                    MapLoader.ReloadMap();
                    break;
                case "info":
                    MapLoader.LogMapInfo();
                    break;
                case "spawn":
                    Spawn();
                    break;
                default:
                    Warning("empty_type");
                    break;
            }
        }

        Dictionary<int, List<string>> argAutofill2 = new Dictionary<int, List<string>>
        {
            {
                0,
                [
                    "reload",
                    "info",
                    "spawn"
                ]
            }
        };
        (string, string)[] valueTupleArray =
        [
            ("string", Locale("string"))
        ];
        ConsoleScript.Commands.Add(new Command(
            "fungame",
            Locale("description"),
            Action,
            argAutofill2,
            valueTupleArray)
        );
    }

    private static void Spawn()
    {
        var fungame = FungameCheck.CurrentFungame;
        LogConsole("spawn", fungame.SpawnPosition);
        Player.Tp(fungame.SpawnPosition);
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.GetFormat($"command.fungame.{key}", args);
    }
    
    private static void LogConsole(string key, params object[] args)
    {
        var message = Locale(key, args);
        Log.Info(message, Logger);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
        Log.Info(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
        Log.Warning(message, Logger);
    }
}