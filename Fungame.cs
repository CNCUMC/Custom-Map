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
    public Waypoint Waypoint { get; set; }
    public List<Waypoint> Waypoints { get; set; } = [];
    public float[] Spawn { get; set; } = [0, 0];
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public Vector2 MapPosition => new(X, Y);
    [JsonProperty("map_data")] public MapData MapData { get; set; }
    [JsonProperty("custom_structures")] public string CustomStructures;
    [JsonProperty("build_mode_save")] public string BuildModeSave;

    [JsonProperty("skip_terrain")] public bool SkipTerrain { get; set; } = true;
    [JsonProperty("skip_structures")] public bool SkipStructures { get; set; } = true;
    [JsonProperty("skip_background")] public bool SkipBackground { get; set; } = true;

    [JsonIgnore] public string DirectoryPath { get; set; }

    [JsonIgnore] public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";

    [JsonIgnore] public string Features => Feature != null
        ? string.Join(", ", typeof(Feature).GetProperties().Select(prop =>
            $"{prop.Name}={prop.GetValue(Feature)}"))
        : "None";

    [JsonIgnore] public Vector2 SpawnPosition => new(
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
    public WorldGeneration.OverrideSceneType Type { get; set; } = WorldGeneration.OverrideSceneType.Debug;
    public string[] Map { get; set; } = [];
    public Dictionary<string, object> Key { get; set; } = new();
}

[UsedImplicitly]
public class Feature
{
    public bool Fullbright { get; set; } = true;
    public bool ForgivingLevel { get; set; }
    public float Gravity { get; set; } = Physics2D.gravity.y;
    public int JumpLimit { get; set; } = 0;
    public int ClimbLimit { get; set; } = 0;
}

[UsedImplicitly]
public class CommandData
{
    [JsonProperty("once_commands")] public List<string> OnceCommands;
    [JsonProperty("loop_commands")] public List<string> LoopCommands;
    [JsonProperty("loop_interval")] public float LoopInterval;
}

[UsedImplicitly]
public class Waypoint
{
    public string Id { get; set; }
    public Vector2 Position => new(X, Y);
    public float X { get; set; }
    public float Y { get; set; }
}