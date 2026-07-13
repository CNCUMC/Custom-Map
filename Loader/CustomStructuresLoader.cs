using System;
using System.IO;
using System.Text.RegularExpressions;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx;
using CUCoreLib.Registries;
using CustomMap.Data.Feature.World;
using Newtonsoft.Json.Linq;

namespace CustomMap.Loader;

public static class CustomStructuresLoader
{
    private const string LocaleKeyPre = "custom_structures_loader.";

    private static readonly Regex StructureFileRegex = new(@"\.ms2\.json$", RegexOptions.IgnoreCase);

    public static void SuppressAutoGeneration()
    {
        StructureRegistryHelper.Clear();
    }

    public static void SpawnCustomStructures(Map map)
    {
        if (map?.Structures == null || map.Structures.Count == 0) return;

        try
        {
            var basePath = map.DirectoryPath;
            if (string.IsNullOrEmpty(basePath))
            {
                Error("not_found_custom_structures");
                return;
            }

            foreach (var placement in map.Structures)
            {
                if (placement == null || string.IsNullOrWhiteSpace(placement.Structure)) continue;

                var structureId = placement.Structure.Trim();
                var filePath = FindStructureFile(basePath, structureId);

                if (filePath == null)
                {
                    Error("not_found_custom_structures");
                    continue;
                }

                var text = File.ReadAllText(filePath);

                // 注册结构�?CUCoreLib
                if (!StructureRegistryHelper.RegisterFromJson(structureId, text))
                {
                    Error("failed", structureId, "注册失败");
                    continue;
                }

                // 放置结构到指定坐�?
                var position = new UnityEngine.Vector2(placement.X, placement.Y);
                StructureRegistry.Place(structureId, position);

                MoreInfo("loading", structureId);
            }
        }
        catch (Exception e)
        {
            Error("failed", map.Name, e);
        }
    }

    private static string FindStructureFile(string basePath, string structureId)
    {
        // 直接尝试 .ms2.json
        var path = Path.Combine(basePath, structureId + ".ms2.json");
        if (File.Exists(path)) return path;

        // 尝试不带扩展名（如果用户已经包含了扩展名�?
        path = Path.Combine(basePath, structureId);
        if (File.Exists(path)) return path;

        // 尝试旧版扩展名（兼容性）
        path = Path.Combine(basePath, structureId + ".ms.json");
        if (File.Exists(path)) return path;

        return null;
    }

    private static void MoreInfo(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void Info(string key, params object[] args) =>
        LogUtil.Info(LocaleLog(key, args), Plugin.Logger);


    private static void Error(string key, params object[] args) =>
        LogUtil.Error(LocaleLog(key, args), Plugin.Logger);

    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
}
