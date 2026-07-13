using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using CustomMap.Data;
using CustomMap.Data.Feature.Player;
using CustomMap.Data.Feature.World;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomMap.Loader;

public static class CustomMapDirectoryLoader
{
    public static Map LoadFromDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            return null;

        var MapJsonPath = Path.Combine(directoryPath, "map.json");
        if (!File.Exists(MapJsonPath))
            return null;

        try
        {
            var map = LoadJsonWithTypeCheck<Map>(MapJsonPath, "Map");
            if (map == null)
                return null;

            map.DirectoryPath = directoryPath;

            map.Layers = LoadLayers(directoryPath);

            map.WorldSettingsData = LoadWorldSettings(directoryPath) ?? new WorldSettingsData();

            foreach (var (propName, subDir, fileName, typeCheck) in FeatureDescriptors)
            {
                var prop = typeof(Map).GetProperty(propName);
                if (prop == null) continue;
                var value = LoadFeatureFileByType(prop.PropertyType, directoryPath, subDir, fileName, typeCheck);
                if (value != null)
                    prop.SetValue(map, value);
            }

            map.XpData ??= new XpData();

            map.CommandData = LoadCommandData(directoryPath);

            return map;
        }
        catch
        {
            return null;
        }
    }

    private static List<LayerData> LoadLayers(string directoryPath)
    {
        var layersDir = Path.Combine(directoryPath, "layers");
        if (!Directory.Exists(layersDir))
            return [];

        var layerFiles = Directory.GetFiles(layersDir, "layer_*.json")
            .Select(f =>
            {
                var name = Path.GetFileNameWithoutExtension(f);
                var numStr = name.Substring("layer_".Length);
                var num = int.TryParse(numStr, out var n)
                    ? n
                    : int.MaxValue;
                return (Path: f, Num: num);
            })
            .OrderBy(x => x.Num)
            .Select(x => x.Path)
            .ToList();

        if (layerFiles.Count == 0)
            return [];

        var layers = new List<LayerData>();
        foreach (var layerFile in layerFiles)
            try
            {
                var layerData = LoadJsonWithTypeCheck<LayerData>(layerFile, "layer");
                if (layerData != null)
                    layers.Add(layerData);
            }
            catch
            {
                // ignore
            }

        return layers;
    }

    private static WorldSettingsData LoadWorldSettings(string directoryPath)
    {
        var settingsPath = Path.Combine(directoryPath, "feature", "world", "settings.json");
        return !File.Exists(settingsPath)
            ? null
            : LoadJsonWithTypeCheck<WorldSettingsData>(settingsPath, "feature.world.settings");
    }

    private static readonly (string PropName, string SubDir, string FileName, string TypeCheck)[] FeatureDescriptors =
    [
        (nameof(Map.MineData), "world", "mine.json", "feature.world.mine"),
        (nameof(Map.JumpPadData), "world", "jump_pad.json", "feature.world.jump_pad"),
        (nameof(Map.TurretData), "world", "turret.json", "feature.world.turret"),
        (nameof(Map.SoundCannonData), "world", "sound_cannon.json", "feature.world.sound_cannon"),
        (nameof(Map.SpikeStabberData), "world", "spike_stabber.json", "feature.world.spike_stabber"),
        (nameof(Map.GeyserData), "world", "geyser.json", "feature.world.geyser"),
        (nameof(Map.BearTrapData), "world", "beartrap.json", "feature.world.beartrap"),
        (nameof(Map.XpData), "player", "xp.json", "feature.player.xp"),
    ];

    private static object LoadFeatureFileByType(Type dataType, string directoryPath, string subDir, string fileName,
        string expectedType)
    {
        var method = typeof(CustomMapDirectoryLoader).GetMethod(nameof(LoadFeatureFile),
            BindingFlags.NonPublic | BindingFlags.Static);
        return method?.MakeGenericMethod(dataType).Invoke(null, [directoryPath, subDir, fileName, expectedType]);
    }

    private static T LoadFeatureFile<T>(string directoryPath, string subDir, string fileName, string expectedType)
        where T : class
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

    public static void SaveToDirectory(Map map, string targetDirectory = null)
    {
        var directoryPath = targetDirectory ?? map.DirectoryPath;
        if (string.IsNullOrEmpty(directoryPath))
            return;

        // 从目录名生成 id（小写）
        var dirName =
            Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var originalId = map.Id;
        map.Id = dirName.ToLowerInvariant();

        var MapJsonPath = Path.Combine(directoryPath, "map.json");

        SaveJsonWithTypeCheck(MapJsonPath, map, "Map");

        // 恢复原始 Id
        map.Id = originalId;

        if (map.Layers is { Count: > 0 })
        {
            var layersDir = Path.Combine(directoryPath, "layers");
            Directory.CreateDirectory(layersDir);

            // 清理旧的文件，防止重�?
            foreach (var oldFile in Directory.GetFiles(layersDir, "*.json"))
                try
                {
                    File.Delete(oldFile);
                }
                catch
                {
                    /* ignore */
                }

            for (var i = 0; i < map.Layers.Count; i++)
            {
                var layerPath = Path.Combine(layersDir, $"layer{i + 1}.json");
                SaveJsonWithTypeCheck(layerPath, map.Layers[i], "layer");
            }
        }

        if (map.WorldSettingsData != null)
            SaveFeatureFileToDisk(directoryPath, "world", "settings.json", map.WorldSettingsData,
                "feature.world.settings");

        SaveFeatureFileToDisk(directoryPath, "world", "mine.json", map.MineData, "feature.world.mine");
        SaveFeatureFileToDisk(directoryPath, "world", "jump_pad.json", map.JumpPadData, "feature.world.jump_pad");
        SaveFeatureFileToDisk(directoryPath, "world", "turret.json", map.TurretData, "feature.world.turret");
        SaveFeatureFileToDisk(directoryPath, "world", "sound_cannon.json", map.SoundCannonData,
            "feature.world.sound_cannon");
        SaveFeatureFileToDisk(directoryPath, "world", "spike_stabber.json", map.SpikeStabberData,
            "feature.world.spike_stabber");
        SaveFeatureFileToDisk(directoryPath, "world", "geyser.json", map.GeyserData, "feature.world.geyser");
        SaveFeatureFileToDisk(directoryPath, "world", "beartrap.json", map.BearTrapData, "feature.world.beartrap");

        if (map.XpData != null)
            SaveFeatureFileToDisk(directoryPath, "player", "xp.json", map.XpData, "feature.player.xp");

        Plugin.Logger?.LogInfo(
            $"[CustomMapDirectoryLoader.Debug] SaveToDirectory calling SaveToCurrentLang: dir={directoryPath}, Name={map.Name}, Id={map.Id}");

        // �?name/description/author 写入当前语言�?lang 文件
        MapLocale.SaveToCurrentLang(map, directoryPath);

        Plugin.Logger?.LogInfo(
            $"[MapDirectoryLoader.Debug] SaveToDirectory after SaveToCurrentLang, CommandData is null? {map.CommandData == null}");

        if (map.CommandData == null) return;
        var commandPath = Path.Combine(directoryPath, "command.json");
        SaveJsonWithTypeCheck(commandPath, map.CommandData, "command");
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

    private static void SaveFeatureFileToDisk<T>(string directoryPath, string subDir, string fileName, T obj,
        string expectedType) where T : class
    {
        if (obj == null)
            return;

        var filePath = Path.Combine(directoryPath, "feature", subDir, fileName);
        SaveJsonWithTypeCheck(filePath, obj, expectedType);
    }
}

