using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.World;

[UsedImplicitly]
public class BearTrapData
{
    [JsonProperty("type")] public string Type { get; internal set; }
    [JsonProperty("damage_mult")] public float DamageMult { get; set; } = 1f;
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 5f;
}