using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using MossLib.Tool;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CustomFungamePack.Loader;

[BepInDependency(CustomStructuresGuid, BepInDependency.DependencyFlags.SoftDependency)]
public static class CustomStructuresLoader
{
    private const string LocaleKeyPre = "custom_structures_loader.";
    private static readonly ManualLogSource Logger = Plugin.Logger;
    private const string CustomStructuresGuid = "com.Jimmyking.morestructures";

    /// <summary>
    /// 当使用 MapData 类型 Fungame 时，抑制 Custom Structures 模组的自动生成。
    /// 该模组有自己的 Harmony 补丁，会在世界生成时自动放置已注册的结构。
    /// 通过清除其结构注册表来防止其自动生成干扰 MapData 的地图块。
    /// </summary>
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

            if (definitionsField != null)
            {
                if (definitionsField.GetValue(null) is not IDictionary { Count: > 0 } dict) return;
                dict.Clear();
                Info("suppress.cleared_definitions");
                return;
            }

            foreach (var field in structureLoaderType.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                                BindingFlags.Static))
            {
                if (!field.FieldType.Name.Contains("Dictionary") && !field.FieldType.Name.Contains("List") &&
                    !field.FieldType.Name.Contains("IEnumerable")) continue;
                var value = field.GetValue(null);
                switch (value)
                {
                    case IDictionary { Count: > 0 } dict2:
                        dict2.Clear();
                        Info("suppress.cleared_field", field.Name);
                        return;
                    case IList { Count: > 0 } list:
                        list.Clear();
                        Info("suppress.cleared_field", field.Name);
                        return;
                }
            }

            Info("suppress.no_registry");
        }
        catch (Exception ex)
        {
            Warning("suppress.failed", ex.Message);
        }
    }

    private static readonly Regex V1NameRegex =
        new("StructureDefinitions\\[\"(.*?)\"\\]", RegexOptions.Compiled);

    public static void SpawnCustomStructures(Fungame fungame)
    {
        if (fungame == null || string.IsNullOrEmpty(fungame.CustomStructures))
        {
            return;
        }

        try
        {
            if (!Chainloader.PluginInfos.TryGetValue(CustomStructuresGuid, out PluginInfo targetPlugin))
            {
                Error("not_found", "Custom Structures mod");
                return;
            }

            Assembly customStructuresAssembly = targetPlugin.Instance.GetType().Assembly;

            string filePath = Path.Combine(fungame.DirectoryPath, fungame.CustomStructures);
            if (!File.Exists(filePath))
            {
                Error("not_found_custom_structures");
                return;
            }

            string text = File.ReadAllText(filePath);
            string fileName = Path.GetFileName(filePath);
            bool isV2 = filePath.EndsWith(".ms2.json", StringComparison.OrdinalIgnoreCase);

            Type structureLoaderType = customStructuresAssembly.GetType("Custom_Structures.StructureLoader");
            if (structureLoaderType == null)
            {
                Error("not_found", "StructureLoader");
                return;
            }

            string parseMethodName = isV2 ? "ParseAndRegisterV2" : "ParseAndRegister";
            MethodInfo parseMethod = structureLoaderType.GetMethod(
                parseMethodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            if (parseMethod == null)
            {
                Error("not_found", parseMethodName);
                return;
            }

            parseMethod.Invoke(null, [text, fileName]);

            string structName = ExtractStructureName(text, fileName, isV2);
            if (string.IsNullOrEmpty(structName))
            {
                Error("not_found", "structure name");
                return;
            }

            Type worldGenPatcherType =
                customStructuresAssembly.GetType("Custom_Structures.WorldGenerationPatcher");
            if (worldGenPatcherType == null)
            {
                Error("not_found", "WorldGenerationPatcher");
                return;
            }

            MethodInfo generateMethod = worldGenPatcherType.GetMethod(
                "GenerateStructure",
                BindingFlags.Public | BindingFlags.Static);

            if (generateMethod == null)
            {
                Error("not_found", "GenerateStructure");
                return;
            }

            Vector2 position = fungame.MapPosition;
            generateMethod.Invoke(null, [structName, position]);

            MoreInfo("loading", fungame.CustomStructures);
        }
        catch (Exception e)
        {
            Error("failed", fungame, e);
        }
    }

    private static string ExtractStructureName(string text, string fileName, bool isV2)
    {
        if (isV2)
        {
            try
            {
                JObject json = JObject.Parse(text);
                string name = json["metadata"]?["name"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(name))
                    return name.Trim();
            }
            catch
            {
                // fall through to fallback
            }

            string nameFromFile = Path.GetFileNameWithoutExtension(fileName);
            if (nameFromFile.EndsWith(".ms2", StringComparison.OrdinalIgnoreCase))
                nameFromFile = Path.GetFileNameWithoutExtension(nameFromFile);
            return nameFromFile;
        }

        Match match = V1NameRegex.Match(text);
        if (match.Success)
            return match.Groups[1].Value;

        return Path.GetFileNameWithoutExtension(fileName);
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