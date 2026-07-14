using System;
using System.IO;
using Bark.BetterCCL;
using Bark.Tool;
using CUCoreLib.Registries;
using UnityEngine;

namespace CustomMap.Loader;

public static class CustomStructuresLoader
{
    private const string LocaleKeyPre = "custom_structures_loader.";

    private static readonly string[] StructureExtensions = [".ms2.json", ".m2", ".txt"];
    private static readonly string[] StructureSubdirectories = ["", "layers"];

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

                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                bool success;
                if (extension == ".txt")
                {
                    success = StructureRegistryHelper.RegisterFromFile(structureId, filePath);
                }
                else
                {
                    var text = File.ReadAllText(filePath);
                    success = StructureRegistryHelper.RegisterFromJson(structureId, text);
                }

                if (!success)
                {
                    Error("failed", structureId, extension);
                    continue;
                }

                var position = new Vector2(placement.X, placement.Y);
                StructureRegistry.Place(structureId, position);

                MoreInfo("loading", structureId);
            }
        }
        catch (Exception e)
        {
            Error("failed", MapLocale.GetName(map), e);
        }
    }

    private static string FindStructureFile(string basePath, string structureId)
    {
        foreach (var subDir in StructureSubdirectories)
        foreach (var extension in StructureExtensions)
        {
            var path = string.IsNullOrEmpty(subDir)
                ? Path.Combine(basePath, structureId + extension)
                : Path.Combine(basePath, subDir, structureId + extension);
            if (File.Exists(path)) return path;
        }

        var directPath = Path.Combine(basePath, structureId);
        if (File.Exists(directPath)) return directPath;

        return null;
    }

    private static void MoreInfo(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void Info(string key, params object[] args)
    {
        LogUtil.Info(LocaleLog(key, args), Plugin.Logger);
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