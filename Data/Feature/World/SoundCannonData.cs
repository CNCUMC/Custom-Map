using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomMap.Data.Feature.World;

[UsedImplicitly]
public class SoundCannonData
{
    [JsonProperty("max_distance")] public float MaxDistance { get; set; } = 50f;
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 5f;
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
}