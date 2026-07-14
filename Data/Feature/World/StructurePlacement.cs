using Newtonsoft.Json;

namespace CustomMap.Data.Feature.World;

public class StructurePlacement
{
    [JsonProperty("structure")] public string Structure { get; set; }

    [JsonProperty("x")] public int X { get; set; }

    [JsonProperty("y")] public int Y { get; set; }
}