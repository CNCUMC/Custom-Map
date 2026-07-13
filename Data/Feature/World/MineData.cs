using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomMap.Data.Feature.World;

[UsedImplicitly]
public class MineData
{
    [JsonProperty("type")] public string Type { get; internal set; }
    [JsonProperty("undestroy")] public bool Undestroy { get; set; }
    [JsonProperty("explosion_params")] public ExplosionParamsData ExplosionParamsData { get; set; }
    [JsonProperty("cooldown")] public float Cooldown { get; set; } = 0.8f;
}