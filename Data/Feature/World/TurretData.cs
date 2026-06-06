using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.World;

[UsedImplicitly]
public class TurretData
{
    [JsonProperty("type")] public string Type { get; internal set; }
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 15f;
    [JsonProperty("shot_power_multiplier")] public float ShotPowerMultiplier { get; set; } = 1f;
    [JsonProperty("range")] public float Range { get; set; } = 100f;
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
    [JsonProperty("no_light")] public bool NoLight { get; set; }
    [JsonProperty("explosion_params")] public ExplosionParamsData ExplosionParamsData { get; set; }
}