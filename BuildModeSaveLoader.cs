using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using MossLib.Tool;

namespace CustomFungamePack;

[BepInDependency("com.alexx_.buildmode", BepInDependency.DependencyFlags.SoftDependency)]
public static class BuildModeSaveLoader
{
    private const string LocaleKeyPre = "build_mode_save_loader.";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    private static readonly MethodInfo LoadFileAndStartPlacementMethod =
        typeof(BuildModeMod).GetMethod(
            "LoadFileAndStartPlacement",
            BindingFlags.NonPublic | BindingFlags.Static);

    public static void SpawnBuildModeSave(Fungame fungame)
    {
        if (fungame is {
                MapData: not null,
                CustomStructures: not null,
                BuildModeSave: null
            })
        {
            ModLocale.Log("map_loader.load_error");
            return;
        }

        try
        {
            if (LoadFileAndStartPlacementMethod == null)
            {
                Error("buildmode_method_not_found");
                return;
            }

            if (string.IsNullOrEmpty(fungame.BuildModeSave))
            {
                Error("buildmode_save_name_empty");
                return;
            }

            string assemblyDir = fungame.DirectoryPath;
            if (string.IsNullOrEmpty(assemblyDir))
            {
                Error("assembly_dir_not_found");
                return;
            }

            string saveFilePath = Path.Combine(
                assemblyDir,
                $"{fungame.BuildModeSave}.alexx_BMsave");

            LoadFileAndStartPlacementMethod.Invoke(
                null,
                [saveFilePath]);
            
            MoreInfo("loading", fungame.BuildModeSave);
        }
        catch (Exception e)
        {
            Error("failed", fungame, e);
        }
    }
    
    private static void MoreInfo(string key, params object[] args)
    {
        if (Configs.MoreLogs)
            Info(key, args);
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
}