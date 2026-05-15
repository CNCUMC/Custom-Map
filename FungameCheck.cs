using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomFungamePack;

public static class FungameCheck
{
    private static readonly ManualLogSource Logger = Plugin.Logger;
    public static readonly string FungamesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fungames");
    public static string[] FungamesDirectories;
    public static readonly List<string> ValidDirectories = [];
    public static readonly List<string> InvalidDirectories = [];

    public static void Initialize()
    {
        CheckFungameDirectory();
        LoadFungameDirectories();
        CheckFungameDirectories();
    }

    private static void LoadFungameDirectories()
    {
        FungamesDirectories = Directory.GetDirectories(FungamesDirectory);
        Logger.LogInfo($"已读取 {FungamesDirectories.Length} 个Fungame文件夹");
    }

    private static void CheckFungameDirectories()
    {
        var requiredFiles = new[] { "fungame.json" };

        foreach (var fungamesDirectory in FungamesDirectories)
        {
            var missingFiles = (
                from requiredFile
                    in requiredFiles
                let filePath = Path.Combine(fungamesDirectory, requiredFile)
                where !File.Exists(filePath)
                select requiredFile).ToList();

            if (missingFiles.Count > 0)
            {
                InvalidDirectories.Add(fungamesDirectory);
                var folderName = Path.GetFileName(fungamesDirectory);
                Logger.LogError($"Fungame目录缺少必需文件：{folderName}");
                foreach (var missingFile in missingFiles)
                {
                    Logger.LogError($"  - 缺失文件: {missingFile}");
                }

                continue;
            }

            ValidDirectories.Add(fungamesDirectory);
        }

        if (ValidDirectories.Count != 0)
        {
            var checkFailDirectories = new List<string>();
            Logger.LogInfo($"有效目录: {ValidDirectories.Count} 个，正在加载中...");
            
            var directoriesToValidate = ValidDirectories.ToList();
            foreach (var fungame in directoriesToValidate)
            {
                if (!ValidateAndLoadFungame(Path.Combine(fungame, "fungame.json")))
                {
                    var folderName = Path.GetFileName(fungame);
                    Logger.LogWarning($"{folderName} 识别失败!");
                    checkFailDirectories.Add(fungame);
                }
            }

            if (checkFailDirectories.Count != 0)
            {
                Logger.LogInfo($"目录识别失败: {checkFailDirectories.Count} 个：");
                foreach (var failDirectory in checkFailDirectories)
                {
                    var folderName = Path.GetFileName(failDirectory);
                    Logger.LogInfo($"- {folderName}");
                    ValidDirectories.Remove(failDirectory);
                    InvalidDirectories.Add(failDirectory);
                }
            }
        }

        if (InvalidDirectories.Count != 0)
        {
            Logger.LogInfo($"无效目录: {InvalidDirectories.Count} 个：");
            foreach (var invalidDirectory in InvalidDirectories)
            {
                var folderName = Path.GetFileName(invalidDirectory);
                Logger.LogInfo($"- {folderName}");
            }
        }
    }

