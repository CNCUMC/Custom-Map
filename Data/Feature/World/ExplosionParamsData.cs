using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomFungamePack.Data.Feature.World;

[UsedImplicitly]
public class ExplosionParamsData
{
    private static readonly Lazy<ExplosionParams> DefaultExplosionParams = new(() => new ExplosionParams());

    [JsonProperty("muscle_damage")] public RangeF MuscleDamage { get; set; }
    [JsonProperty("skin_damage")] public RangeF SkinDamage { get; set; }
    [JsonProperty("skin_damage_chance")] public float SkinDamageChance { get; set; }
    [JsonProperty("bone_break_chance")] public float BoneBreakChance { get; set; }
    [JsonProperty("dislocation_chance")] public float DislocationChance { get; set; }
    [JsonProperty("disfigure_chance")] public float DisfigureChance { get; set; }
    [JsonProperty("bleed_chance")] public float BleedChance { get; set; }
    [JsonProperty("bleed_amount")] public RangeF BleedAmount { get; set; }
    [JsonProperty("structural_damage")] public float StructuralDamage { get; set; }
    [JsonProperty("range")] public float Range { get; set; }
    [JsonProperty("velocity")] public float Velocity { get; set; }
    [JsonProperty("shrapnel_chance")] public float ShrapnelChance { get; set; }
    [JsonProperty("sound")] public string Sound { get; set; }

    public ExplosionParamsData()
    {
        var defaults = DefaultExplosionParams.Value;
        MuscleDamage = defaults.muscleDamage;
        SkinDamage = defaults.skinDamage;
        SkinDamageChance = defaults.skinDamageChance;
        BoneBreakChance = defaults.boneBreakChance;
        DislocationChance = defaults.dislocationChance;
        DisfigureChance = defaults.disfigureChance;
        BleedChance = defaults.bleedChance;
        BleedAmount = defaults.bleedAmount;
        StructuralDamage = defaults.structuralDamage;
        Range = defaults.range;
        Velocity = defaults.velocity;
        ShrapnelChance = defaults.shrapnelChance;
        Sound = defaults.sound;
    }
}