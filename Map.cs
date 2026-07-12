using System.Collections.Generic;
using System.Linq;
using CustomMap.Data;
using CustomMap.Data.Feature.Player;
using CustomMap.Data.Feature.World;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomMap;

public class Map
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("author")] public List<string> Author { get; set; }
    [JsonProperty("description")] public string Description { get; set; }

    [JsonIgnore] public string DirectoryPath { get; set; }

    [JsonIgnore] public List<LevelData> Levels { get; set; } = [];

    [JsonIgnore] public WorldSettingsData WorldSettingsData { get; set; } = new();

    [JsonIgnore] public CommandData CommandData { get; set; }

    [JsonIgnore] public XpData XpData { get; set; } = new();

    [JsonIgnore] public MineData MineData { get; set; }
    [JsonIgnore] public JumpPadData JumpPadData { get; set; }
    [JsonIgnore] public TurretData TurretData { get; set; }
    [JsonIgnore] public SoundCannonData SoundCannonData { get; set; }
    [JsonIgnore] public SpikeStabberData SpikeStabberData { get; set; }
    [JsonIgnore] public GeyserData GeyserData { get; set; }
    [JsonIgnore] public BearTrapData BearTrapData { get; set; }

    [JsonIgnore] public int CurrentLevelIndex { get; set; }

    /// <summary>
    /// 存储该地图需要的、但未安装的模组列表
    /// </summary>
    [JsonIgnore] public IList<string> MissingMods { get; set; } = [];

    [JsonIgnore] public LevelData CurrentLevel => Levels.ElementAtOrDefault(CurrentLevelIndex);

    [JsonIgnore] public Vector2 MapPosition => CurrentLevel is { } l ? new Vector2(l.X, l.Y) : Vector2.zero;

    [JsonIgnore]
    public Vector2 SpawnPosition => CurrentLevel is { Spawn: { Length: >= 2 } s }
        ? new Vector2(s[0], s[1])
        : Vector2.zero;

    [JsonIgnore]
    public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";

    [JsonIgnore] public MapData MapData => CurrentLevel?.MapData;
    [JsonIgnore] public string CustomStructures => CurrentLevel?.CustomStructures;
    [JsonIgnore] public string BuildModeSave => CurrentLevel?.BuildModeSave;
    [JsonIgnore] public float[] Spawn => CurrentLevel?.Spawn;
    [JsonIgnore] public List<ItemData> Items => CurrentLevel?.Items;
    [JsonIgnore] public List<WaypointData> Waypoints => CurrentLevel?.Waypoints;
    [JsonIgnore] public int X => CurrentLevel?.X ?? 0;
    [JsonIgnore] public int Y => CurrentLevel?.Y ?? 0;
    [JsonIgnore] public bool SkipTerrain => WorldSettingsData?.SkipTerrain ?? true;
    [JsonIgnore] public bool SkipStructures => WorldSettingsData?.SkipStructures ?? true;
    [JsonIgnore] public bool SkipBackground => WorldSettingsData?.SkipBackground ?? true;

    [JsonIgnore]
    public WorldGeneration.OverrideSceneType Type => CurrentLevel?.SceneType ?? WorldGeneration.OverrideSceneType.Debug;

    [JsonIgnore]
    public string ActiveFeatures
    {
        get
        {
            var items = new List<string>();

            if (WorldSettingsData?.FullBright == true)
                items.Add("full_bright");

            if (WorldSettingsData?.ForgivingLevel == true)
                items.Add("forgiving_level");

            if (!Mathf.Approximately(WorldSettingsData?.Gravity ?? Physics2D.gravity.y, Physics2D.gravity.y))
                items.Add($"gravity={WorldSettingsData?.Gravity}");

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