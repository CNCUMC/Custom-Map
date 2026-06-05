using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data;

[UsedImplicitly]
public class MapData
{
    [JsonProperty("map")] public string[] Map { get; set; } = [];
    [JsonProperty("key")] public Dictionary<string, object> Key { get; set; } = new();
}
