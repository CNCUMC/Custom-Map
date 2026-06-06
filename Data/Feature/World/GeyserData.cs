using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.World
{
    [UsedImplicitly]
    public class GeyserData
    {
        [JsonProperty("type")] public string Type { get; internal set; }
        [JsonProperty("cooldown")] public float Cooldown { get; set; } = 10f;
        [JsonProperty("activate_duration")] public float ActivateDuration { get; set; } = 4.5f;
        [JsonProperty("rumble_time")] public float RumbleTime { get; set; } = 1f;
        [JsonProperty("range")] public float Range { get; set; } = 64f;
        [JsonProperty("no_liquid")] public bool NoLiquid { get; set; }
    }
}
