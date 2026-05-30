using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(BearTrap))]
public class BearTrapScriptPatch
{
    private static BearTrapData BearTrapData => FungameCheck.CurrentFungame?.Feature?.BearTrapData;

    private static readonly FieldInfo ActivatedField = typeof(BearTrap).GetField(
        "activated", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo CaughtLimbField = typeof(BearTrap).GetField(
        "caughtLimb", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Dictionary<BearTrap, float> LastTriggerTime = new();

    [HarmonyPatch("OnTriggerEnter2D")]
    [HarmonyPrefix]
    public static bool OnTriggerPrefix(BearTrap __instance, Collider2D other)
    {
        var data = BearTrapData;
        if (data == null)
            return true;

        try
        {
            switch (data.Undestroy)
            {
                case true when data.Cooldown > 0f &&
                               LastTriggerTime.TryGetValue(__instance, out float lastTime) &&
                               Time.time - lastTime < data.Cooldown:
                    return false;
                case true when (bool)(ActivatedField?.GetValue(__instance) ?? false):
                    HandleReTrigger(__instance, other, data);
                    return false;
                default:
                    return true;
            }
        }
        catch
        {
            return true;
        }
    }

    [HarmonyPatch("OnTriggerEnter2D")]
    [HarmonyPostfix]
    public static void OnTriggerPostfix(BearTrap __instance)
    {
        var data = BearTrapData;
        if (data == null) return;

        try
        {
            switch (data.Undestroy)
            {
                case true when data.Cooldown > 0f &&
                               LastTriggerTime.TryGetValue(__instance, out float lastTime) &&
                               Time.time - lastTime < data.Cooldown:
                    return;
                case true:
                    LastTriggerTime[__instance] = Time.time;
                    ActivatedField?.SetValue(__instance, false);
                    break;
            }
        }
        catch
        {
            // Silently ignore
        }
    }

    private static void HandleReTrigger(BearTrap instance, Collider2D other, BearTrapData data)
    {
        float mult = data.DamageMult;
        var spriteRenderer = instance.GetComponent<SpriteRenderer>();

        if (instance.closeSprite != null)
            spriteRenderer.sprite = instance.closeSprite;

        if (!other.TryGetComponent(out Limb limb)) return;

        limb.body.shock = 100f * mult;
        limb.pain += 100f * mult;
        limb.body.adrenaline += 80f;
        if (Random.Range(0f, 1f) < 0.5f)
            limb.BreakBone();

        limb.skinHealth -= Random.Range(70f, 100f) / limb.GetArmorReduction() * mult;
        limb.muscleHealth -= Random.Range(50f, 100f) / limb.GetArmorReduction() * mult;
        limb.bleedAmount += Random.Range(14f, 18f) / limb.GetArmorReduction() * mult;
        limb.DamageWearables(0.9f);

        if (limb.isHead)
        {
            limb.body.consciousness = 0.0f;
            limb.body.brainHealth -= Random.Range(0.0f, 40f) * mult;
            if (Random.value > 0.6f)
                limb.body.RemoveEye();
        }

        Sound.Play("beartrap", instance.transform.position, pitchShift: false);
        Sound.Play("gore", limb.transform.position);

        CaughtLimbField?.SetValue(instance, limb);
        spriteRenderer.sprite = instance.closeSprite;
        PlayerCamera.main.shaker.Shake(50f);
    }
}
