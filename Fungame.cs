using System.Collections.Generic;
using System.Linq;
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
    public CommandData CommandData { get; set; }
    public float[] Spawn { get; set; } = [0, 0];
    [JsonProperty("map_data")] public MapData MapData { get; set; }
    [JsonProperty("custom_structures")] public string CustomStructures;
    [JsonProperty("skip_terrain")] public bool SkipTerrain { get; set; } = true;
    [JsonProperty("skip_structures")] public bool SkipStructures { get; set; } = true;
    [JsonProperty("skip_background")] public bool SkipBackground { get; set; } = true;

    public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";

    public string Features => Feature != null
        ? string.Join(", ", typeof(Feature).GetProperties().Select(prop =>
            $"{prop.Name}={prop.GetValue(Feature)}"))
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
    public bool Fullbright { get; set; } = true;
    public bool ForgivingLevel { get; set; } = false;
    public float Gravity { get; set; } = Physics2D.gravity.y;
}

[UsedImplicitly]
public class CommandData
{
    [JsonProperty("once_commands")] public List<string> OnceCommands;
    [JsonProperty("loop_commands")] public List<string> LoopCommands;
    [JsonProperty("loop_interval")] public float LoopInterval;
}