using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CustomFungamePack;

/// <summary>
/// 提供 Fungame 名称、描述等文本的本地化支持。
/// 优先读取对应 Fungame 的 <c>{DirectoryPath}/lang/{currentLang}.json</c> 文件中的 <c>fungame.{key}</c> 键，
/// 找不到对应键时回退到 Fungame 对象上的原始值。
/// </summary>
public static class FungameLocale
{
    private const string FungameType = "fungame";

    /// <summary>
    /// 获取本地化的 Fungame 名称。
    /// 优先从 <c>{DirectoryPath}/lang/{currentLang}.json</c> 读取 <c>fungame.name</c> 键，
    /// 若不存在则返回 <see cref="Fungame.Name"/> 原始值。
    /// </summary>
    public static string GetName(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        var localized = ReadFromFungameLang(fungame, "name");
        return localized ?? fungame.Name ?? string.Empty;
    }

    /// <summary>
    /// 获取本地化的 Fungame 描述。
    /// 优先从 <c>{DirectoryPath}/lang/{currentLang}.json</c> 读取 <c>fungame.description</c> 键，
    /// 若不存在则返回 <see cref="Fungame.Description"/> 原始值。
    /// </summary>
    public static string GetDescription(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        var localized = ReadFromFungameLang(fungame, "description");
        return localized ?? fungame.Description ?? string.Empty;
    }

    /// <summary>
    /// 获取作者字符串（来自 Fungame 对象，不经过本地化）。
    /// </summary>
    public static string GetAuthor(Fungame fungame)
    {
        return fungame?.Authors ?? string.Empty;
    }

    /// <summary>
    /// 通用的 Fungame 格式化方法。
    /// 从插件 locale 中读取 <c>format.{formatKey}</c> 键并填充参数。
    /// </summary>
    public static string GetFormatted(string formatKey, params object[] args)
    {
        return ModLocale.GetFormat($"format.{formatKey}", args);
    }

    /// <summary>
    /// 获取格式化名称 + 版本号，例如 "MyMap v1.0.0"。
    /// </summary>
    public static string GetFormattedNameVersion(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        return $"{GetName(fungame)} v{fungame.Version ?? "1.0.0"}";
    }

    /// <summary>
    /// 获取格式化作者信息，例如 "by Author1, Author2"。
    /// </summary>
    public static string GetFormattedAuthor(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        return GetFormatted("author", GetAuthor(fungame));
    }

    /// <summary>
    /// 获取完整显示文本（含换行）。
    /// </summary>
    public static string GetFormattedInfo(Fungame fungame)
    {
        return fungame == null
            ? string.Empty
            : $"{GetFormattedNameVersion(fungame)}\n{GetFormattedAuthor(fungame)}";
    }

    /// <summary>
    /// 获取本地化的特性描述字符串。
    /// </summary>
    public static string GetFormattedFeatures(Fungame fungame)
    {
        if (fungame == null) return string.Empty;
        return GetFormatted("features", fungame.ActiveFeatures);
    }

    /// <summary>
    /// 将 Fungame 的 name/description/author 写入当前语言的 lang 文件。
    /// 文件路径为 <c>{fungame.DirectoryPath}/lang/{currentLang}.json</c>，键名为 <c>fungame.{key}</c>。
    /// </summary>
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

    /// <summary>
    /// 从 Fungame 的 lang 文件读取 <c>fungame.{key}</c> 的值。
    /// </summary>
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
