using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.Player;

[UsedImplicitly]
public class XpData
{
    [JsonProperty("type")] public string Type { get; internal set; }

    private readonly int[] _base = Skills.BaseSkills(0);

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

    public XpData()
    {
        ResetToDefaults();
    }

    private void ResetToDefaults()
    {
        StrXp = _base[0];
        ResXp = _base[1];
        IntXp = _base[2];
        RecalculateMinMax();
    }

    private void RecalculateMinMax()
    {
        MinStr = Skills.GetExperienceForLevel(StrXp);
        MaxStr = Skills.GetExperienceForLevel(StrXp + 1);
        MinRes = Skills.GetExperienceForLevel(ResXp);
        MaxRes = Skills.GetExperienceForLevel(ResXp + 1);
        MinInt = Skills.GetExperienceForLevel(IntXp);
        MaxInt = Skills.GetExperienceForLevel(IntXp + 1);

        ExpStr = MinStr;
        ExpRes = MinRes;
        ExpInt = MinInt;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        RecalculateMinMax();
    }
}
