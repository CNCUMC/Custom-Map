using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data;

[UsedImplicitly]
public class SpikeStabberData
{
    [JsonProperty("type")] public string Type { get; internal set; }

    [JsonProperty("damage_mult")] public float DamageMult { get; set; } = 1f;
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
    [JsonProperty("no_light")] public bool NoLight { get; set; }
    [JsonProperty("sound")] public string Sound { get; set; }
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 5f;
}
