using System.Collections.Generic;
using CustomMap.Data;
using CustomMap.Data.Feature.World;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace CustomMap;

public class Layer
{
    [JsonProperty("structures", NullValueHandling = NullValueHandling.Ignore)]
    public List<StructureData> Structures { get; set; } = [];
    [JsonProperty("build_mode_save", NullValueHandling = NullValueHandling.Ignore)]
    public string BuildModeSave { get; set; }

    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public WorldGeneration.OverrideSceneType Type { get; set; } = WorldGeneration.OverrideSceneType.Debug;

    [JsonProperty("spawn")] public float[] Spawn { get; set; } = [0, 0];
    [JsonProperty("x")] public int X { get; set; }
    [JsonProperty("y")] public int Y { get; set; }
    [JsonProperty("waypoints")] public List<WaypointData> Waypoints { get; set; } = [];
    [JsonProperty("items")] public List<ItemData> Items { get; set; } = [];

    [JsonIgnore]
    public Vector2 SpawnPosition => Spawn is { Length: >= 2 } ? new Vector2(Spawn[0], Spawn[1]) : Vector2.zero;

    [JsonProperty("skip_terrain")] public bool SkipTerrain { get; set; } = true;
    [JsonProperty("skip_structures")] public bool SkipStructures { get; set; } = true;
    [JsonProperty("skip_background")] public bool SkipBackground { get; set; } = true;
    [JsonIgnore] public MineData MineData { get; set; }
    [JsonIgnore] public JumpPadData JumpPadData { get; set; }
    [JsonIgnore] public TurretData TurretData { get; set; }
    [JsonIgnore] public SoundCannonData SoundCannonData { get; set; }
    [JsonIgnore] public SpikeStabberData SpikeStabberData { get; set; }
    [JsonIgnore] public GeyserData GeyserData { get; set; }
    [JsonIgnore] public BearTrapData BearTrapData { get; set; }
    [JsonIgnore] public WorldSettingsData WorldSettingsData { get; set; } = new();
    [JsonIgnore] public CommandData CommandData { get; set; }

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