using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomMap.Data;

[UsedImplicitly]
public class ItemData
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("slot")] public int Slot { get; set; }
    [JsonProperty("force")] public bool Force { get; set; }
}