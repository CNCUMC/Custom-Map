using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomMap.Data.Feature.World;

public class WorldSettingsData
{
    [JsonProperty("type")] public string Type { get; internal set; }
    [JsonProperty("full_bright")] public bool FullBright { get; set; } = true;
    [JsonProperty("forgiving_layer")] public bool Forgivinglayer { get; set; }
    [JsonProperty("gravity")] public float Gravity { get; set; } = Physics2D.gravity.y;
    [JsonProperty("jump_limit")] public int JumpLimit { get; set; }
    [JsonProperty("climb_limit")] public int ClimbLimit { get; set; }
    [JsonProperty("skip_terrain")] public bool SkipTerrain { get; set; } = true;
    [JsonProperty("skip_structures")] public bool SkipStructures { get; set; } = true;
    [JsonProperty("skip_background")] public bool SkipBackground { get; set; } = true;
    [JsonProperty("settings_overrides")] public Dictionary<string, object> SettingsOverrides { get; set; }
}
