using System.Collections.Generic;
using JetBrains.Annotations;
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
    public Vector2 Spawn { get; set; } = new(0, 0);
    public MapData Map { get; set; }
}

[UsedImplicitly]
public class MapData
{
    public int X { get; set; }
    public int Y { get; set; }
    public int[][] Blocks { get; set; } = [];
    public string[][] Items { get; set; } = [];
    public WorldGeneration.OverrideSceneType Type { get; set; } = WorldGeneration.OverrideSceneType.Debug;
    public bool SkipTerrain { get; set; } = true;
    public bool SkipStructures { get; set; } = true;
    public bool SkipBackground { get; set; } = true;
}