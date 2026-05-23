using System;
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
    private const string LocaleKeyPre = "mod_command.";

    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void RegisterCustomCommands(ConsoleScript __instance)
    {
        try
        {
            void Action(string[] args)
            {
                CheckArg(args, 1);
                switch (args[1])
                {
                    case "reload":
                        if (CheckWorld()) return;
                        CheckArg(args, 1);
                        MapLoader.ReloadMap(FungameCheck.CurrentFungame);
                        break;
                    case "info":
                        CheckArg(args, 1);
                        MapLoader.LogMapInfo();
                        break;
                    case "spawn":
                        if (CheckWorld()) return;
                        CheckArg(args, 1);
                        Spawn();
                        break;
                    case "select":
                        CheckArg(args, 2);
                        Select(args[2]);
                        break;
                    case "list":
                        CheckArg(args, 1);
                        if (args.Length > 2)
                        {
                            Select(args[2]);
                        }
                        else
                        {
                            MapLoader.LogFungameList();
                        }

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
                ("string", Command("fungame.string")),
                ("string", Command("fungame.parameter"))
            ];
            ConsoleScript.Commands.Add(new Command(
                "fungame",
                Command("fungame.description"),
                Action,
                argAutofill2,
                valueTupleArray)
            );
            ConsoleScript.Commands.Add(new Command(
                "fg",
                Command("fungame.description"),
                Action,
                argAutofill2,
                valueTupleArray)
            );
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to register custom commands: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static bool CheckWorld()
    {
        if (HasWorldLoaded()) return false;
        Error("world_not_loaded");
        return true;
    }

    private static bool HasWorldLoaded()
    {
        try
        {
            return WorldGeneration.world != null && !WorldGeneration.world.generatingWorld;
        }
        catch
        {
            return false;
        }
    }

    private static void Select(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Command("fungame.select.no_key");
            return;
        }

        if (FungameCheck.Fungames == null || FungameCheck.Fungames.Count == 0)
        {
            Command("fungame.list.empty");
            return;
        }

        Fungame fungame;

        if (int.TryParse(key, out int index))
        {
            if (index < 1 || index > FungameCheck.Fungames.Count)
            {
                Command("fungame.select.invalid_index", index, FungameCheck.Fungames.Count);
                return;
            }

            fungame = FungameCheck.Fungames[index - 1];
        }
        else
        {
            fungame = FungameCheck.Fungames.Find(f =>
                f != null &&
                (f.Id?.Equals(key, StringComparison.OrdinalIgnoreCase) == true ||
                 f.Name?.Equals(key, StringComparison.OrdinalIgnoreCase) == true));
        }

        if (fungame == null)
        {
            Command("fungame.select.not_found", key);
            return;
        }

        WorldGenerationPatch.CurrentFungame = fungame;

        Command("fungame.select.success", fungame.Name, fungame.Id);

        if (HasWorldLoaded())
        {
            MapLoader.ReloadMap(fungame);
        }
        else
        {
            Command("select.without_world", fungame.Name);
        }
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
        return ModLocale.GetFormat(key, args);
    }

    private static string Command(string key, params object[] args)
    {
        return Locale($"command.{key}", args);
    }

    private static void LogConsole(string key, params object[] args)
    {
        var message = Locale(key, args);
        Log.Info(message, Logger);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Info(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Error(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Warning(message, Logger);
    }
}