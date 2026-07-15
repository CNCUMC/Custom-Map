using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomMap.Data.Feature.World;

[UsedImplicitly]
public class BearTrapData
{
    [JsonProperty("damage_mult")] public float DamageMult { get; set; } = 1f;
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 5f;
}