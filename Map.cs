using System.Collections.Generic;
using System.Linq;
using CustomMap.Data.Feature.Player;
using Newtonsoft.Json;

namespace CustomMap;

public class Map
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("author")] public List<string> Author { get; set; }
    [JsonIgnore] public string DirectoryPath { get; set; }
    [JsonIgnore] public List<Layer> Layers { get; set; } = [];
    [JsonIgnore] public XpData XpData { get; set; } = new();
    [JsonIgnore] public int CurrentLayerIndex { get; set; }
    [JsonIgnore] public IList<string> MissingMods { get; } = [];
    [JsonIgnore] public Layer CurrentLayer => Layers.ElementAtOrDefault(CurrentLayerIndex);

    [JsonIgnore]
    public string Authors => Author is { Count: > 0 }
        ? string.Join(", ", Author)
        : "Unknown";
}