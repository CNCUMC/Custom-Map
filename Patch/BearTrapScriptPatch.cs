using System.Collections.Generic;
using System.Reflection;
using CustomMap.Data.Feature.World;
using HarmonyLib;
using UnityEngine;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(BearTrap))]
public class BearTrapScriptPatch
{
    private static readonly FieldInfo ActivatedField = typeof(BearTrap).GetField(
        "activated", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo CaughtLimbField = typeof(BearTrap).GetField(
        "caughtLimb", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo OrigPosField = typeof(BearTrap).GetField(
        "origPos", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Dictionary<BearTrap, float> LastTriggerTime = new();
    private static BearTrapData BearTrapData => MapCheck.CurrentMap?.CurrentLayer.BearTrapData;

    [HarmonyPatch("OnTriggerEnter2D")]
    [HarmonyPrefix]
    public static bool OnTriggerPrefix(BearTrap __instance, Collider2D other)
    {
        var data = BearTrapData;
        var mult = data?.DamageMult ?? 1f;
        var undestroy = data?.Undestroy ?? false;
        var cooldown = data?.Cooldown ?? 0f;

        var isActivated = (bool)(ActivatedField?.GetValue(__instance) ?? false);
        var now = Time.time;

        if (cooldown > 0f
            && LastTriggerTime.TryGetValue(__instance, out var lastTime)
            && now - lastTime < cooldown)
            return false;

        if (!isActivated)
        {
            if (other.TryGetComponent(out Body body))
            {
                body.shock = 20f * mult;
                body.Ragdoll();
            }

            if (!other.TryGetComponent(out Limb limb))
                return false;

            ActivatedField?.SetValue(__instance, true);

            limb.body.shock = 100f * mult;
            limb.pain += 100f * mult;
            limb.body.adrenaline += 80f;
            if (Random.Range(0f, 1f) < 0.5f)
                limb.BreakBone();

            Vector2 origPos = limb.transform.position;
            OrigPosField?.SetValue(__instance, origPos);

            limb.skinHealth -= Random.Range(70f, 100f) / limb.GetArmorReduction() * mult;
            limb.muscleHealth -= Random.Range(50f, 100f) / limb.GetArmorReduction() * mult;
            limb.bleedAmount += Random.Range(14f, 18f) / limb.GetArmorReduction() * mult;
            limb.DamageWearables(0.9f);

            if (limb.isHead)
            {
                limb.body.consciousness = 0f;
                limb.body.brainHealth -= Random.Range(0f, 40f) * mult;
                if (Random.value > 0.6f)
                    limb.body.RemoveEye();
            }

            Sound.Play("beartrap", __instance.transform.position, pitchShift: false);
            Sound.Play("gore", limb.transform.position);

            CaughtLimbField?.SetValue(__instance, limb);

            limb.rb.constraints = RigidbodyConstraints2D.FreezeAll;

            var spriteRenderer = __instance.GetComponent<SpriteRenderer>();
            if (__instance.closeSprite != null)
                spriteRenderer.sprite = __instance.closeSprite;

            if (__instance.transform.childCount > 0)
                Object.Destroy(__instance.transform.GetChild(0).gameObject);

            PlayerCamera.main.shaker.Shake(50f);

            LastTriggerTime[__instance] = now;
            return false;
        }

        if (!undestroy) return false;
        HandleReTrigger(__instance, other, data);
        LastTriggerTime[__instance] = now;

        return false;
    }

    private static void HandleReTrigger(BearTrap instance, Collider2D other, BearTrapData data)
    {
        var mult = data.DamageMult;
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