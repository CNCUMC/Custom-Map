using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using MossLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomFungamePack;

public static class FungameCheck
{
    private static ManualLogSource _logger;
    private const string LogPrefix = "log.fungame_check.";
    private static readonly string FungamesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fungames");
    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> CheckFailDirectories = [];
    public static readonly List<Fungame> Fungames = [];

    public static void Initialize()
    {
        _logger = Plugin.Logger;
        LoadFungameDirectories();
    }

    private static void LoadFungameDirectories()
    {
        var directories = Directory.GetDirectories(FungamesPath);

        // 早期初始化，直接使用 Logger，不依赖本地化
        _logger.LogInfo($"已读取 {directories.Length} 个Fungame文件夹");

        ValidDirectories.Clear();
        CheckFailDirectories.Clear();
        Fungames.Clear();

        foreach (var fungamesDirectory in directories)
        {
            var missingFiles = (
                from requiredFile
                    in new[] { "fungame.json" }
                let filePath = Path.Combine(fungamesDirectory, requiredFile)
                where !File.Exists(filePath)
                select requiredFile).ToList();

            if (missingFiles.Count > 0)
            {
                _logger.LogWarning($"{Path.GetFileName(fungamesDirectory)} 缺少文件: {string.Join(", ", missingFiles)}");
                continue;
            }

            ValidDirectories.Add(fungamesDirectory);
        }

        if (ValidDirectories.Count != 0)
        {
            _logger.LogInfo($"有效目录: {ValidDirectories.Count} 个，正在加载中...");

            var directoriesToValidate = ValidDirectories.ToList();
            foreach (var fungame in directoriesToValidate)
            {
                if (!ValidateAndLoadFungame(Path.Combine(fungame, "fungame.json")))
                {
                    _logger.LogWarning($"{Path.GetFileName(fungame)} 识别失败!");
                    CheckFailDirectories.Add(fungame);
                }
            }

            if (CheckFailDirectories.Count != 0)
            {
                _logger.LogInfo($"目录识别失败: {CheckFailDirectories.Count} 个：");
                foreach (var failDirectory in CheckFailDirectories)
                {
                    _logger.LogInfo($"- {Path.GetFileName(failDirectory)}");
                    ValidDirectories.Remove(failDirectory);
                }
            }
        }
    }

    private static bool ValidateAndLoadFungame(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError($"找不到 fungame.json 文件: {filePath}");
                return false;
            }

