using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using MossLib.Tool;
using UnityEngine;

namespace CustomFungamePack.Loader;

[BepInDependency(BuildModeGuid, BepInDependency.DependencyFlags.SoftDependency)]
public static class BuildModeSaveLoader
{
    private const string LocaleKeyPre = "build_mode_save_loader.";
    private static readonly ManualLogSource Logger = Plugin.Logger;
    private const string BuildModeGuid = "com.alexx_.buildmode";

    public static void SpawnBuildModeSave(Fungame fungame)
    {
        if (fungame == null || string.IsNullOrEmpty(fungame.BuildModeSave))
        {
            return;
        }

        try
        {
            var saveFilePath = Path.Combine(
                fungame.DirectoryPath,
                $"{fungame.BuildModeSave}.alexx_BMsave");

            if (!File.Exists(saveFilePath))
            {
                Error("not_found_buildmode_save");
                return;
            }

            var anchor = fungame.MapPosition;
            var anchorX = (int)anchor.x;
            var anchorY = (int)anchor.y;

            MapLoader.ApplyBuildModeSave(saveFilePath, anchorX, anchorY);
        }
        catch (Exception e)
        {
            Error("failed", fungame, e);
        }
    }

    private static void MoreInfo(string key, params object[] args)
    {
        if (ModConfigs.MoreLogs)
            Info(key, args);
    }

    private static void Info(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Info(message, Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Warning(message, Logger);
    }

    private static void Error(string key, params object[] args)
    {
        var message = ModLocale.Log($"{LocaleKeyPre}{key}", args);
        Log.Error(message, Logger);
    }
}