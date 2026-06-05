using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomFungamePack.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomFungamePack.Loader;

public static class FungameDirectoryLoader
{
    /// <summary>
    /// 从新版目录结构加载 Fungame。
    /// 目录结构：
    /// FungameName/
    ///   fungame.json           - 元数据（name, id, version, author, description）
    ///   level/                 - 关卡数据
    ///     l1.json              - 包含 map_data, custom_structures, build_mode_save, scene_type, spawn, x, y, waypoints, items
    ///   feature/
    ///     world/
    ///       settings.json      - WorldSettings（fullbright, forgiving_level, gravity 等）
    ///       mine.json          - MineData
    ///       jump_pad.json      - JumpPadData
    ///       turret.json        - TurretData
    ///       sound_cannon.json  - SoundCannonData
    ///       spike_stabber.json - SpikeStabberData
    ///       geyser.json        - GeyserData
    ///       beartrap.json      - BearTrapData
    ///     player/
    ///       xp.json            - XpData
    ///   command.json           - CommandData
    /// </summary>
    public static Fungame LoadFromDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            return null;

        var fungameJsonPath = Path.Combine(directoryPath, "fungame.json");
        if (!File.Exists(fungameJsonPath))
            return null;

        try
        {
            // 1. 加载元数据
            var fungame = LoadJsonWithTypeCheck<Fungame>(fungameJsonPath, "meta.fungame");
            if (fungame == null)
                return null;

            fungame.DirectoryPath = directoryPath;

            // 2. 加载关卡数据
            fungame.Levels = LoadLevels(directoryPath);

            // 3. 加载世界设置
            fungame.WorldSettings = LoadWorldSettings(directoryPath) ?? new WorldSettingsData();

            // 4. 加载 Feature 数据
            fungame.MineData = LoadFeatureFile<MineData>(directoryPath, "world", "mine.json", "feature.world.mine");
            fungame.JumpPadData = LoadFeatureFile<JumpPadData>(directoryPath, "world", "jump_pad.json", "feature.world.jump_pad");
            fungame.TurretData = LoadFeatureFile<TurretData>(directoryPath, "world", "turret.json", "feature.world.turret");
            fungame.SoundCannonData = LoadFeatureFile<SoundCannonData>(directoryPath, "world", "sound_cannon.json", "feature.world.sound_cannon");
            fungame.SpikeStabberData = LoadFeatureFile<SpikeStabberData>(directoryPath, "world", "spike_stabber.json", "feature.world.spike_stabber");
            fungame.GeyserData = LoadFeatureFile<GeyserData>(directoryPath, "world", "geyser.json", "feature.world.geyser");
            fungame.BearTrapData = LoadFeatureFile<BearTrapData>(directoryPath, "world", "beartrap.json", "feature.world.beartrap");

            // 5. 加载玩家数据
            fungame.XpData = LoadFeatureFile<XpData>(directoryPath, "player", "xp.json", "feature.player.xp") ?? new XpData();

            // 6. 加载命令数据
            fungame.CommandData = LoadCommandData(directoryPath);

            return fungame;
        }
        catch
        {
            return null;
        }
    }

    private static List<LevelData> LoadLevels(string directoryPath)
    {
        var levelDir = Path.Combine(directoryPath, "level");
        if (!Directory.Exists(levelDir))
            return [];

        var levelFiles = Directory.GetFiles(levelDir, "*.json")
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (levelFiles.Count == 0)
            return [];

        var levels = new List<LevelData>();
        foreach (var levelFile in levelFiles)
        {
            try
            {
                // Level 文件的 type 格式: "level.{fileName}" (e.g. "level.l1", "level.l2")
                var fileName = Path.GetFileNameWithoutExtension(levelFile);
                var expectedType = $"level.{fileName}";
                var levelData = LoadJsonWithTypeCheck<LevelData>(levelFile, expectedType);
                if (levelData != null)
                    levels.Add(levelData);
            }
            catch
            {
                // ignore
            }
        }

        return levels;
    }

    private static WorldSettingsData LoadWorldSettings(string directoryPath)
    {
        var settingsPath = Path.Combine(directoryPath, "feature", "world", "settings.json");
        return !File.Exists(settingsPath)
            ? null
            : LoadJsonWithTypeCheck<WorldSettingsData>(settingsPath, "feature.world.settings");
    }

    private static T LoadFeatureFile<T>(string directoryPath, string subDir, string fileName, string expectedType) where T : class
    {
        var filePath = Path.Combine(directoryPath, "feature", subDir, fileName);
        return !File.Exists(filePath)
            ? null 
            : LoadJsonWithTypeCheck<T>(filePath, expectedType);
    }

    private static CommandData LoadCommandData(string directoryPath)
    {
        var commandPath = Path.Combine(directoryPath, "command.json");
        return !File.Exists(commandPath)
            ? null
            : LoadJsonWithTypeCheck<CommandData>(commandPath, "command");
    }

    /// <summary>
    /// 读取 JSON 文件，先检查 type 字段是否匹配预期值，再反序列化。
    /// </summary>
    private static T LoadJsonWithTypeCheck<T>(string filePath, string expectedType) where T : class
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var jObject = JObject.Parse(json);

            var actualType = jObject["type"]?.ToString();
            if (string.IsNullOrEmpty(actualType))
                return null;

            if (!string.Equals(actualType, expectedType, StringComparison.OrdinalIgnoreCase))
                return null;

            return jObject.ToObject<T>();
        }
        catch
        {
            return null;
        }
    }

    // =========================== 保存到目录 ===========================

    /// <summary>
    /// 将 Fungame 的所有数据保存到目录结构中。
    /// 目录结构同 LoadFromDirectory 的文档说明。
    /// 如果提供了 targetDirectory，则保存到该目录；否则使用 fungame.DirectoryPath。
    /// </summary>
    public static void SaveToDirectory(Fungame fungame, string targetDirectory = null)
    {
        var directoryPath = targetDirectory ?? fungame.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
            return;

        // 1. 保存元数据到 fungame.json
        var fungameJsonPath = Path.Combine(directoryPath, "fungame.json");
        SaveJsonWithTypeCheck(fungameJsonPath, fungame, "meta.fungame");

        // 2. 保存关卡数据
        if (fungame.Levels is { Count: > 0 })
        {
            var levelDir = Path.Combine(directoryPath, "level");
            Directory.CreateDirectory(levelDir);

            for (var i = 0; i < fungame.Levels.Count; i++)
            {
                var levelPath = Path.Combine(levelDir, $"l{i + 1}.json");
                var expectedType = $"level.l{i + 1}";
                SaveJsonWithTypeCheck(levelPath, fungame.Levels[i], expectedType);
            }
        }

        // 3. 保存世界设置
        if (fungame.WorldSettings != null)
            SaveFeatureFileToDisk(directoryPath, "world", "settings.json", fungame.WorldSettings, "feature.world.settings");

        // 4. 保存 Feature 数据
        SaveFeatureFileToDisk(directoryPath, "world", "mine.json", fungame.MineData, "feature.world.mine");
        SaveFeatureFileToDisk(directoryPath, "world", "jump_pad.json", fungame.JumpPadData, "feature.world.jump_pad");
        SaveFeatureFileToDisk(directoryPath, "world", "turret.json", fungame.TurretData, "feature.world.turret");
        SaveFeatureFileToDisk(directoryPath, "world", "sound_cannon.json", fungame.SoundCannonData, "feature.world.sound_cannon");
        SaveFeatureFileToDisk(directoryPath, "world", "spike_stabber.json", fungame.SpikeStabberData, "feature.world.spike_stabber");
        SaveFeatureFileToDisk(directoryPath, "world", "geyser.json", fungame.GeyserData, "feature.world.geyser");
        SaveFeatureFileToDisk(directoryPath, "world", "beartrap.json", fungame.BearTrapData, "feature.world.beartrap");

        // 5. 保存玩家数据
        if (fungame.XpData != null)
            SaveFeatureFileToDisk(directoryPath, "player", "xp.json", fungame.XpData, "feature.player.xp");

        // 6. 保存命令数据
        if (fungame.CommandData != null)
        {
            var commandPath = Path.Combine(directoryPath, "command.json");
            SaveJsonWithTypeCheck(commandPath, fungame.CommandData, "command");
        }
    }

    /// <summary>
    /// 将对象保存为 JSON 文件，并注入 type 字段。
    /// </summary>
    private static void SaveJsonWithTypeCheck<T>(string filePath, T obj, string expectedType) where T : class
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            // 序列化为 JObject 并注入 type 字段
            var jObject = JObject.FromObject(obj, new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
            jObject["type"] = expectedType;

            File.WriteAllText(filePath, jObject.ToString(Formatting.Indented));
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogWarning($"[SaveToDirectory] Failed to save {filePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// 将 Feature 数据保存到 feature/{subDir}/{fileName} 路径。
    /// 如果 obj 为 null，则跳过（不创建文件）。
    /// </summary>
    private static void SaveFeatureFileToDisk<T>(string directoryPath, string subDir, string fileName, T obj, string expectedType) where T : class
    {
        if (obj == null)
            return;

        var filePath = Path.Combine(directoryPath, "feature", subDir, fileName);
        SaveJsonWithTypeCheck(filePath, obj, expectedType);
    }
}
