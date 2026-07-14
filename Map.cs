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
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("author")] public List<string> Author { get; set; }

    [JsonIgnore] public string DirectoryPath { get; set; }

    [JsonIgnore] public List<LayerData> Layers { get; set; } = [];

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

    [JsonIgnore] public int CurrentLayerIndex { get; set; }

    [JsonIgnore] public IList<string> MissingMods { get; } = [];

    [JsonIgnore] public LayerData CurrentLayer => Layers.ElementAtOrDefault(CurrentLayerIndex);

    [JsonIgnore] public Vector2 MapPosition => CurrentLayer is { } l ? new Vector2(l.X, l.Y) : Vector2.zero;

    [JsonIgnore]
    public Vector2 SpawnPosition => CurrentLayer is { Spawn: { Length: >= 2 } s }
        ? new Vector2(s[0], s[1])
        : Vector2.zero;

    [JsonIgnore]
    public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";

    [JsonIgnore] public List<StructurePlacement> Structures => CurrentLayer?.Structures;
    [JsonIgnore] public string BuildModeSave => CurrentLayer?.BuildModeSave;
    [JsonIgnore] public float[] Spawn => CurrentLayer?.Spawn;
    [JsonIgnore] public List<ItemData> Items => CurrentLayer?.Items;
    [JsonIgnore] public List<WaypointData> Waypoints => CurrentLayer?.Waypoints;
    [JsonIgnore] public int X => CurrentLayer?.X ?? 0;
    [JsonIgnore] public int Y => CurrentLayer?.Y ?? 0;
    [JsonIgnore] public bool SkipTerrain => CurrentLayer?.SkipTerrain ?? true;
    [JsonIgnore] public bool SkipStructures => CurrentLayer?.SkipStructures ?? true;
    [JsonIgnore] public bool SkipBackground => CurrentLayer?.SkipBackground ?? true;

    [JsonIgnore]
    public WorldGeneration.OverrideSceneType Type => CurrentLayer?.SceneType ?? WorldGeneration.OverrideSceneType.Debug;

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

            return items.Count > 0
                ? string.Join(", ", items)
                : "None";
        }
    }
}