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
            List<string> checkFailDirectories = [];
            Logger.LogInfo($"有效目录: {ValidDirectories.Count} 个，正在加载中...");
            foreach (var fungame in ValidDirectories.Where(fungame =>
                         !ValidateAndLoadFungame(Path.Combine(fungame, "fungame.json"))))
            {
                var folderName = Path.GetFileName(fungame);
                Logger.LogWarning($"{folderName} 识别失败!");
                checkFailDirectories.Add(fungame);
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
        var error = $"Fungames目录创建失败（权限不足）：{FungamesDirectory}，详情：";
        try
        {
            Directory.CreateDirectory(FungamesDirectory);
            Logger.LogInfo($"Fungames目录不存在，已创建：{FungamesDirectory}");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            Logger.LogError(error + ex);
        }
    }

private static bool ValidateAndLoadFungame(string filePath)
    {
        try
        {
            var jsonContent = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonContent);
            var errors = new List<string>();

            ValidateRequiredField(jsonObject, "name", errors);
            ValidateRequiredField(jsonObject, "id", errors);
            ValidateRequiredField(jsonObject, "version", errors);
            ValidateRequiredField(jsonObject, "author", errors);
            ValidateRequiredField(jsonObject, "description", errors);

            if (!errors.Contains("id") && jsonObject["id"] != null)
            {
                var idValue = jsonObject["id"].ToString();
                if (!IsValidId(idValue))
                {
                    errors.Add("id 字段只能包含小写字母、数字和下划线");
                }
            }

            if (!errors.Contains("Author") && jsonObject["Author"] != null)
            {
                if (jsonObject["Author"].Type != JTokenType.Array)
                {
                    errors.Add("Author 字段必须是字符串数组");
                }
                else
                {
                    var authorArray = jsonObject["Author"] as JArray;
                    if (authorArray == null || authorArray.Count == 0)
                    {
                        errors.Add("Author 数组不能为空");
                    }
                }
            }

            if (jsonObject.ContainsKey("map") && jsonObject["map"] != null)
            {
                ValidateMapData(jsonObject["map"] as JObject, errors);
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

            var fungame = JsonConvert.DeserializeObject<Fungame>(jsonContent);

            if (!IsValidVersion(fungame.Version))
            {
                Logger.LogWarning($"Fungame文件的Version格式不正确: {Path.GetFileName(filePath)}, Version: {fungame.Version}");
                return false;
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
            
            return true;
        }
        catch (Exception ex) when (ex is JsonException or UnauthorizedAccessException or IOException)
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
        return parts.Length is >= 2 and <= 4 && parts.All(part => int.TryParse(part, out _));
    }

    private static void ValidateMapData(JObject mapObject, List<string> errors)
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
    
    private static void ValidateRequiredField(JObject jsonObject, string fieldName, List<string> errors)
    {
        if (!jsonObject.ContainsKey(fieldName))
        {
            errors.Add($"缺少必需字段: {fieldName}");
        }
        else if (jsonObject[fieldName] == null || jsonObject[fieldName].Type == JTokenType.Null)
        {
            errors.Add($"字段不能为空: {fieldName}");
        }
        else if (jsonObject[fieldName].Type == JTokenType.String &&
                 string.IsNullOrWhiteSpace(jsonObject[fieldName].ToString()))
        {
            errors.Add($"字段不能为空字符串: {fieldName}");
        }
    }

    private static bool IsValidId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return id.All(c => char.IsLower(c) || char.IsDigit(c) || c == '_');
    }
}