using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomMap.Data.Feature.Player;

[UsedImplicitly]
public class XpData
{
    private const int StrIndex = 0;
    private const int ResIndex = 1;
    private const int IntIndex = 2;

    private static readonly int[] DefaultBaselayers = Skills.BaseSkills(0);

    public XpData()
    {
        ResetToDefaults();
    }

    [JsonProperty("type")] public string Type { get; internal set; }
    [JsonProperty("str_xp")] public int StrXp { get; set; }
    [JsonProperty("res_xp")] public int ResXp { get; set; }
    [JsonProperty("int_xp")] public int IntXp { get; set; }

    [JsonProperty("exp_str")] public float ExpStr { get; set; }
    [JsonProperty("exp_res")] public float ExpRes { get; set; }
    [JsonProperty("exp_int")] public float ExpInt { get; set; }

    [JsonProperty("min_str")] public int MinStr { get; set; }
    [JsonProperty("max_str")] public int MaxStr { get; set; }

    [JsonProperty("min_res")] public int MinRes { get; set; }
    [JsonProperty("max_res")] public int MaxRes { get; set; }

    [JsonProperty("min_int")] public int MinInt { get; set; }
    [JsonProperty("max_int")] public int MaxInt { get; set; }

    public void ResetToDefaults()
    {
        StrXp = DefaultBaselayers[StrIndex];
        ResXp = DefaultBaselayers[ResIndex];
        IntXp = DefaultBaselayers[IntIndex];
        RecalculateMinMax();
    }

    public void RecalculateMinMax()
    {
        MinStr = ClampThreshold(Skills.GetExperienceForLevel(StrXp));
        MaxStr = ClampThreshold(Skills.GetExperienceForLevel(StrXp + 1));
        ExpStr = MinStr;

        MinRes = ClampThreshold(Skills.GetExperienceForLevel(ResXp));
        MaxRes = ClampThreshold(Skills.GetExperienceForLevel(ResXp + 1));
        ExpRes = MinRes;

        MinInt = ClampThreshold(Skills.GetExperienceForLevel(IntXp));
        MaxInt = ClampThreshold(Skills.GetExperienceForLevel(IntXp + 1));
        ExpInt = MinInt;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        StrXp = Math.Max(0, StrXp);
        ResXp = Math.Max(0, ResXp);
        IntXp = Math.Max(0, IntXp);
        RecalculateMinMax();
    }

    private static int ClampThreshold(int value)
    {
        return Math.Max(0, value);
    }
}