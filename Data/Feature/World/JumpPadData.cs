using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.World;

[UsedImplicitly]
public class JumpPadData
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 15f;
    [JsonProperty("no_light")] public bool NoLight { get; set; }
    [JsonProperty("force")] public float Force { get; set; } = 1f;
}
