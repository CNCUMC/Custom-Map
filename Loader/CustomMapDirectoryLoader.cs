using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomMap.Data;
using CustomMap.Data.Feature.Player;
using CustomMap.Data.Feature.World;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomMap.Loader;

public static class CustomMapDirectoryLoader
{
    private static readonly (string PropName, string SubDir, string FileName)[] FeatureDescriptors =
    [
        (nameof(Map.CurrentLayer.MineData), "world", "mine.json"),
        (nameof(Map.CurrentLayer.JumpPadData), "world", "jump_pad.json"),
        (nameof(Map.CurrentLayer.TurretData), "world", "turret.json"),
        (nameof(Map.CurrentLayer.SoundCannonData), "world", "sound_cannon.json"),
        (nameof(Map.CurrentLayer.SpikeStabberData), "world", "spike_stabber.json"),
        (nameof(Map.CurrentLayer.GeyserData), "world", "geyser.json"),
        (nameof(Map.CurrentLayer.BearTrapData), "world", "beartrap.json"),
        (nameof(Map.XpData), "player", "xp.json")
    ];

    public static Map LoadFromDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            return null;

        var MapJsonPath = Path.Combine(directoryPath, "map.json");
        if (!File.Exists(MapJsonPath))
            return null;

        try
        {
            var map = LoadJson<Map>(MapJsonPath);
            if (map == null)
                return null;

            map.DirectoryPath = directoryPath;

            // 先加载地图根目录的 command.json 作为全局回退
            var mapCommandData = LoadCommandData(directoryPath);

            map.Layers = LoadLayers(directoryPath);

            if (map.Layers.Count == 0)
                map.Layers.Add(new Layer());

            // 为所有没有独立 command.json 的层级应用地图级命令
            if (mapCommandData != null)
                foreach (var layer in map.Layers)
                    layer.CommandData ??= mapCommandData;

            map.CurrentLayer.WorldSettingsData = LoadWorldSettings(directoryPath) ?? new WorldSettingsData();

            foreach (var (propName, subDir, fileName) in FeatureDescriptors)
            {
                var prop = typeof(Map).GetProperty(propName);
                if (prop == null) continue;
                var value = LoadFeatureFileByType(prop.PropertyType, directoryPath, subDir, fileName);
                if (value != null)
                    prop.SetValue(map, value);
            }

            map.XpData ??= new XpData();

            return map;
        }
        catch
        {
            return null;
        }
    }

    private static List<Layer> LoadLayers(string directoryPath)
    {
        var layersDir = Path.Combine(directoryPath, "layers");
        if (!Directory.Exists(layersDir))
            return [];

        // 扫描 layers/ 下的子目录（layer1/, layer2/, ...）
        var layerDirs = Directory.GetDirectories(layersDir, "layer*")
            .Select(d =>
            {
                var name = Path.GetFileName(d);
                var numStr = name.Substring("layer".Length);
                var num = int.TryParse(numStr, out var n)
                    ? n
                    : int.MaxValue;
                return (Path: d, Num: num);
            })
            .OrderBy(x => x.Num)
            .Select(x => x.Path)
            .ToList();

        if (layerDirs.Count == 0)
            return [];

        var layers = new List<Layer>();
        foreach (var layerDir in layerDirs)
            try
            {
                var layerJsonPath = Path.Combine(layerDir, "layer.json");
                if (!File.Exists(layerJsonPath))
                    continue;

                var layerData = LoadJson<Layer>(layerJsonPath);
                if (layerData == null)
                    continue;

                // 加载 command.json（从层级目录）
                var commandPath = Path.Combine(layerDir, "command.json");
                if (File.Exists(commandPath))
                    layerData.CommandData = LoadJson<CommandData>(commandPath);

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
            : LoadJson<WorldSettingsData>(settingsPath);
    }

    private static object LoadFeatureFileByType(Type dataType, string directoryPath, string subDir, string fileName)
    {
        var method = typeof(CustomMapDirectoryLoader).GetMethod(nameof(LoadFeatureFile),
            BindingFlags.NonPublic | BindingFlags.Static);
        return method?.MakeGenericMethod(dataType).Invoke(null, [directoryPath, subDir, fileName]);
    }

    private static T LoadFeatureFile<T>(string directoryPath, string subDir, string fileName)
        where T : class
    {
        var filePath = Path.Combine(directoryPath, "feature", subDir, fileName);
        return !File.Exists(filePath)
            ? null
            : LoadJson<T>(filePath);
    }

    private static CommandData LoadCommandData(string directoryPath)
    {
        var commandPath = Path.Combine(directoryPath, "command.json");
        return !File.Exists(commandPath)
            ? null
            : LoadJson<CommandData>(commandPath);
    }

    private static T LoadJson<T>(string filePath) where T : class
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var jObject = JObject.Parse(json);

            return jObject.ToObject<T>();
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

        SaveJsonWithTypeCheck(MapJsonPath, map, "map");

        // 恢复原始 Id
        map.Id = originalId;

        if (map.Layers is { Count: > 0 })
        {
            var layersDir = Path.Combine(directoryPath, "layers");
            Directory.CreateDirectory(layersDir);

            // 清理旧的层级文件和目录
            foreach (var oldDir in Directory.GetDirectories(layersDir, "layer*"))
                try
                {
                    Directory.Delete(oldDir, true);
                }
                catch
                {
                    /* ignore */
                }

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
                var layerDir = Path.Combine(layersDir, $"layer{i + 1}");
                Directory.CreateDirectory(layerDir);

                // 保存 layer.json
                var layerJsonPath = Path.Combine(layerDir, "layer.json");
                SaveJsonWithTypeCheck(layerJsonPath, map.Layers[i], "layer");

                // 保存 command.json（如果有）
                if (map.Layers[i].CommandData != null)
                {
                    var commandPath = Path.Combine(layerDir, "command.json");
                    SaveJsonWithTypeCheck(commandPath, map.Layers[i].CommandData, "command");
                }

            }
        }

        if (map.CurrentLayer.WorldSettingsData != null)
            SaveFeatureFileToDisk(directoryPath, "world", "settings.json", map.CurrentLayer.WorldSettingsData,
                "feature.world.settings");

        SaveFeatureFileToDisk(directoryPath, "world", "mine.json", map.CurrentLayer.MineData, "feature.world.mine");
        SaveFeatureFileToDisk(directoryPath, "world", "jump_pad.json", map.CurrentLayer.JumpPadData,
            "feature.world.jump_pad");
        SaveFeatureFileToDisk(directoryPath, "world", "turret.json", map.CurrentLayer.TurretData,
            "feature.world.turret");
        SaveFeatureFileToDisk(directoryPath, "world", "sound_cannon.json", map.CurrentLayer.SoundCannonData,
            "feature.world.sound_cannon");
        SaveFeatureFileToDisk(directoryPath, "world", "spike_stabber.json", map.CurrentLayer.SpikeStabberData,
            "feature.world.spike_stabber");
        SaveFeatureFileToDisk(directoryPath, "world", "geyser.json", map.CurrentLayer.GeyserData,
            "feature.world.geyser");
        SaveFeatureFileToDisk(directoryPath, "world", "beartrap.json", map.CurrentLayer.BearTrapData,
            "feature.world.beartrap");

        if (map.XpData != null)
            SaveFeatureFileToDisk(directoryPath, "player", "xp.json", map.XpData, "feature.player.xp");

        Plugin.Logger?.LogInfo(
            $"[CustomMapDirectoryLoader.Debug] SaveToDirectory calling SaveToCurrentLang: dir={directoryPath}, Name={MapLocale.GetName(map)}, Id={map.Id}");

        // name/description/author 写入当前语言 lang 文件
        MapLocale.SaveToCurrentLang(map, directoryPath);

        Plugin.Logger?.LogInfo(
            $"[MapDirectoryLoader.Debug] SaveToDirectory after SaveToCurrentLang, CommandData is null? {map.CurrentLayer.CommandData == null}");
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