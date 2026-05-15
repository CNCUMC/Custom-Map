using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomFungamePack;

public class Fungame
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Version { get; set; }
    public List<string> Author { get; set; }
    public string Description { get; set; }
    public Dictionary<string, float> Feature { get; set; } = new();
    public List<string> Command { get; set; }
    public float[] Spawn { get; set; } = [0, 0];
    [JsonProperty("map_data")] public MapData MapData { get; set; }

    public Vector2 SpawnPosition => new(Spawn is { Length: >= 2 } ? Spawn[0] : 0,
        Spawn != null && Spawn.Length >= 2 ? Spawn[1] : 0);

    public string Authors => Author is { Count: > 0 } ? string.Join(", ", Author) : "Unknown";
}

[UsedImplicitly]
public class MapData
{
    public int X { get; set; }
    public int Y { get; set; }
    public WorldGeneration.OverrideSceneType Type { get; set; } = WorldGeneration.OverrideSceneType.Debug;
    public bool SkipTerrain { get; set; } = true;
    public bool SkipStructures { get; set; } = true;
    public bool SkipBackground { get; set; } = true;
    public string[] Map { get; set; } = [];
    public Dictionary<string, object> Key { get; set; } = new();
}