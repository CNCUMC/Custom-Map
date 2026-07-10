using System;
using System.IO;
using Bark.BetterCCL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CustomMap;

public static class MapLocale
{
    private const string MapType = "map";

    public static string GetName(Map map)
    {
        if (map == null) return string.Empty;
        var localized = ReadFromMapLang(map, "name");
        return localized ?? map.Name ?? string.Empty;
    }

    public static string GetDescription(Map map)
    {
        if (map == null) return string.Empty;
        var localized = ReadFromMapLang(map, "description");
        return localized ?? map.Description ?? string.Empty;
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
            : GetFormatted("features", map.ActiveFeatures);
    }

    public static void SaveToCurrentLang(Map map, string directoryPath = null)
    {
        var saveDir = directoryPath ?? map?.DirectoryPath;
        if (map == null || string.IsNullOrEmpty(saveDir))
        {
            Plugin.Logger?.LogInfo(
                $"[MapLocale.Debug] SaveToCurrentLang skipped: map={(map == null ? "null" : $"DirectoryPath={map.DirectoryPath}")}, directoryPath={directoryPath}");
            return;
        }

        Plugin.Logger?.LogInfo(
            $"[MapLocale.Debug] SaveToCurrentLang start: saveDir={saveDir}, Name={map.Name}, Desc={map.Description}");

        try
        {
            var currentLang = PlayerPrefs.GetString("locale", "EN");
            var langDir = Path.Combine(saveDir, "lang");
            Plugin.Logger?.LogInfo($"[MapLocale.Debug] Creating lang dir: {langDir}");
            Directory.CreateDirectory(langDir);
            var langFile = Path.Combine(langDir, $"{currentLang}.json");
            Plugin.Logger?.LogInfo($"[MapLocale.Debug] Lang file path: {langFile}");

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

            // 获取或创�?map 对象
            JObject mapObj;
            if (langJson[MapType] is JObject existing)
            {
                mapObj = existing;
            }
            else
            {
                mapObj = new JObject();
                langJson[MapType] = mapObj;
            }

            if (!string.IsNullOrEmpty(map.Name))
                mapObj["name"] = map.Name;
            if (!string.IsNullOrEmpty(map.Description))
                mapObj["description"] = map.Description;

            var jsonContent = JsonConvert.SerializeObject(langJson, Formatting.Indented);
            File.WriteAllText(langFile, jsonContent + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Plugin.Logger?.LogWarning($"[MapLocale] Failed to save locale: {ex.Message}");
        }
    }

    private static string ReadFromMapLang(Map map, string key)
    {
        try
        {
            if (string.IsNullOrEmpty(map.DirectoryPath))
                return null;

            var currentLang = PlayerPrefs.GetString("locale", "EN");
            var langFile = Path.Combine(map.DirectoryPath, "lang", $"{currentLang}.json");

            if (!File.Exists(langFile))
                return null;

            var json = JObject.Parse(File.ReadAllText(langFile));
            var mapObj = json[MapType] as JObject;
            return mapObj?[key]?.ToString();
        }
        catch
        {
            return null;
        }
    }
}