    private static void CheckFungameDirectory()
    {
        if (Directory.Exists(FungamesDirectory)) return;
        
        try
        {
            Directory.CreateDirectory(FungamesDirectory);
            Logger.LogInfo($"Fungames目录不存在，已创建：{FungamesDirectory}");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            Logger.LogError($"Fungames目录创建失败（权限不足）：{FungamesDirectory}，详情：{ex.Message}");
        }
    }

private static bool ValidateAndLoadFungame(string filePath)
    {
        try
        {
            var jsonContent = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonContent);
            var errors = new List<string>();
            var warnings = new List<string>();

            ValidateRequiredFieldWithDefault(jsonObject, "name", errors, warnings, "Unnamed Fungame");
            ValidateRequiredFieldWithDefault(jsonObject, "id", errors, warnings, GenerateDefaultId(filePath));
            ValidateRequiredFieldWithDefault(jsonObject, "version", errors, warnings, "1.0");
            ValidateRequiredListFieldWithDefault(jsonObject, "author", errors, warnings, "Unknown");
            ValidateRequiredFieldWithDefault(jsonObject, "description", errors, warnings, "No description provided");
            ValidateOptionalListField(jsonObject, "command", warnings);

            if (jsonObject["id"] != null && jsonObject["id"].Type == JTokenType.String)
            {
                var idValue = jsonObject["id"].ToString();
                if (!IsValidId(idValue))
                {
                    warnings.Add("id 字段只能包含小写字母、数字和下划线，已自动修正");
                    jsonObject["id"] = SanitizeId(idValue);
                }
            }

            if (jsonObject["author"] != null && jsonObject["author"].Type == JTokenType.Array)
            {
                var authorArray = jsonObject["author"] as JArray;
                if (authorArray != null)
                {
                    for (int i = 0; i < authorArray.Count; i++)
                    {
                        if (authorArray[i].Type != JTokenType.String)
                        {
                            warnings.Add($"author 列表第 {i} 个元素不是字符串，已移除");
                            authorArray[i].Remove();
                            i--;
                        }
                    }
                    
                    if (authorArray.Count == 0)
                    {
                        warnings.Add("author 列表为空，已设置默认值");
                        jsonObject["author"] = new JArray(new[] { "Unknown" });
                    }
                }
            }

            if (jsonObject.ContainsKey("map") && jsonObject["map"] != null)
            {
                ValidateMapData(jsonObject["map"] as JObject, errors, warnings);
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
                Logger.LogWarning($"Fungame文件验证失败: {Path.GetFileName(filePath)}");
                foreach (var error in errors)
                {
                    Logger.LogWarning($"  - {error}");
                }

                return false;
            }

            if (warnings.Count > 0)
            {
                Logger.LogWarning($"Fungame文件存在问题，已使用默认值: {Path.GetFileName(filePath)}");
                foreach (var warning in warnings)
                {
                    Logger.LogWarning($"  - {warning}");
                }
            }

            var modifiedJson = jsonObject.ToString(Formatting.None);
            var fungame = JsonConvert.DeserializeObject<Fungame>(modifiedJson);

            if (!IsValidVersion(fungame.Version))
            {
                warnings.Add($"Version格式不正确: {fungame.Version}，已设置为 1.0");
                fungame.Version = "1.0";
            }

            if (fungame.Map != null)
            {
                if (!ValidateMapDataInObject(fungame.Map))
                {
                    Logger.LogWarning($"Fungame文件的地图数据无效: {Path.GetFileName(filePath)}");
                    return false;
                }
                Logger.LogInfo($"成功加载Fungame: {fungame.Name} (ID: {fungame.Id}, Version: {fungame.Version}, 包含地图数据)");
            }
            else
            {
                Logger.LogInfo($"成功加载Fungame: {fungame.Name} (ID: {fungame.Id}, Version: {fungame.Version})");
            }
            
            if (warnings.Count > 0)
            {
                Logger.LogInfo("提示: 建议修复上述警告以提高兼容性");
            }
            
            return true;
        }
        catch (Exception ex) when (ex is JsonException or UnauthorizedAccessException or IOException or ArgumentException)
        {
            Logger.LogError($"Fungame文件处理失败: {Path.GetFileName(filePath)}, 详情: {ex.Message}");
            return false;
        }
    }

