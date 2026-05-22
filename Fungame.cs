using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CustomFungamePack;

public class Fungame
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Version { get; set; }
    public List<string> Author { get; set; }
    public string Description { get; set; }
    public Feature Feature { get; set; } = new();
    public List<string> Command { get; set; }
    public float[] Spawn { get; set; } = [0, 0];
    [JsonProperty("map_data")]
    public MapData MapData { get; set; }
    [JsonProperty("custom_structures")]
    public string CustomStructures;
    
    public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";
    public string Features => Feature != null 
        ? $"Fullbright={Feature.Fullbright}, ForgivingLevel={Feature.ForgivingLevel}, Gravity={Feature.Gravity}, SkipTerrain={Feature.SkipTerrain}, SkipStructures={Feature.SkipStructures}, SkipBackground={Feature.SkipBackground}"
        : "None";
    public Vector2 SpawnPosition => new(
        Spawn is { Length: >= 2 } 
            ? Spawn[0]
            : 0, 
        Spawn is { Length: >= 2 } 
            ? Spawn[1]
            : 0);
}

[UsedImplicitly]
public class MapData
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public WorldGeneration.OverrideSceneType Type { get; set; } = WorldGeneration.OverrideSceneType.Debug;
    public string[] Map { get; set; } = [];
    public Dictionary<string, object> Key { get; set; } = new();
}

[UsedImplicitly]
public class Feature
{
    public bool  Fullbright = true;
    public bool  ForgivingLevel = false;
    public float Gravity = 0.3f;
    public bool  SkipTerrain = true;
    public bool  SkipStructures = true;
    public bool  SkipBackground = true;
}