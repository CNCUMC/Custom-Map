using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomFungamePack.Data;
using CustomFungamePack.Data.Feature.Player;
using CustomFungamePack.Data.Feature.World;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CustomFungamePack;

public class Fungame
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("author")] public List<string> Author { get; set; }
    [JsonProperty("description")] public string Description { get; set; }

    // ===== 运行时属性（从目录结构加载，不序列化到 fungame.json） =====

    [JsonIgnore] public string DirectoryPath { get; set; }

    [JsonIgnore] public List<LevelData> Levels { get; set; } = [];

    [JsonIgnore] public WorldSettingsData WorldSettings { get; set; } = new();

    [JsonIgnore] public CommandData CommandData { get; set; }

    [JsonIgnore] public XpData XpData { get; set; } = new();

    // Feature 数据对象（从 feature/world/*.json 和 feature/player/*.json 加载）
    [JsonIgnore] public MineData MineData { get; set; }
    [JsonIgnore] public JumpPadData JumpPadData { get; set; }
    [JsonIgnore] public TurretData TurretData { get; set; }
    [JsonIgnore] public SoundCannonData SoundCannonData { get; set; }
    [JsonIgnore] public SpikeStabberData SpikeStabberData { get; set; }
    [JsonIgnore] public GeyserData GeyserData { get; set; }
    [JsonIgnore] public BearTrapData BearTrapData { get; set; }

    // ===== 便捷属性 =====

    [JsonIgnore]
    public LevelData CurrentLevel => Levels.FirstOrDefault();

    [JsonIgnore]
    public Vector2 MapPosition => CurrentLevel is { } l ? new Vector2(l.X, l.Y) : Vector2.zero;

    [JsonIgnore]
    public Vector2 SpawnPosition => CurrentLevel is { Spawn: { Length: >= 2 } s }
        ? new Vector2(s[0], s[1])
        : Vector2.zero;

    [JsonIgnore]
    public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";

    // ===== 兼容属性（指向 CurrentLevel，过渡期使用） =====

    [JsonIgnore]
    public MapData MapData => CurrentLevel?.MapData;

    [JsonIgnore]
    public string CustomStructures => CurrentLevel?.CustomStructures;

    [JsonIgnore]
    public string BuildModeSave => CurrentLevel?.BuildModeSave;

    [JsonIgnore]
    public float[] Spawn => CurrentLevel?.Spawn;

    [JsonIgnore]
    public List<ItemData> Items => CurrentLevel?.Items;

    [JsonIgnore]
    public List<WaypointData> Waypoints => CurrentLevel?.Waypoints;

    [JsonIgnore]
    public int X => CurrentLevel?.X ?? 0;

    [JsonIgnore]
    public int Y => CurrentLevel?.Y ?? 0;

    [JsonIgnore]
    public bool SkipTerrain => WorldSettings?.SkipTerrain ?? true;

    [JsonIgnore]
    public bool SkipStructures => WorldSettings?.SkipStructures ?? true;

    [JsonIgnore]
    public bool SkipBackground => WorldSettings?.SkipBackground ?? true;

    [JsonIgnore]
    public WorldGeneration.OverrideSceneType Type => CurrentLevel?.SceneType ?? WorldGeneration.OverrideSceneType.Debug;

    // 旧 Feature 类属性（通过 WorldSettings 访问）
    // 这些由消费者代码直接使用 CurrentFungame.WorldSettings.xxx 或 Fungame.WorldSettings.xxx

    /// <summary>
    /// 获取所有启用的 Feature 描述字符串（用于日志显示）
    /// </summary>
    [JsonIgnore]
    public string ActiveFeatures
    {
        get
        {
            var items = new List<string>();

            if (WorldSettings?.Fullbright == true)
                items.Add("fullbright");

            if (WorldSettings?.ForgivingLevel == true)
                items.Add("forgiving_level");

            if (WorldSettings?.Gravity != Physics2D.gravity.y)
                items.Add($"gravity={WorldSettings?.Gravity}");

            if (MineData != null)
                items.Add("mine");

            if (JumpPadData != null)
                items.Add("jump_pad");

            if (TurretData != null)
                items.Add("turret");

            if (SoundCannonData != null)
                items.Add("sound_cannon");

            if (SpikeStabberData != null)
                items.Add("spike_stabber");

            if (GeyserData != null)
                items.Add("geyser");

            if (BearTrapData != null)
                items.Add("beartrap");

            return items.Count > 0 ? string.Join(", ", items) : "None";
        }
    }
}