private static bool IsValidVersion(string version)
{
    if (string.IsNullOrWhiteSpace(version))
            return false;

        var parts = version.Split('.');
        if (parts.Length is < 2 or > 4)
            return false;

        return parts.All(part => int.TryParse(part, out _));
    }

 private static void ValidateMapData(JObject mapObject, List<string> errors, List<string> warnings = null)
    {
        if (mapObject == null)
        {
            errors.Add("map 字段格式不正确");
            return;
        }

        if (!mapObject.ContainsKey("x"))
        {
            errors.Add("地图缺少必需字段: x");
        }
        else if (mapObject["x"] == null || mapObject["x"].Type != JTokenType.Integer)
        {
            errors.Add("地图 x 字段必须是整数");
        }

        if (!mapObject.ContainsKey("y"))
        {
            errors.Add("地图缺少必需字段: y");
        }
        else if (mapObject["y"] == null || mapObject["y"].Type != JTokenType.Integer)
        {
            errors.Add("地图 y 字段必须是整数");
        }

        if (!mapObject.ContainsKey("blocks"))
        {
            errors.Add("地图缺少必需字段: blocks");
        }
        else if (mapObject["blocks"] == null || mapObject["blocks"].Type != JTokenType.Array)
        {
            errors.Add("地图 blocks 字段必须是二维数组");
        }
        else
        {
            var blocksArray = mapObject["blocks"] as JArray;
            if (blocksArray == null || blocksArray.Count == 0)
            {
                errors.Add("地图 blocks 数组不能为空");
            }
            else
            {
                for (int i = 0; i < blocksArray.Count; i++)
                {
                    if (blocksArray[i].Type != JTokenType.Array)
                    {
                        errors.Add($"地图 blocks 第 {i} 行必须是数组");
                        break;
                    }
                }
            }
        }

        if (!mapObject.ContainsKey("items") || mapObject["items"] == null)
            return;

        if (mapObject["items"].Type != JTokenType.Array)
        {
            errors.Add("地图 items 字段必须是二维字符串数组");
        }
        else
        {
            var itemsArray = mapObject["items"] as JArray;
            if (itemsArray == null || itemsArray.Count <= 0)
                return;
                
            for (int i = 0; i < itemsArray.Count; i++)
            {
                if (itemsArray[i].Type != JTokenType.Array)
                {
                    errors.Add($"地图 items 第 {i} 行必须是数组");
                    break;
                }

                var rowArray = itemsArray[i] as JArray;
                if (rowArray == null) continue;
                
                for (int j = 0; j < rowArray.Count; j++)
                {
                    if (rowArray[j].Type != JTokenType.String)
                    {
                        errors.Add($"地图 items[{i}][{j}] 必须是字符串");
                        break;
                    }
                }
            }
        }
    }

    private static void ValidateFeatures(JArray featuresArray, List<string> errors, List<string> warnings = null)
    {
        if (featuresArray == null)
        {
            errors.Add("features 字段必须是数组或对象");
            return;
        }

        if (featuresArray.Count == 0)
        {
            warnings?.Add("features 数组为空，将被忽略");
            return;
        }

        for (int i = 0; i < featuresArray.Count; i++)
        {
            if (featuresArray[i].Type != JTokenType.String && 
                featuresArray[i].Type != JTokenType.Object &&
                featuresArray[i].Type != JTokenType.Float &&
                featuresArray[i].Type != JTokenType.Integer)
            {
                warnings?.Add($"features 第 {i} 个元素格式不正确，已跳过");
            }
        }
    }

    private static void ValidateSpawn(JArray spawnArray, List<string> errors, List<string> warnings = null)
    {
        if (spawnArray == null)
        {
            errors.Add("spawn 字段必须是数组");
            return;
        }

        if (spawnArray.Count != 2)
        {
            warnings?.Add($"spawn 数组包含 {spawnArray.Count} 个元素而非2个，将被忽略");
            return;
        }

        for (int i = 0; i < spawnArray.Count; i++)
        {
            if (spawnArray[i].Type != JTokenType.Float && spawnArray[i].Type != JTokenType.Integer)
            {
                warnings?.Add($"spawn 第 {i} 个元素不是数字，将被忽略");
            }
        }
    }

    private static bool ValidateMapDataInObject(MapData mapData)
    {
        if (mapData.Blocks == null || mapData.Blocks.Length == 0)
        {
            Logger.LogWarning("地图中没有方块数据");
            return false;
        }

        var rowCount = mapData.Blocks.Length;
        var maxColCount = 0;
        
        foreach (var row in mapData.Blocks)
        {
            if (row != null && row.Length > maxColCount)
            {
                maxColCount = row.Length;
            }
        }

        if (maxColCount != 0) return true;
        Logger.LogWarning("地图行数据为空");
        return false;

    }
    
    private static void ValidateRequiredFieldWithDefault(JObject jsonObject, string fieldName, List<string> errors, List<string> warnings, string defaultValue)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            warnings.Add($"缺少必需字段: {fieldName}，已使用默认值 \"{defaultValue}\"");
            jsonObject[fieldName] = defaultValue;
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            warnings.Add($"字段为空: {fieldName}，已使用默认值 \"{defaultValue}\"");
            jsonObject[fieldName] = defaultValue;
        }
        else if (jsonObject[fieldName].Type == JTokenType.String &&
                 string.IsNullOrWhiteSpace(jsonObject[fieldName].ToString()))
        {
            warnings.Add($"字段为空字符串: {fieldName}，已使用默认值 \"{defaultValue}\"");
            jsonObject[fieldName] = defaultValue;
        }
    }

    private static void ValidateRequiredListFieldWithDefault(JObject jsonObject, string fieldName, List<string> errors, List<string> warnings, string defaultItem)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            warnings.Add($"缺少必需字段: {fieldName}，已使用默认值 [\"{defaultItem}\"]");
            jsonObject[fieldName] = new JArray(new[] { defaultItem });
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            warnings.Add($"字段为空: {fieldName}，已使用默认值 [\"{defaultItem}\"]");
            jsonObject[fieldName] = new JArray(new[] { defaultItem });
        }
        else if (jsonObject[fieldName].Type != JTokenType.Array)
        {
            warnings.Add($"{fieldName} 字段必须是数组，已转换为数组");
            var currentValue = jsonObject[fieldName].ToString();
            jsonObject[fieldName] = new JArray(new[] { currentValue });
        }
        else
        {
            var array = jsonObject[fieldName] as JArray;
            if (array == null || array.Count == 0)
            {
                warnings.Add($"{fieldName} 数组为空，已设置默认值");
                jsonObject[fieldName] = new JArray(new[] { defaultItem });
            }
        }
    }

    private static void ValidateOptionalListField(JObject jsonObject, string fieldName, List<string> warnings)
    {
        if (jsonObject.ContainsKey(fieldName) && jsonObject[fieldName] != null)
        {
            if (jsonObject[fieldName].Type != JTokenType.Array)
            {
                warnings.Add($"{fieldName} 字段必须是数组，已转换为数组");
                var currentValue = jsonObject[fieldName].ToString();
                jsonObject[fieldName] = new JArray(new[] { currentValue });
            }
            else
            {
                var array = jsonObject[fieldName] as JArray;
                if (array == null || array.Count == 0)
                {
                    warnings.Add($"{fieldName} 数组为空，已移除");
                    jsonObject[fieldName].Remove();
                }
            }
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
        if (string.IsNullOrWhiteSpace(result))
            return "unknown_fungame";
        
        return result;
    }

    private static bool IsValidId(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && id.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_');
    }

}