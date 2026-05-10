using System.Collections.Generic;

namespace CustomFungamePack;

public class Fungame
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Version { get; set; }
    public List<string> Author { get; set; }
    public string Description { get; set; }
    public List<string> Feature { get; set; }
    public MapData Map { get; set; }
}

public class MapData
{
    public int X { get; set; }
    public int Y { get; set; }
    public int[][] Blocks { get; set; } = [];
    public int[][] Items { get; set; } = [];
}