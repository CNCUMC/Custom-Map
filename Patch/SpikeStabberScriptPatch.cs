using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Bark.Tool;
using CustomMap.Data.Feature.World;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(SpikeStabberScript))]
public class SpikeStabberScriptPatch
{
    private static readonly FieldInfo LightField = typeof(SpikeStabberScript).GetField(
        "light", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Dictionary<SpikeStabberScript, float> LastTriggerTime = new();
    private static SpikeStabberData SpikeStabberData => MapCheck.CurrentMap?.SpikeStabberData;

    [HarmonyPatch("OnTriggerEnter2D")]
    [HarmonyPrefix]
    public static bool OnTriggerPrefix(SpikeStabberScript __instance, Collider2D collision)
    {
        var data = SpikeStabberData;
        if (data == null) return true;

        try
        {
            __instance.damageMult = data.DamageMult;

            if (!string.IsNullOrEmpty(data.Sound))
                __instance.sound = data.Sound;

            return !data.Undestroy || !(data.Cooldown > 0f) ||
                   !LastTriggerTime.TryGetValue(__instance, out var lastTime) ||
                   !(Time.time - lastTime < data.Cooldown);
        }
        catch
        {
            return true;
        }
    }

    [HarmonyPatch("OnTriggerEnter2D")]
    [HarmonyPostfix]
    public static void OnTriggerPostfix(SpikeStabberScript __instance)
    {
        var data = SpikeStabberData;
        if (data == null) return;

        try
        {
            switch (data.Undestroy)
            {
                case true when data.Cooldown > 0f &&
                               LastTriggerTime.TryGetValue(__instance, out var lastTime)
                               && Time.time - lastTime < data.Cooldown:
                    return;
                case true:
                    LastTriggerTime[__instance] = Time.time;

                    __instance.CheckStab();

                    __instance.StartCoroutine(ResetSpikeCoroutine(__instance));
                    break;
            }

            if (!data.NoLight)
                return;
            if (LightField?.GetValue(__instance) is Light2D light)
                light.enabled = false;
            if (__instance.bonusLight != null)
                __instance.bonusLight.enabled = false;
        }
        catch
        {
            // ignore
        }
    }

    private static IEnumerator ResetSpikeCoroutine(SpikeStabberScript instance)
    {
        var data = SpikeStabberData;
        var cooldown = data?.Cooldown ?? 0f;

        if (cooldown > 0f)
            yield return new WaitForSeconds(cooldown);

        if (!instance || !WorldGeneration.world)
            yield break;

        WorldUtil.PlaceItem(instance.transform.position, "spikestabber");

        Object.Destroy(instance.gameObject);
    }
}
