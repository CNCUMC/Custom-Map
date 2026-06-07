using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CustomFungamePack;

public static class FungameLocale
{
    private const string FungameType = "fungame";

    public static string GetName(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        var localized = ReadFromFungameLang(fungame, "name");
        return localized ?? fungame.Name ?? string.Empty;
    }

    public static string GetDescription(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        var localized = ReadFromFungameLang(fungame, "description");
        return localized ?? fungame.Description ?? string.Empty;
    }

    public static string GetAuthor(Fungame fungame)
    {
        return fungame?.Authors ?? string.Empty;
    }

    public static string GetFormatted(string formatKey, params object[] args)
    {
        return ModLocale.GetFormat($"format.{formatKey}", args);
    }

    public static string GetFormattedNameVersion(Fungame fungame)
    {
        return fungame == null 
            ? string.Empty 
            : $"{GetName(fungame)} v{fungame.Version ?? "1.0.0"}";
    }

    public static string GetFormattedAuthor(Fungame fungame)
    {
        return fungame == null 
            ? string.Empty : GetFormatted("author", GetAuthor(fungame));
    }

    public static string GetFormattedInfo(Fungame fungame)
    {
        return fungame == null
            ? string.Empty
            : $"{GetFormattedNameVersion(fungame)}\n{GetFormattedAuthor(fungame)}";
    }

    public static string GetFormattedFeatures(Fungame fungame)
    {
        return fungame == null
            ? string.Empty 
            : GetFormatted("features", fungame.ActiveFeatures);
    }

    public static void SaveToCurrentLang(Fungame fungame, string directoryPath = null)
    {
        var saveDir = directoryPath ?? fungame?.DirectoryPath;
        if (fungame == null || string.IsNullOrEmpty(saveDir))
        {
            Plugin.Logger?.LogInfo($"[FungameLocale.Debug] SaveToCurrentLang skipped: fungame={(fungame == null ? "null" : $"DirectoryPath={fungame.DirectoryPath}")}, directoryPath={directoryPath}");
            return;
        }

        Plugin.Logger?.LogInfo($"[FungameLocale.Debug] SaveToCurrentLang start: saveDir={saveDir}, Name={fungame.Name}, Desc={fungame.Description}");

        try
        {
            var currentLang = PlayerPrefs.GetString("locale", "EN");
            var langDir = Path.Combine(saveDir, "lang");
            Plugin.Logger?.LogInfo($"[FungameLocale.Debug] Creating lang dir: {langDir}");
            Directory.CreateDirectory(langDir);
            var langFile = Path.Combine(langDir, $"{currentLang}.json");
            Plugin.Logger?.LogInfo($"[FungameLocale.Debug] Lang file path: {langFile}");

            JObject langJson;
            if (File.Exists(langFile))
            {
                var content = File.ReadAllText(langFile);
                langJson = !string.IsNullOrEmpty(content)
                    ? JObject.Parse(content)
                    : new JObject();
            }
            else
            {
                langJson = new JObject();
            }

            // 获取或创建 fungame 对象
            JObject fungameObj;
            if (langJson[FungameType] is JObject existing)
                fungameObj = existing;
            else
            {
                fungameObj = new JObject();
                langJson[FungameType] = fungameObj;
            }

            if (!string.IsNullOrEmpty(fungame.Name))
                fungameObj["name"] = fungame.Name;
            if (!string.IsNullOrEmpty(fungame.Description))
                fungameObj["description"] = fungame.Description;

            var jsonContent = JsonConvert.SerializeObject(langJson, Formatting.Indented);
            File.WriteAllText(langFile, jsonContent + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Plugin.Logger?.LogWarning($"[FungameLocale] Failed to save locale: {ex.Message}");
        }
    }

    private static string ReadFromFungameLang(Fungame fungame, string key)
    {
        try
        {
            if (string.IsNullOrEmpty(fungame.DirectoryPath))
                return null;

            var currentLang = PlayerPrefs.GetString("locale", "EN");
            var langFile = Path.Combine(fungame.DirectoryPath, "lang", $"{currentLang}.json");

            if (!File.Exists(langFile))
                return null;

            var json = JObject.Parse(File.ReadAllText(langFile));
            var fungameObj = json[FungameType] as JObject;
            return fungameObj?[key]?.ToString();
        }
        catch
        {
            return null;
        }
    }
}
