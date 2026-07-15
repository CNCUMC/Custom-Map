using System;
using System.IO;
using Bark.BetterCCL;
using Bark.Tool;

namespace CustomMap.Loader;

public static class BuildModeSaveLoader
{
    private const string LocaleKeyPre = "build_mode_save_loader.";

    public static void SpawnBuildModeSave(Map map)
    {
        if (map == null || string.IsNullOrEmpty(map.CurrentLayer.BuildModeSave)) return;
        if (!Plugin.BuildModeLoaded) return;

        try
        {
            var saveFilePath = Path.Combine(
                map.DirectoryPath,
                $"{map.CurrentLayer.BuildModeSave}.alexx_BMsave");

            if (!File.Exists(saveFilePath))
            {
                Error("not_found_buildmode_save");
                return;
            }

            MapLoader.ApplyBuildModeSave(saveFilePath);
        }
        catch (Exception e)
        {
            Error("failed", map, e);
        }
    }

    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Plugin.Logger);
    }

    private static string LocaleLog(string key, params object[] args)
    {
        return BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
    }
}