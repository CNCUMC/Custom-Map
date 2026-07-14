using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace CustomMap.Data.Feature.World;

public class LayerData
{
    [JsonProperty("structures", NullValueHandling = NullValueHandling.Ignore)]
    public List<StructurePlacement> Structures { get; set; } = [];

    [JsonProperty("build_mode_save", NullValueHandling = NullValueHandling.Ignore)]
    public string BuildModeSave { get; set; }

    [JsonProperty("type")] public string Type { get; internal set; }

    [JsonProperty("scene_type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public WorldGeneration.OverrideSceneType SceneType { get; set; } = WorldGeneration.OverrideSceneType.Debug;

    [JsonProperty("spawn")] public float[] Spawn { get; set; } = [0, 0];
    [JsonProperty("x")] public int X { get; set; }
    [JsonProperty("y")] public int Y { get; set; }
    [JsonProperty("waypoints")] public List<WaypointData> Waypoints { get; set; } = [];
    [JsonProperty("items")] public List<ItemData> Items { get; set; } = [];
    [JsonProperty("skip_terrain")] public bool SkipTerrain { get; set; } = true;
    [JsonProperty("skip_structures")] public bool SkipStructures { get; set; } = true;
    [JsonProperty("skip_background")] public bool SkipBackground { get; set; } = true;

    [JsonIgnore]
    public Vector2 SpawnPosition => Spawn is { Length: >= 2 } ? new Vector2(Spawn[0], Spawn[1]) : Vector2.zero;
}