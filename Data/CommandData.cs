using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomMap.Data;

[UsedImplicitly]
public class CommandData
{
    [JsonProperty("once_commands")] public List<string> OnceCommands { get; set; }
    [JsonProperty("loop_commands")] public List<string> LoopCommands { get; set; }
    [JsonProperty("loop_interval")] public float LoopInterval { get; set; }
}