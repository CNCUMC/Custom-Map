using System.Reflection;
using CustomMap.Data.Feature.World;
using HarmonyLib;
using UnityEngine;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(GeyserScript))]
public class GeyserScriptPatch
{
    private static readonly FieldInfo ActivateTimeField = typeof(GeyserScript).GetField(
        "activateTime", BindingFlags.NonPublic | BindingFlags.Instance);

    private static GeyserData GeyserData => MapCheck.CurrentMap?.GeyserData;

    [HarmonyPatch("TryRumble")]
    [HarmonyPrefix]
    public static void TryRumblePrefix(GeyserScript __instance)
    {
        var data = GeyserData;
        if (data == null) return;

        try
        {
            var activateTime = (float)(ActivateTimeField?.GetValue(__instance) ?? 0f);
            var shift = 10f - data.Cooldown;
            if (shift > 0f)
                ActivateTimeField?.SetValue(__instance, activateTime - shift);
        }
        catch
        {
            // Silently ignore
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(GeyserScript __instance)
    {
        var data = GeyserData;
        if (data == null) return;

        try
        {
            var activateTime = (float)(ActivateTimeField?.GetValue(__instance) ?? 0f);

            if (activateTime < -50f) return;

            var elapsed = Time.time - activateTime;

            if (elapsed >= data.ActivateDuration && elapsed < 4.5f)
                ActivateTimeField?.SetValue(__instance, Time.time - 5f);

            if (!data.NoLiquid || !(elapsed < 4.5f)) return;
            var targetTime = Time.time - 10f;
            ActivateTimeField?.SetValue(__instance, targetTime);
        }
        catch
        {
            // Silently ignore
        }
    }
}