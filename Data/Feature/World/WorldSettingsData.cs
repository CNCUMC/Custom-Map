using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomMap.Data.Feature.World;

public class WorldSettingsData
{
    public static readonly Dictionary<string, string> RunSettingsKeyMap = new()
    {
        ["un_chipped"] = "unchipped",
        ["base_loot_density"] = "baselootdensity",
        ["loot_multiplier"] = "lootmultiplier",
        ["base_trap_density"] = "basetrapdensity",
        ["trap_increase"] = "trapincrease",
        ["ambient_light"] = "ambientlight",
        ["time_limit"] = "timelimit",
        ["starting_supplies"] = "startingsupplies",
        ["xp_gain"] = "xpgain",
        ["metabolism_rate"] = "metabolismrate",
        ["healing_rate"] = "healingrate",
        ["fracture_pain"] = "fracturepain",
        ["bleed_rate"] = "bleedrate",
        ["infection_speed"] = "infectionspeed",
        ["infection_chance"] = "infectionchance",
        ["fibrillation_rate"] = "fibrillationrate",
        ["mood_normalization_rate"] = "moodnormalizationrate",
        ["bonus_limb_armor"] = "bonuslimbarmor",
        ["stamina_regen"] = "staminaregen",
        ["attack_damage"] = "attackdamage",
        ["minigame_handshake"] = "minigamehandshake",
        ["sleep_cycle_speed"] = "sleepcyclespeed",
        ["encumbrance_cap"] = "encumbrancecap",
        ["strokes"] = "strokes",
        ["brain_damage_fx"] = "braindamagefx",
        ["force_sleep"] = "forcesleep",
        ["low_mood_events"] = "lowmoodevents",
        ["liquid_pushing"] = "liquidpushing",
        ["disfigurement"] = "disfigurement",
        ["no_sleep_restrictions"] = "nosleeprestrictions",
        ["infinite_last_stand"] = "infinitelaststand",
        ["trader_chance"] = "traderchance",
        ["trader_item_amount"] = "traderitemamount",
        ["trader_rep_offset"] = "traderrepoffset",
        ["item_decay_rate"] = "itemdecayrate",
        ["lock_pick_precision"] = "lockpickprecision",
        ["layer_modifier_chance"] = "layermodifierchance",
        ["time_between_earthquakes"] = "timebetweenearthquakes",
        ["temperature_offset"] = "temperatureoffset",
        ["ore_amount"] = "oreamount"
    };

    // 自定义属性
    [JsonProperty("full_bright")] public bool FullBright { get; set; } = true;
    [JsonProperty("forgiving_level")] public bool ForgivingLevel { get; set; }
    [JsonProperty("gravity")] public float Gravity { get; set; } = Physics2D.gravity.y;
    [JsonProperty("jump_limit")] public int JumpLimit { get; set; }
    [JsonProperty("climb_limit")] public int ClimbLimit { get; set; }
    [JsonProperty("settings_overrides")] public Dictionary<string, object> SettingsOverrides { get; set; }

    // RunSettings 属性（蛇形命名 → RunSettings 键名）
    [JsonProperty("un_chipped")] public bool Unchipped { get; set; }
    [JsonProperty("base_loot_density")] public float BaseLootDensity { get; set; } = 1.0f;
    [JsonProperty("loot_multiplier")] public float LootMultiplier { get; set; } = 1.0f;
    [JsonProperty("base_trap_density")] public float BaseTrapDensity { get; set; } = 1.0f;
    [JsonProperty("trap_increase")] public float TrapIncrease { get; set; }
    [JsonProperty("ambient_light")] public int AmbientLight { get; set; }
    [JsonProperty("time_limit")] public float TimeLimit { get; set; }
    [JsonProperty("starting_supplies")] public int StartingSupplies { get; set; }
    [JsonProperty("xp_gain")] public float XpGain { get; set; } = 1.0f;
    [JsonProperty("metabolism_rate")] public float MetabolismRate { get; set; } = 1.0f;
    [JsonProperty("healing_rate")] public float HealingRate { get; set; } = 1.0f;
    [JsonProperty("fracture_pain")] public float FracturePain { get; set; } = 1.0f;
    [JsonProperty("bleed_rate")] public float BleedRate { get; set; } = 1.0f;
    [JsonProperty("infection_speed")] public float InfectionSpeed { get; set; } = 1.0f;
    [JsonProperty("infection_chance")] public float InfectionChance { get; set; } = 1.0f;
    [JsonProperty("fibrillation_rate")] public float FibrillationRate { get; set; } = 1.0f;

