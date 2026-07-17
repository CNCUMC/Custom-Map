using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;

namespace CustomMap.Loader;

public static class BuildModeSaveLoader
{
    private const string LocaleKeyPre = "build_mode_save_loader";

    public static void SpawnBuildModeSave(Map map)
    {
        LogUtil.Warning("Coming soon :)", Plugin.Logger);
        // if (map == null || string.IsNullOrEmpty(map.CurrentLayer.BuildModeSave)) return;
        // if (!Plugin.BuildModeLoaded) return;
        //
        // try
        // {
        //     ApplyBuildModeSave(map);
        // }
        // catch (Exception e)
        // {
        //     Error("failed", map, e);
        // }
    }

    public static void ApplyBuildModeSave(Map map)
    {
        // var saveFilePath = Path.Combine(
        //     map.DirectoryPath,
        //     $"{map.CurrentLayer.BuildModeSave}.alexx_BMsave");
        //
        // if (!File.Exists(saveFilePath))
        // {
        //     Error("not_found_buildmode_save");
        //     return;
        // }
        //
        // try
        // {
        //     var loadModeType = AppDomain.CurrentDomain.GetAssemblies()
        //         .Select(a => a.GetType("LoadMode"))
        //         .FirstOrDefault(t => t != null);
        //
        //     if (loadModeType == null)
        //     {
        //         Error("not_found", "Build Mode mod");
        //         return;
        //     }
        //
        //     var assembly = loadModeType.Assembly;
        //     var buildModeModType = assembly.GetType("BuildModeMod");
        //     if (buildModeModType == null)
        //     {
        //         Error("not_found", "BuildModeMod");
        //         return;
        //     }
        //
        //     loadModeType.GetMethod("LoadFileAndStartPlacement",
        //             BindingFlags.Public | BindingFlags.Static)
        //         ?.Invoke(null, [saveFilePath]);
        //
        //     var applyLoadMethod = loadModeType.GetMethod("ApplyLoad",
        //         BindingFlags.NonPublic | BindingFlags.Static);
        //     if (applyLoadMethod == null)
        //     {
        //         Error("not_found", "LoadMode.ApplyLoad");
        //         return;
        //     }
        //
        //     applyLoadMethod.Invoke(null, null);
        //
        //     buildModeModType.GetField("_loadBuffer", BindingFlags.Public | BindingFlags.Static)
        //         ?.SetValue(null, null);
        //     buildModeModType.GetField("_isLoading", BindingFlags.Public | BindingFlags.Static)
        //         ?.SetValue(null, false);
        // }
        // catch (Exception ex)
        // {
        //     Error("failed", map.Id, ex.Message);
        // }
    }


    private static void Warning(string key, params object[] args)
    {
        LogUtil.Warning(LocaleLog(key, args), Plugin.Logger);
    }
    
    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Plugin.Logger);
    }

    private static string LocaleLog(string key, params object[] args)
    {
        return BetterLocale.GetLog($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }
}