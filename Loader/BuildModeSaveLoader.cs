using System;
using System.IO;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx;
using BepInEx.Logging;

namespace CustomMap.Loader;

[BepInDependency(BuildModeGuid, BepInDependency.DependencyFlags.SoftDependency)]
public static class BuildModeSaveLoader
{
    private const string LocaleKeyPre = "build_mode_save_loader.";
    private const string BuildModeGuid = "com.alexx_.buildmode";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    public static void SpawnBuildModeSave(Map map)
    {
        if (map == null || string.IsNullOrEmpty(map.BuildModeSave)) return;

        try
        {
            var saveFilePath = Path.Combine(
                map.DirectoryPath,
                $"{map.BuildModeSave}.alexx_BMsave");

            if (!File.Exists(saveFilePath))
            {
                Error("not_found_buildmode_save");
                return;
            }

            var anchor = map.MapPosition;
            var anchorX = (int)anchor.x;
            var anchorY = (int)anchor.y;

            MapLoader.ApplyBuildModeSave(saveFilePath, anchorX, anchorY);
        }
        catch (Exception e)
        {
            Error("failed", map, e);
        }
    }

    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Logger);
    }

    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
}