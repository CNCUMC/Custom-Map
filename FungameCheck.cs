using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using MossLib.Tool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomFungamePack;

public static class FungameCheck
{
    private static ManualLogSource _logger;
    private const string LocaleKeyPre = "fungame_check.";
    public static readonly string FungamesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fungames");
    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> CheckFailDirectories = [];
    public static readonly List<Fungame> Fungames = [];
    public static Fungame CurrentFungame => WorldGenerationPatch.CurrentFungame;
    public static bool HasRunningFungame => CurrentFungame != null;

    public static void Initialize()
    {
        _logger = Plugin.Logger;
        LoadFungameDirectories();
    }

    private static void LoadFungameDirectories()
    {
        var directories = Directory.GetDirectories(FungamesPath);

        _logger.LogInfo($"Read {directories.Length} Fungame folders");

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
                Warning($"{Path.GetFileName(fungamesDirectory)} Missing files: {string.Join(", ", missingFiles)}");
                continue;
            }

            ValidDirectories.Add(fungamesDirectory);
        }

        Fungames.Add(Plugin.TemplateFungame);

        if (ValidDirectories.Count == 0) return;
        _logger.LogInfo($"Valid directories: {ValidDirectories.Count}, loading...");

        var directoriesToValidate = ValidDirectories.ToList();
        foreach (var fungame in directoriesToValidate.Where(fungame =>
                     !ValidateAndLoadFungame(Path.Combine(fungame, "fungame.json"))))
        {
            UninitializedWarning($"{Path.GetFileName(fungame)} Validation failed!");
            CheckFailDirectories.Add(fungame);
        }

        if (CheckFailDirectories.Count == 0) return;
        _logger.LogInfo($"Directory validation failed: {CheckFailDirectories.Count}:");
        foreach (var failDirectory in CheckFailDirectories)
        {
            _logger.LogInfo($"- {Path.GetFileName(failDirectory)}");
            ValidDirectories.Remove(failDirectory);
        }
    }

    private static bool ValidateAndLoadFungame(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError($"Cannot find fungame.json file: {filePath}");
                return false;
            }

            var jsonContent = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonContent);
            var errors = new List<string>();
            var warnings = new List<string>();

            NormalizeKey(jsonObject, "Name", "name");
            NormalizeKey(jsonObject, "Id", "id");
            NormalizeKey(jsonObject, "Version", "version");
            NormalizeKey(jsonObject, "Author", "author");
            NormalizeKey(jsonObject, "Description", "description");
            NormalizeKey(jsonObject, "Feature", "feature");
            NormalizeKey(jsonObject, "Waypoints", "waypoints");
            NormalizeKey(jsonObject, "Items", "items");
            NormalizeKey(jsonObject, "Spawn", "spawn");
            NormalizeKey(jsonObject, "X", "x");
            NormalizeKey(jsonObject, "Y", "y");

            ValidateRequiredFieldWithDefault(jsonObject, "name", warnings, "Unnamed Fungame");
            ValidateRequiredFieldWithDefault(jsonObject, "id", warnings, GenerateDefaultId(filePath));
            ValidateRequiredFieldWithDefault(jsonObject, "version", warnings, "1.0.0");
            ValidateRequiredListFieldWithDefault(jsonObject, "author", warnings, "Unknown");
            ValidateRequiredFieldWithDefault(jsonObject, "description", warnings, "No description provided");
            ValidateOptionalListField(jsonObject, "command", warnings);

            if (jsonObject["id"] != null && jsonObject["id"].Type == JTokenType.String)
            {
                var idValue = jsonObject["id"].ToString();
                if (!IsValidId(idValue))
                {
                    warnings.Add(Locale("id_format_warning"));
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
                        warnings.Add(Locale("author_not_string", i));
                        authorArray[i].Remove();
                        i--;
                    }

                    if (authorArray.Count == 0)
                    {
                        warnings.Add(Locale("author_empty"));
                        jsonObject["author"] = new JArray(new object[] { "Unknown" });
                    }
                }
            }

            bool hasCustomStructuresMod = Type.GetType("Custom_Structures.Plugin, Custom Structures") != null;
            bool hasCustomStructuresField =
                jsonObject.ContainsKey("custom_structures") && jsonObject["custom_structures"] != null
                                                            && jsonObject["custom_structures"].Type != JTokenType.Null;
            bool hasMapData = jsonObject.ContainsKey("map_data") && jsonObject["map_data"] != null
                                                                 && jsonObject["map_data"].Type != JTokenType.Null;
            bool hasBuildModeSave = jsonObject.ContainsKey("build_mode_save")
                                    && jsonObject["build_mode_save"] != null
                                    && jsonObject["build_mode_save"].Type != JTokenType.Null
                                    && !string.IsNullOrWhiteSpace(jsonObject["build_mode_save"].ToString());

            int contentTypeCount = (hasMapData ? 1 : 0)
                                   + (hasCustomStructuresField ? 1 : 0)
                                   + (hasBuildModeSave ? 1 : 0);

            if (contentTypeCount > 1)
            {
                warnings.Add(Validation("multiple_content_types"));
            }

            // map_data > custom_structures > build_mode_save
            if (hasMapData)
            {
                ValidateMapData(jsonObject["map_data"] as JObject, errors);
            }
            else if (hasCustomStructuresField)
            {
                if (!hasCustomStructuresMod)
                {
                    errors.Add(Validation("custom_structures_without_mod"));
                }
            }
            else if (hasBuildModeSave)
            {
                // build_mode_save is valid
            }
            else
            {
                errors.Add(Validation("missing_content_type"));
            }


            if (jsonObject.ContainsKey("features") && jsonObject["features"] != null)
            {
                ValidateFeatures(jsonObject["features"] as JArray, errors, warnings);
            }

            if (errors.Count > 0)
            {
                UninitializedWarning($"fungame.json validation failed: {Path.GetFileName(filePath)}");
                foreach (var error in errors)
                {
                    UninitializedWarning($"  - {error}");
                }

                return false;
            }

            if (warnings.Count > 0)
            {
                UninitializedWarning($"fungame.json validation passed with defaults: {Path.GetFileName(filePath)}");
                foreach (var warning in warnings)
                {
                    UninitializedWarning($"  - {warning}");
                }
            }

            var modifiedJson = jsonObject.ToString(Formatting.None);
            var fungame = JsonConvert.DeserializeObject<Fungame>(modifiedJson);

            if (!IsValidVersion(fungame.Version))
            {
                warnings.Add(Locale("version_format_warning", fungame.Version));
                fungame.Version = "1.0";
            }

            var hasMap = fungame.MapData != null;
            if (hasMap)
            {
                if (!ValidateMapDataInObject(fungame.MapData))
                {
                    UninitializedWarning($"fungame.json validation failed: {Path.GetFileName(filePath)}");
                    return false;
                }
            }

            fungame.DirectoryPath = Path.GetDirectoryName(filePath);
            Fungames.Add(fungame);

            var name = fungame.Name;
            var id = fungame.Id;
            var version = fungame.Version;

            _logger.LogInfo(hasMap
                ? $"Successfully loaded Fungame: {name} (ID: {id}, Version: {version}, includes map data)"
                : $"Successfully loaded Fungame: {name} (ID: {id}, Version: {version})");

            if (warnings.Count > 0)
            {
                _logger.LogInfo("Please check and fix the above warnings");
            }

            return true;
        }
        catch (Exception ex) when (ex is JsonException or UnauthorizedAccessException or IOException
                                       or ArgumentException)
        {
            UninitializedWarning($"fungame.json processing failed: {Path.GetFileName(filePath)} ({ex.Message})");
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

    private static void ValidateMapData(JObject mapObject, List<string> errors)
    {
        if (mapObject == null)
        {
            errors.Add(Validation("map_invalid_type"));
            return;
        }

        if (!mapObject.ContainsKey("map") && mapObject.ContainsKey("Map"))
            mapObject["map"] = mapObject["Map"];
        if (!mapObject.ContainsKey("key") && mapObject.ContainsKey("Key"))
            mapObject["key"] = mapObject["Key"];

        if (!mapObject.ContainsKey("map"))
        {
            errors.Add(Validation("map_missing_field", "map"));
        }
        else if (mapObject["map"] == null
                 || mapObject["map"].Type != JTokenType.Array)
        {
            errors.Add(Validation("map_field_type_error", "map", "array"));
        }

        else
        {
            if (mapObject["map"] is not JArray mapArray
                || mapArray.Count == 0)
            {
                errors.Add(Validation("map_map_empty"));
            }
            else
            {
                for (int i = 0; i < mapArray.Count; i++)
                {
                    if (mapArray[i].Type == JTokenType.String) continue;
                    errors.Add(Validation("map_row_not_string", i));
                    break;
                }
            }
        }

        if (!mapObject.ContainsKey("key"))
        {
            errors.Add(Validation("map_missing_field", "key"));
        }
        else if (mapObject["key"] == null
                 || mapObject["key"].Type != JTokenType.Object)
        {
            errors.Add(Validation("map_field_type_error", "key", "object"));
        }

        if (!mapObject.ContainsKey("items")
            || mapObject["items"] == null)
            return;

        if (mapObject["items"].Type != JTokenType.Array)
        {
            errors.Add(Validation("map_field_type_error", "items", "array"));
        }
        else
        {
            if (mapObject["items"] is not JArray { Count: > 0 } itemsArray)
                return;

            for (int i = 0; i < itemsArray.Count; i++)
            {
                if (itemsArray[i].Type != JTokenType.Array)
                {
                    errors.Add(Validation("map_item_row_not_array", i));
                    break;
                }

                var rowArray = itemsArray[i] as JArray;
                if (rowArray == null) continue;

                for (int j = 0; j < rowArray.Count; j++)
                {
                    if (rowArray[j].Type == JTokenType.String) continue;
                    errors.Add(Validation("map_item_not_string", i, j));
                    break;
                }
            }
        }
    }

    private static void ValidateFeatures(JArray featuresArray, List<string> errors, List<string> warnings = null)
    {
        if (featuresArray == null)
        {
            errors.Add(Validation("features_invalid_type"));
            return;
        }

        if (featuresArray.Count == 0)
        {
            warnings?.Add(Validation("features_empty"));
            return;
        }

        for (int i = 0; i < featuresArray.Count; i++)
        {
            if (featuresArray[i].Type != JTokenType.String &&
                featuresArray[i].Type != JTokenType.Object &&
                featuresArray[i].Type != JTokenType.Float &&
                featuresArray[i].Type != JTokenType.Integer)
            {
                warnings?.Add(Validation("features_element_invalid", i));
            }
        }
    }

    private static bool ValidateMapDataInObject(MapData mapData)
    {
        if (mapData.Map == null || mapData.Map.Length == 0)
        {
            Warning(Validation("no_data", Locale("common.map"), "string map"));
            return false;
        }

        if (mapData.Key == null || mapData.Key.Count == 0)
        {
            Warning(Validation("map_missing_field", "key"));
            return false;
        }

        var maxColCount = mapData.Map.Max(row => row?.Length ?? 0);

        if (maxColCount != 0) return true;
        Warning(Validation("row_data_empty", Locale("common.map")));
        return false;
    }

    private static void ValidateRequiredFieldWithDefault(JObject jsonObject, string fieldName, List<string> warnings,
        string defaultValue)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            warnings.Add(Validation("field_missing_default", fieldName, defaultValue));
            jsonObject[fieldName] = defaultValue;
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            warnings.Add(Validation("field_null_default", fieldName, defaultValue));
            jsonObject[fieldName] = defaultValue;
        }
        else if (jsonObject[fieldName].Type == JTokenType.String &&
                 string.IsNullOrWhiteSpace(jsonObject[fieldName].ToString()))
        {
            warnings.Add(Validation("field_empty_string_default", fieldName, defaultValue));
            jsonObject[fieldName] = defaultValue;
        }
    }

    private static void ValidateRequiredListFieldWithDefault(JObject jsonObject, string fieldName,
        List<string> warnings, string defaultItem)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            warnings.Add(Validation("field_must_be_array_default", fieldName, defaultItem));
            jsonObject[fieldName] = new JArray(new object[] { defaultItem });
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            warnings.Add(Validation("field_null_array_default", fieldName, defaultItem));
            jsonObject[fieldName] = new JArray(new object[] { defaultItem });
        }
        else if (jsonObject[fieldName].Type != JTokenType.Array)
        {
            warnings.Add(Validation("field_convert_to_array", fieldName));
            var currentValue = jsonObject[fieldName].ToString();
            jsonObject[fieldName] = new JArray(new object[] { currentValue });
        }
        else
        {
            if (jsonObject[fieldName] is JArray array && array.Count != 0) return;
            warnings.Add(Validation("array_empty_default", fieldName));
            jsonObject[fieldName] = new JArray(new object[] { defaultItem });
        }
    }

    private static void ValidateOptionalListField(JObject jsonObject, string fieldName, List<string> warnings)
    {
        if (!jsonObject.ContainsKey(fieldName) || jsonObject[fieldName] == null) return;
        if (jsonObject[fieldName].Type != JTokenType.Array)
        {
            warnings.Add(Validation("field_convert_to_array", fieldName));
            var currentValue = jsonObject[fieldName].ToString();
            jsonObject[fieldName] = new JArray(new object[] { currentValue });
        }
        else
        {
            if (jsonObject[fieldName] is JArray array && array.Count != 0) return;
            warnings.Add(Validation("array_empty_removed", fieldName));
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

    private static void NormalizeKey(JObject jsonObject, string pascalKey, string lowerKey)
    {
        if (!jsonObject.ContainsKey(lowerKey) && jsonObject.ContainsKey(pascalKey))
            jsonObject[lowerKey] = jsonObject[pascalKey];
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.Log($"{LocaleKeyPre}{key}", args);
    }

    private static string Validation(string key, params object[] args)
    {
        return ModLocale.Log($"validation.{key}", args);
    }

    private static void Warning(string key)
    {
        Log.Warning(key, _logger);
    }

    private static void UninitializedWarning(string key)
    {
        _logger.LogWarning($"{key}");
    }

    public static string GetFungamePath(Fungame fungame = null)
    {
        var target = fungame ?? CurrentFungame;
        return string.IsNullOrEmpty(target?.DirectoryPath) ? null : target.DirectoryPath;
    }
}