    [JsonProperty("mood_normalization_rate")]
    public float MoodNormalizationRate { get; set; } = 1.0f;

    [JsonProperty("bonus_limb_armor")] public float BonusLimbArmor { get; set; }
    [JsonProperty("stamina_regen")] public float StaminaRegen { get; set; } = 1.0f;
    [JsonProperty("attack_damage")] public float AttackDamage { get; set; } = 1.0f;
    [JsonProperty("minigame_handshake")] public float MinigameHandshake { get; set; } = 1.0f;
    [JsonProperty("sleep_cycle_speed")] public float SleepCycleSpeed { get; set; } = 1.0f;
    [JsonProperty("encumbrance_cap")] public float EncumbranceCap { get; set; } = 1.0f;
    [JsonProperty("strokes")] public bool Strokes { get; set; } = true;
    [JsonProperty("brain_damage_fx")] public bool BrainDamageFx { get; set; } = true;
    [JsonProperty("force_sleep")] public bool ForceSleep { get; set; } = true;
    [JsonProperty("low_mood_events")] public bool LowMoodEvents { get; set; } = true;
    [JsonProperty("liquid_pushing")] public bool LiquidPushing { get; set; } = true;
    [JsonProperty("disfigurement")] public bool Disfigurement { get; set; } = true;

    [JsonProperty("no_sleep_restrictions")]
    public bool NoSleepRestrictions { get; set; }

    [JsonProperty("infinite_last_stand")] public bool InfiniteLastStand { get; set; }
    [JsonProperty("trader_chance")] public float TraderChance { get; set; } = 33.0f;
    [JsonProperty("trader_item_amount")] public float TraderItemAmount { get; set; } = 1.0f;
    [JsonProperty("trader_rep_offset")] public float TraderRepOffset { get; set; }
    [JsonProperty("item_decay_rate")] public float ItemDecayRate { get; set; } = 1.0f;
    [JsonProperty("lock_pick_precision")] public float LockPickPrecision { get; set; } = 1.0f;

    [JsonProperty("layer_modifier_chance")]
    public float LayerModifierChance { get; set; } = 40.0f;

    [JsonProperty("time_between_earthquakes")]
    public float TimeBetweenEarthquakes { get; set; } = 1.0f;

    [JsonProperty("temperature_offset")] public float TemperatureOffset { get; set; }
    [JsonProperty("ore_amount")] public float OreAmount { get; set; } = 1.0f;

    public static string GetRunSettingsKey(string jsonPropertyName)
    {
        return RunSettingsKeyMap.TryGetValue(jsonPropertyName, out var key) ? key : jsonPropertyName;
    }

    public static IEnumerable<string> GetMappablePropertyNames()
    {
        return RunSettingsKeyMap.Keys;
    }

    public Dictionary<string, object> ToRunSettingsDictionary()
    {
        var result = new Dictionary<string, object>();
        var properties = typeof(WorldSettingsData).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var jsonAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();
            var jsonName = jsonAttr?.PropertyName ?? prop.Name;

            if (!RunSettingsKeyMap.ContainsKey(jsonName)) continue;

            var runSettingsKey = GetRunSettingsKey(jsonName);
            var value = prop.GetValue(this);
            if (value != null) result[runSettingsKey] = value;
        }

        return result;
    }
}