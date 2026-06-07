using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomFungamePack.Data;

[UsedImplicitly]
public class WaypointData
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonIgnore] public Vector2 Position => new(X, Y);
    [JsonProperty("x")] public float X { get; set; }
    [JsonProperty("y")] public float Y { get; set; }
}
