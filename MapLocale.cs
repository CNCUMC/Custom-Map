using System;
using System.IO;
using Bark.BetterCCL;
using Bark.Tool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CustomMap;

public static class MapLocale
{
    public static string GetName(Map map)
    {
        if (map == null) return string.Empty;
        return ReadFromMapLang(map, "name") ?? map.Id ?? string.Empty;
    }

    public static string GetNameForLang(Map map, string lang)
    {
        if (map == null) return string.Empty;
        return ReadFromMapLangForLang(map, "name", lang) ?? map.Id ?? string.Empty;
    }

    public static string GetDisplayName(Map map)
    {
        if (map == null) return string.Empty;
        var currentLang = PlayerPrefs.GetString("locale", "EN");

        // 先尝试当前语言
        var name = ReadFromMapLangForLang(map, "name", currentLang);
        if (!string.IsNullOrEmpty(name)) return name;

        // 回退到 EN
        if (currentLang == "EN") return map.Id ?? string.Empty;
        name = ReadFromMapLangForLang(map, "name", "EN");
        if (!string.IsNullOrEmpty(name)) return name;

        // 最后回退到 ID
        return map.Id ?? string.Empty;
    }

    public static string GetDescriptionForLang(Map map, string lang)
    {
        if (map == null) return string.Empty;
        return ReadFromMapLangForLang(map, "description", lang) ?? string.Empty;
    }

    public static string GetDisplayDescription(Map map)
    {
        if (map == null) return string.Empty;
        var currentLang = PlayerPrefs.GetString("locale", "EN");

        // 先尝试当前语言
        var desc = ReadFromMapLangForLang(map, "description", currentLang);
        if (!string.IsNullOrEmpty(desc)) return desc;

        // 回退到 EN
        if (currentLang == "EN") return string.Empty;
        desc = ReadFromMapLangForLang(map, "description", "EN");
        if (!string.IsNullOrEmpty(desc)) return desc;

        return string.Empty;
    }

    public static string GetAuthor(Map map)
    {
        return map?.Authors ?? string.Empty;
    }

    public static string GetFormatted(string formatKey, params object[] args)
    {
        return BetterLocale.GetOther($"format.{formatKey}", args);
    }

    public static string GetFormattedNameVersion(Map map)
    {
        return map == null
            ? string.Empty
            : $"{GetName(map)} v{map.Version ?? "1.0.0"}";
    }

    public static string GetFormattedAuthor(Map map)
    {
        return map == null
            ? string.Empty
            : GetFormatted("author", GetAuthor(map));
    }

    public static string GetFormattedInfo(Map map)
    {
        return map == null
            ? string.Empty
            : $"{GetFormattedNameVersion(map)}\n{GetFormattedAuthor(map)}";
    }

    public static string GetFormattedFeatures(Map map)
    {
        return map == null
            ? string.Empty
            : GetFormatted("features", map.CurrentLayer.ActiveFeatures);
    }

    public static void SaveToCurrentLang(Map map, string directoryPath = null)
    {
        var saveDir = directoryPath ?? map?.DirectoryPath;
        if (map == null || string.IsNullOrEmpty(saveDir))
        {
            LogUtil.Info(
                $"[MapLocale.Debug] SaveToCurrentLang skipped: map={(map == null
                    ? "null"
                    : $"DirectoryPath={map.DirectoryPath}")}, directoryPath={directoryPath}", Plugin.Logger);
            return;
        }

        var name = GetDisplayName(map);
        var desc = GetDisplayDescription(map);
        LogUtil.Info(
            $"[MapLocale.Debug] SaveToCurrentLang start: saveDir={saveDir}, Name={name}, Desc={desc}", Plugin.Logger);

        try
        {
            var currentLang = PlayerPrefs.GetString("locale", "EN");
            var langDir = Path.Combine(saveDir, "lang");
            LogUtil.Info($"[MapLocale.Debug] Creating lang dir: {langDir}", Plugin.Logger);
            Directory.CreateDirectory(langDir);
            var langFile = Path.Combine(langDir, $"{currentLang}.json");
            LogUtil.Info($"[MapLocale.Debug] Lang file path: {langFile}", Plugin.Logger);

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

            // 修复：将 name 和 description 合并到 langJson
            langJson["name"] ??= name;
            langJson["description"] ??= desc;

            var jsonContent = JsonConvert.SerializeObject(langJson, Formatting.Indented);
            File.WriteAllText(langFile, jsonContent + Environment.NewLine);
        }
        catch (Exception ex)
        {
            LogUtil.Warning($"[MapLocale] Failed to save locale: {ex.Message}", Plugin.Logger);
        }
    }

    private static string ReadFromMapLang(Map map, string key)
    {
        if (map == null || string.IsNullOrEmpty(map.DirectoryPath))
            return null;

        var currentLang = PlayerPrefs.GetString("locale", "EN");
        return ReadFromMapLangForLang(map, key, currentLang);
    }

    private static string ReadFromMapLangForLang(Map map, string key, string lang)
    {
        try
        {
            if (string.IsNullOrEmpty(map.DirectoryPath))
                return null;

            var langFile = Path.Combine(map.DirectoryPath, "lang", $"{lang}.json");

            if (!File.Exists(langFile))
                return null;

            var json = JObject.Parse(File.ReadAllText(langFile));
            return json[key]?.ToString();
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (JsonException ex)
        {
            LogUtil.Warning($"[MapLocale] Failed to parse lang file for {lang}: {ex.Message}", Plugin.Logger);
            return null;
        }
        catch (Exception ex)
        {
            LogUtil.Warning($"[MapLocale] Unexpected error reading lang file for {lang}: {ex.Message}", Plugin.Logger);
            return null;
        }
    }
}