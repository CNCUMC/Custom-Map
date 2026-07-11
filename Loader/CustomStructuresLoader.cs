using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Newtonsoft.Json.Linq;

namespace CustomMap.Loader;

[BepInDependency(CustomStructuresGuid, BepInDependency.DependencyFlags.SoftDependency)]
public static class CustomStructuresLoader
{
    private const string LocaleKeyPre = "custom_structures_loader.";
    private const string CustomStructuresGuid = "com.Jimmyking.morestructures";
    private static readonly ManualLogSource Logger = Plugin.Logger;

    private static readonly Regex V1NameRegex =
        new("StructureDefinitions\\[\"(.*?)\"\\]", RegexOptions.Compiled);

    public static void SuppressAutoGeneration()
    {
        try
        {
            if (!Chainloader.PluginInfos.TryGetValue(CustomStructuresGuid, out var targetPlugin))
                return;

            var assembly = targetPlugin.Instance.GetType().Assembly;
            var structureLoaderType = assembly.GetType("Custom_Structures.StructureLoader");
            if (structureLoaderType == null)
            {
                Warning("suppress.structure_loader_not_found");
                return;
            }

            var definitionsField = structureLoaderType.GetField("StructureDefinitions",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (definitionsField == null)
            {
                Info("suppress.no_registry");
                return;
            }

            if (definitionsField.GetValue(null) is not IDictionary { Count: > 0 } dict) return;
            dict.Clear();
            Info("suppress.cleared_definitions");
        }
        catch (Exception ex)
        {
            Warning("suppress.failed", ex.Message);
        }
    }

    public static void SpawnCustomStructures(Map map)
    {
        if (map == null || string.IsNullOrEmpty(map.CustomStructures)) return;

        try
        {
            if (!Chainloader.PluginInfos.TryGetValue(CustomStructuresGuid, out var targetPlugin))
            {
                Error("not_found", "Custom Structures mod");
                return;
            }

            var customStructuresAssembly = targetPlugin.Instance.GetType().Assembly;

            var basePath = Path.Combine(map.DirectoryPath, map.CustomStructures);
            var filePath = basePath;
            if (!File.Exists(filePath))
            {
                filePath = basePath + ".txt";
                if (!File.Exists(filePath))
                {
                    filePath = basePath + ".ms.json";
                    if (!File.Exists(filePath))
                    {
                        filePath = basePath + ".ms2.json";
                        if (!File.Exists(filePath))
                        {
                            Error("not_found_custom_structures");
                            return;
                        }
                    }
                }
            }

            var text = File.ReadAllText(filePath);
            var fileName = Path.GetFileName(filePath);
            var isV2 = !text.TrimStart().StartsWith("StructureDefinitions");

            var structureLoaderType = customStructuresAssembly.GetType("Custom_Structures.StructureLoader");
            if (structureLoaderType == null)
            {
                Error("not_found", "StructureLoader");
                return;
            }

            var parseMethodName = isV2
                ? "ParseAndRegisterV2"
                : "ParseAndRegister";
            var parseMethod = structureLoaderType.GetMethod(
                parseMethodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            if (parseMethod == null)
            {
                Error("not_found", parseMethodName);
                return;
            }

            parseMethod.Invoke(null, [text, fileName]);

            var structName = ExtractStructureName(text, fileName, isV2);
            if (string.IsNullOrEmpty(structName))
            {
                Error("not_found", "structure name");
                return;
            }

            var worldGenPatcherType = customStructuresAssembly.GetType("Custom_Structures.WorldGenerationPatcher");
            if (worldGenPatcherType == null)
            {
                Error("not_found", "WorldGenerationPatcher");
                return;
            }

            var generateMethod = worldGenPatcherType.GetMethod(
                "GenerateStructure",
                BindingFlags.Public | BindingFlags.Static);

            if (generateMethod == null)
            {
                Error("not_found", "GenerateStructure");
                return;
            }

            var position = map.MapPosition;
            generateMethod.Invoke(null, [structName, position]);

            MoreInfo("loading", map.CustomStructures);
        }
        catch (Exception e)
        {
            Error("failed", map, e);
        }
    }

    private static string ExtractStructureName(string text, string fileName, bool isV2)
    {
        if (isV2)
        {
            try
            {
                var json = JObject.Parse(text);
                var name = json["metadata"]?["name"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(name))
                    return name.Trim();
            }
            catch
            {
                // fall through to fallback
            }

            var nameFromFile = Path.GetFileNameWithoutExtension(fileName);
            if (nameFromFile.EndsWith(".ms2", StringComparison.OrdinalIgnoreCase))
                nameFromFile = Path.GetFileNameWithoutExtension(nameFromFile);
            return nameFromFile;
        }

        var match = V1NameRegex.Match(text);
        return match.Success
            ? match.Groups[1].Value
            : Path.GetFileNameWithoutExtension(fileName);
    }

    private static void MoreInfo(string key, params object[] args)
    {
        if (Plugin.MoreLogs)
            Info(key, args);
    }

    private static void Info(string key, params object[] args)
    {
        LogUtil.Info(LocaleLog(key, args), Logger);
    }

    private static void Warning(string key, params object[] args)
    {
        LogUtil.Warning(LocaleLog(key, args), Logger);
    }

    private static void Error(string key, params object[] args)
    {
        LogUtil.Error(LocaleLog(key, args), Logger);
    }

    private static string LocaleLog(string key, params object[] args) =>
        BetterLocale.GetLog($"{LocaleKeyPre}{key}", args);
}