            var jsonContent = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonContent);
            var errors = new List<string>();
            var warnings = new List<string>();

            ValidateRequiredFieldWithDefault(jsonObject, "name", warnings, "Unnamed Fungame");
            ValidateRequiredFieldWithDefault(jsonObject, "id", warnings, GenerateDefaultId(filePath));
            ValidateRequiredFieldWithDefault(jsonObject, "version", warnings, "1.0");
            ValidateRequiredListFieldWithDefault(jsonObject, "author", warnings, "Unknown");
            ValidateRequiredFieldWithDefault(jsonObject, "description", warnings, "No description provided");
            ValidateOptionalListField(jsonObject, "command", warnings);

            if (jsonObject["id"] != null && jsonObject["id"].Type == JTokenType.String)
            {
                var idValue = jsonObject["id"].ToString();
                if (!IsValidId(idValue))
                {
                    warnings.Add(Locale("fungame_check.id_format_warning"));
                    jsonObject["id"] = SanitizeId(idValue);
                }
            }

            if (jsonObject["author"] != null && jsonObject["author"].Type == JTokenType.Array)
            {
                if (jsonObject["author"] is JArray authorArray)
                {
                    for (int i = 0; i < authorArray.Count; i++)
                    {
                        if (authorArray[i].Type == JTokenType.String) continue;
                        warnings.Add(Locale("fungame_check.author_not_string", i));
                        authorArray[i].Remove();
                        i--;
                    }

                    if (authorArray.Count == 0)
                    {
                        warnings.Add(Locale("fungame_check.author_empty"));
                        jsonObject["author"] = new JArray(new object[] { "Unknown" });
                    }
                }
            }

            if (jsonObject.ContainsKey("map_data") && jsonObject["map_data"] != null)
            {
                ValidateMapData(jsonObject["map_data"] as JObject, errors, warnings);
            }

            if (jsonObject.ContainsKey("features") && jsonObject["features"] != null)
            {
                ValidateFeatures(jsonObject["features"] as JArray, errors, warnings);
            }

            if (jsonObject.ContainsKey("spawn") && jsonObject["spawn"] != null)
            {
                ValidateSpawn(jsonObject["spawn"] as JArray, errors, warnings);
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning($"fungame.json 文件验证失败: {Path.GetFileName(filePath)}");
                foreach (var error in errors)
                {
                    _logger.LogWarning($"  - {error}");
                }

                return false;
            }

            if (warnings.Count > 0)
            {
                _logger.LogWarning($"fungame.json 文件验证通过，但存在默认值: {Path.GetFileName(filePath)}");
                foreach (var warning in warnings)
                {
                    _logger.LogWarning($"  - {warning}");
                }
            }

            var modifiedJson = jsonObject.ToString(Formatting.None);
            var fungame = JsonConvert.DeserializeObject<Fungame>(modifiedJson);

            if (!IsValidVersion(fungame.Version))
            {
                warnings.Add(Locale("fungame_check.version_format_warning", fungame.Version));
                fungame.Version = "1.0";
            }

            var hasMap = fungame.MapData != null;
            if (hasMap)
            {
                if (!ValidateMapDataInObject(fungame.MapData))
                {
                    _logger.LogWarning($"fungame.json 文件验证失败: {Path.GetFileName(filePath)}");
                    return false;
                }
            }

            Fungames.Add(fungame);

            var name = fungame.Name;
            var id = fungame.Id;
            var version = fungame.Version;

            _logger.LogInfo(hasMap
                ? $"成功加载Fungame: {name} (ID: {id}, Version: {version}, 包含地图数据)"
                : $"成功加载Fungame: {name} (ID: {id}, Version: {version})");

            if (warnings.Count > 0)
            {
                _logger.LogInfo("请检查并修复上述警告");
            }

            return true;
        }
        catch (Exception ex) when (ex is JsonException or UnauthorizedAccessException or IOException
                                       or ArgumentException)
        {
            _logger.LogError($"fungame.json 文件处理失败: {Path.GetFileName(filePath)} ({ex.Message})");
            return false;
        }
    }

    private static bool IsValidVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return false;

        var parts = version.Split('.');
        return parts.Length is >= 2 and <= 4 && parts.All(part => int.TryParse(part, out _));
    }

    private static void ValidateMapData(JObject mapObject, List<string> errors, List<string> _ = null)
    {
        if (mapObject == null)
        {
            errors.Add(Locale("validation.map_invalid_type"));
            return;
        }

        if (!mapObject.ContainsKey("x"))
        {
            errors.Add(Locale("validation.map_missing_x"));
        }
        else if (mapObject["x"] == null || mapObject["x"].Type != JTokenType.Integer)
        {
            errors.Add(Locale("validation.map_x_not_integer"));
        }

        if (!mapObject.ContainsKey("y"))
        {
            errors.Add(Locale("validation.map_missing_y"));
        }
        else if (mapObject["y"] == null || mapObject["y"].Type != JTokenType.Integer)
        {
            errors.Add(Locale("validation.map_y_not_integer"));
        }

        if (!mapObject.ContainsKey("map"))
        {
            errors.Add(Locale("validation.map_missing_map"));
        }
        else if (mapObject["map"] == null || mapObject["map"].Type != JTokenType.Array)
        {
            errors.Add(Locale("validation.map_map_not_array"));
        }
        else
        {
            if (mapObject["map"] is not JArray mapArray || mapArray.Count == 0)
            {
                errors.Add(Locale("validation.map_map_empty"));
            }
            else
            {
                for (int i = 0; i < mapArray.Count; i++)
                {
                    if (mapArray[i].Type == JTokenType.String) continue;
                    errors.Add(Locale("validation.map_row_not_string", i));
                    break;
                }
            }
        }

        if (!mapObject.ContainsKey("key"))
        {
            errors.Add(Locale("validation.map_missing_key"));
        }
        else if (mapObject["key"] == null || mapObject["key"].Type != JTokenType.Object)
        {
            errors.Add(Locale("validation.map_key_not_object"));
        }

        if (!mapObject.ContainsKey("items") || mapObject["items"] == null)
            return;

        if (mapObject["items"].Type != JTokenType.Array)
        {
            errors.Add(Locale("validation.map_items_not_array"));
        }
        else
        {
            if (mapObject["items"] is not JArray { Count: > 0 } itemsArray)
                return;

            for (int i = 0; i < itemsArray.Count; i++)
            {
                if (itemsArray[i].Type != JTokenType.Array)
                {
                    errors.Add(Locale("validation.map_item_row_not_array", i));
                    break;
                }

                var rowArray = itemsArray[i] as JArray;
                if (rowArray == null) continue;

                for (int j = 0; j < rowArray.Count; j++)
                {
                    if (rowArray[j].Type == JTokenType.String) continue;
                    errors.Add(Locale("validation.map_item_not_string", i, j));
                    break;
                }
            }
        }
    }

    private static void ValidateFeatures(JArray featuresArray, List<string> errors, List<string> warnings = null)
    {
        if (featuresArray == null)
        {
            errors.Add(Locale("validation.features_invalid_type"));
            return;
        }

        if (featuresArray.Count == 0)
        {
            warnings?.Add(Locale("validation.features_empty"));
            return;
        }

        for (int i = 0; i < featuresArray.Count; i++)
        {
            if (featuresArray[i].Type != JTokenType.String &&
                featuresArray[i].Type != JTokenType.Object &&
                featuresArray[i].Type != JTokenType.Float &&
                featuresArray[i].Type != JTokenType.Integer)
            {
                warnings?.Add(Locale("validation.features_element_invalid", i));
            }
        }
    }

    private static void ValidateSpawn(JArray spawnArray, List<string> errors, List<string> warnings = null)
    {
        if (spawnArray == null)
        {
            errors.Add(Locale("validation.spawn_must_be_array"));
            return;
        }

        if (spawnArray.Count != 2)
        {
            warnings?.Add(Locale("validation.spawn_wrong_count", spawnArray.Count));
            return;
        }

        for (int i = 0; i < spawnArray.Count; i++)
        {
            if (spawnArray[i].Type != JTokenType.Float && spawnArray[i].Type != JTokenType.Integer)
            {
                warnings?.Add(Locale("validation.spawn_element_not_number", i));
            }
        }
    }

    private static bool ValidateMapDataInObject(MapData mapData)
    {
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            _logger.LogWarning(Locale("validation.no_data", Locale("common.map"), "string map"));
            return false;
        }

        if (mapData.Key == null || mapData.Key.Count == 0)
        {
            _logger.LogWarning(Locale("validation.map_missing_key"));
            return false;
        }

        var maxColCount = mapData.Map.Max(row => row?.Length ?? 0);

        if (maxColCount == 0)
        {
            _logger.LogWarning(Locale("validation.row_data_empty", Locale("common.map")));
            return false;
        }

        return true;
    }

    private static void ValidateRequiredFieldWithDefault(JObject jsonObject, string fieldName, List<string> warnings,
        string defaultValue)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            warnings.Add(Locale("validation.field_missing_default", fieldName, defaultValue));
            jsonObject[fieldName] = defaultValue;
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            warnings.Add(Locale("validation.field_null_default", fieldName, defaultValue));
            jsonObject[fieldName] = defaultValue;
        }
        else if (jsonObject[fieldName].Type == JTokenType.String &&
                 string.IsNullOrWhiteSpace(jsonObject[fieldName].ToString()))
        {
            warnings.Add(Locale("validation.field_empty_string_default", fieldName, defaultValue));
            jsonObject[fieldName] = defaultValue;
        }
    }

    private static void ValidateRequiredListFieldWithDefault(JObject jsonObject, string fieldName,
        List<string> warnings, string defaultItem)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            warnings.Add(Locale("validation.field_must_be_array_default", fieldName, defaultItem));
            jsonObject[fieldName] = new JArray(new object[] { defaultItem });
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            warnings.Add(Locale("validation.field_null_array_default", fieldName, defaultItem));
            jsonObject[fieldName] = new JArray(new object[] { defaultItem });
        }
        else if (jsonObject[fieldName].Type != JTokenType.Array)
        {
            warnings.Add(Locale("validation.field_convert_to_array", fieldName));
            var currentValue = jsonObject[fieldName].ToString();
            jsonObject[fieldName] = new JArray(new object[] { currentValue });
        }
        else
        {
            if (jsonObject[fieldName] is JArray array && array.Count != 0) return;
            warnings.Add(Locale("validation.array_empty_default", fieldName));
            jsonObject[fieldName] = new JArray(new object[] { defaultItem });
        }
    }

    private static void ValidateOptionalListField(JObject jsonObject, string fieldName, List<string> warnings)
    {
        if (!jsonObject.ContainsKey(fieldName) || jsonObject[fieldName] == null) return;
        if (jsonObject[fieldName].Type != JTokenType.Array)
        {
            warnings.Add(Locale("validation.field_convert_to_array", fieldName));
            var currentValue = jsonObject[fieldName].ToString();
            jsonObject[fieldName] = new JArray(new object[] { currentValue });
        }
        else
        {
            if (jsonObject[fieldName] is JArray array && array.Count != 0) return;
            warnings.Add(Locale("validation.array_empty_removed", fieldName));
            jsonObject[fieldName].Remove();
        }
    }

    private static string GenerateDefaultId(string filePath)
    {
        var folderName = Path.GetFileName(Path.GetDirectoryName(filePath));
        return SanitizeId(folderName ?? "unknown_fungame");
    }

    private static string SanitizeId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return "unknown_fungame";

        var sanitized = new System.Text.StringBuilder();
        foreach (var c in id.ToLower())
        {
            if (char.IsLower(c) || char.IsDigit(c) || c == '_')
                sanitized.Append(c);
            else if (char.IsUpper(c))
                sanitized.Append(char.ToLower(c));
            else
                sanitized.Append('_');
        }

        var result = sanitized.ToString();
        return string.IsNullOrWhiteSpace(result) ? "unknown_fungame" : result;
    }

    private static bool IsValidId(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && id.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_');
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.GetFormat($"{LogPrefix}{key}", args);
    }
}