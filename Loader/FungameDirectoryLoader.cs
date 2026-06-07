using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomFungamePack.Data;
using CustomFungamePack.Data.Feature.Player;
using CustomFungamePack.Data.Feature.World;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomFungamePack.Loader;

public static class FungameDirectoryLoader
{
    public static Fungame LoadFromDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            return null;

        var fungameJsonPath = Path.Combine(directoryPath, "fungame.json");
        if (!File.Exists(fungameJsonPath))
            return null;

        try
        {
            var fungame = LoadJsonWithTypeCheck<Fungame>(fungameJsonPath, "fungame");
            if (fungame == null)
                return null;

            fungame.DirectoryPath = directoryPath;

            fungame.Levels = LoadLevels(directoryPath);

            fungame.WorldSettings = LoadWorldSettings(directoryPath) ?? new WorldSettingsData();

            fungame.MineData = LoadFeatureFile<MineData>(directoryPath, "world", "mine.json", "feature.world.mine");
            fungame.JumpPadData = LoadFeatureFile<JumpPadData>(directoryPath, "world", "jump_pad.json", "feature.world.jump_pad");
            fungame.TurretData = LoadFeatureFile<TurretData>(directoryPath, "world", "turret.json", "feature.world.turret");
            fungame.SoundCannonData = LoadFeatureFile<SoundCannonData>(directoryPath, "world", "sound_cannon.json", "feature.world.sound_cannon");
            fungame.SpikeStabberData = LoadFeatureFile<SpikeStabberData>(directoryPath, "world", "spike_stabber.json", "feature.world.spike_stabber");
            fungame.GeyserData = LoadFeatureFile<GeyserData>(directoryPath, "world", "geyser.json", "feature.world.geyser");
            fungame.BearTrapData = LoadFeatureFile<BearTrapData>(directoryPath, "world", "beartrap.json", "feature.world.beartrap");

            fungame.XpData = LoadFeatureFile<XpData>(directoryPath, "player", "xp.json", "feature.player.xp") ?? new XpData();

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
                var levelData = LoadJsonWithTypeCheck<LevelData>(levelFile, "level");
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

    private static T LoadJsonWithTypeCheck<T>(string filePath, string expectedType) where T : class
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var jObject = JObject.Parse(json);

            var actualType = jObject["type"]?.ToString();
            if (string.IsNullOrEmpty(actualType))
                return null;

            return !string.Equals(actualType, expectedType, StringComparison.OrdinalIgnoreCase)
                ? null
                : jObject.ToObject<T>();
        }
        catch
        {
            return null;
        }
    }
    
    public static void SaveToDirectory(Fungame fungame, string targetDirectory = null)
    {
        var directoryPath = targetDirectory ?? fungame.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
            return;

        // 从目录名生成 id（小写）
        var dirName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var originalId = fungame.Id;
        fungame.Id = dirName.ToLowerInvariant();

        var fungameJsonPath = Path.Combine(directoryPath, "fungame.json");

        SaveJsonWithTypeCheck(fungameJsonPath, fungame, "fungame");

        // 恢复原始 Id
        fungame.Id = originalId;

        if (fungame.Levels is { Count: > 0 })
        {
            var levelDir = Path.Combine(directoryPath, "level");
            Directory.CreateDirectory(levelDir);

            // 清理旧的关卡文件，防止重复
            foreach (var oldFile in Directory.GetFiles(levelDir, "*.json"))
            {
                try { File.Delete(oldFile); }
                catch { /* ignore */ }
            }

            for (var i = 0; i < fungame.Levels.Count; i++)
            {
                var levelPath = Path.Combine(levelDir, $"level{i + 1}.json");
                SaveJsonWithTypeCheck(levelPath, fungame.Levels[i], "level");
            }
        }

        if (fungame.WorldSettings != null)
            SaveFeatureFileToDisk(directoryPath, "world", "settings.json", fungame.WorldSettings, "feature.world.settings");

        SaveFeatureFileToDisk(directoryPath, "world", "mine.json", fungame.MineData, "feature.world.mine");
        SaveFeatureFileToDisk(directoryPath, "world", "jump_pad.json", fungame.JumpPadData, "feature.world.jump_pad");
        SaveFeatureFileToDisk(directoryPath, "world", "turret.json", fungame.TurretData, "feature.world.turret");
        SaveFeatureFileToDisk(directoryPath, "world", "sound_cannon.json", fungame.SoundCannonData, "feature.world.sound_cannon");
        SaveFeatureFileToDisk(directoryPath, "world", "spike_stabber.json", fungame.SpikeStabberData, "feature.world.spike_stabber");
        SaveFeatureFileToDisk(directoryPath, "world", "geyser.json", fungame.GeyserData, "feature.world.geyser");
        SaveFeatureFileToDisk(directoryPath, "world", "beartrap.json", fungame.BearTrapData, "feature.world.beartrap");

        if (fungame.XpData != null)
            SaveFeatureFileToDisk(directoryPath, "player", "xp.json", fungame.XpData, "feature.player.xp");

        Plugin.Logger?.LogInfo($"[FungameDirectoryLoader.Debug] SaveToDirectory calling SaveToCurrentLang: dir={directoryPath}, Name={fungame.Name}, Id={fungame.Id}");

        // 将 name/description/author 写入当前语言的 lang 文件
        FungameLocale.SaveToCurrentLang(fungame, directoryPath);

        Plugin.Logger?.LogInfo($"[FungameDirectoryLoader.Debug] SaveToDirectory after SaveToCurrentLang, CommandData is null? {fungame.CommandData == null}");

        if (fungame.CommandData == null) return;
        var commandPath = Path.Combine(directoryPath, "command.json");
        SaveJsonWithTypeCheck(commandPath, fungame.CommandData, "command");
    }

    private static void SaveJsonWithTypeCheck<T>(string filePath, T obj, string expectedType) where T : class
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var jObject = JObject.FromObject(obj, JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            jObject["type"] = expectedType;

            var json = JsonConvert.SerializeObject(jObject, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogWarning($"[SaveToDirectory] Failed to save {filePath}: {ex.Message}");
        }
    }

    private static void SaveFeatureFileToDisk<T>(string directoryPath, string subDir, string fileName, T obj, string expectedType) where T : class
    {
        if (obj == null)
            return;

        var filePath = Path.Combine(directoryPath, "feature", subDir, fileName);
        SaveJsonWithTypeCheck(filePath, obj, expectedType);
    }
}
