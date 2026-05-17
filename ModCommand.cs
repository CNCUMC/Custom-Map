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
    private static readonly Fungame Fungame = WorldGenerationPatch.CurrentFungame;

    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void RegisterCustomCommands(ConsoleScript __instance)
    {
        void Action(string[] args)
        {
            World.CheckForWorld();
            CheckArg(args, 1);
            switch (args[1])
            {
                case "reload":
                    CheckArg(args, 1);
                    LogConsole("reload");
                    MapLoader.ReloadMap(Fungame);
                    break;
                case "info":
                    CheckArg(args, 1);
                    MapLoader.LogMapInfo();
                    break;
                case "spawn":
                    CheckArg(args, 1);
                    Spawn();
                    break;
                case "select":
                    CheckArg(args, 2);
                    Select(args[2]);
                    break;
                case "list":
                    CheckArg(args, 1);
                    ListFungames();
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
                    "spawn",
                    "select",
                    "list"
                ]
            }
        };
        (string, string)[] valueTupleArray =
        [
            ("string", Locale("string")),
            ("string", Locale("parameter"))
        ];
        ConsoleScript.Commands.Add(new Command(
            "fungame",
            Locale("description"),
            Action,
            argAutofill2,
            valueTupleArray)
        );
    }

    private static void ListFungames()
    {
        var fungames = FungameCheck.Fungames;

        if (fungames == null || fungames.Count == 0)
        {
            Command("list.empty");
            return;
        }

        Command("list.header", fungames.Count);

        for (int i = 0; i < fungames.Count; i++)
        {
            var fungame = fungames[i];
            var isCurrent = fungame.Id == FungameCheck.CurrentFungame?.Id;
            var marker = isCurrent ? "->" : "  ";

            Command("list.item", marker, i + 1, fungame.Name, fungame.Id, fungame.Version, fungame.Authors);
        }

        Log.NewLine();
    }

    private static void Select(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Command("select.no_key");
            return;
        }

        var fungame = FungameCheck.Fungames.Find(f =>
            f.Id.Equals(key, System.StringComparison.OrdinalIgnoreCase) ||
            f.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));

        if (fungame == null)
        {
            Command("select.not_found", key);
            return;
        }

        WorldGenerationPatch.CurrentFungame = fungame;
        
        Command("select.success", fungame.Name, fungame.Id);
        MapLoader.ReloadMap(fungame);
    }


    private static void CheckArg(string[] args, int index)
    {
        Tools.CheckArgumentCount(args, index);
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

    private static void Command(string key, params object[] args)
    {
        Log.Info(Locale(key, args), Logger);
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