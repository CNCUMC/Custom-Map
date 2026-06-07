using System;
using System.Reflection;
using CustomFungamePack.Data;
using CustomFungamePack.Data.Feature.World;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(MineScript))]
public class MineScriptPatch
{
    private static MineData MineData => FungameCheck.CurrentFungame?.MineData;

    private static readonly FieldInfo PressedField = typeof(MineScript).GetField(
        "pressed",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ExplodedField = typeof(MineScript).GetField(
        "exploded",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Lazy<ExplosionParams> DefaultExplosionParams = new(() => new ExplosionParams());

    private static ExplosionParams CreateMineExplosionParams(Vector3 pos)
    {
        var explosionData = MineData?.ExplosionParamsData;
        var defaults = DefaultExplosionParams.Value;

        return new ExplosionParams
        {
            position = pos,
            muscleDamage = explosionData?.MuscleDamage ?? defaults.muscleDamage,
            skinDamage = explosionData?.SkinDamage ?? defaults.skinDamage,
            skinDamageChance = explosionData?.SkinDamageChance ?? defaults.skinDamageChance,
            boneBreakChance = explosionData?.BoneBreakChance ?? defaults.boneBreakChance,
            dislocationChance = explosionData?.DislocationChance ?? defaults.dislocationChance,
            disfigureChance = explosionData?.DisfigureChance ?? defaults.disfigureChance,
            bleedChance = explosionData?.BleedChance ?? defaults.bleedChance,
            bleedAmount = explosionData?.BleedAmount ?? defaults.bleedAmount,
            structuralDamage = explosionData?.StructuralDamage ?? defaults.structuralDamage,
            range = explosionData?.Range ?? defaults.range,
            velocity = explosionData?.Velocity ?? defaults.velocity,
            shrapnelChance = explosionData?.ShrapnelChance ?? defaults.shrapnelChance,
            sound = explosionData?.Sound ?? defaults.sound
        };
    }

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix(MineScript __instance)
    {
        if (!(bool)PressedField.GetValue(__instance))
            return true;
        __instance.timeSincePressed += Time.deltaTime;

        var explosionDelay = MineData?.Cooldown ?? 0.8f;
        bool exploded = (bool)ExplodedField.GetValue(__instance);
        bool mineUndestroy = MineData?.Undestroy ?? false;

        if (mineUndestroy && exploded && __instance.timeSincePressed > explosionDelay)
        {
            ExplodedField.SetValue(__instance, false);
            return false;
        }

        if (exploded || !(__instance.timeSincePressed > explosionDelay))
            return false;

        ExplodedField.SetValue(__instance, true);

        if (mineUndestroy)
        {
            __instance.build.health = 99999f;
            PressedField.SetValue(__instance, false);
        }
        else
        {
            __instance.build.health = 100f;
        }

        __instance.timeSincePressed = 0f;

        var explosionParams = CreateMineExplosionParams(__instance.transform.position + Vector3.up);
        WorldGeneration.CreateExplosion(explosionParams);

        return false;
    }

    [HarmonyPatch("OnDestroy")]
    [HarmonyPrefix]
    public static bool OnDestroyPrefix(MineScript __instance)
    {
        if (MineData?.Undestroy == true)
            return false;

        if (__instance.build.health >= 0.5f || (bool)ExplodedField.GetValue(__instance))
            return false;

        var explosionParams = CreateMineExplosionParams(__instance.transform.position + Vector3.up);
        WorldGeneration.CreateExplosion(explosionParams);

        return false;
    }
}