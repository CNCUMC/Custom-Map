using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.World;

[UsedImplicitly]
public class SoundCannonData
{
    [JsonProperty("type")] public string Type { get; internal set; }
    [JsonProperty("max_distance")] public float MaxDistance { get; set; } = 50f;
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 5f;
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
}
