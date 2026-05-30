using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(GeyserScript))]
public class GeyserScriptPatch
{
    private static GeyserData GeyserData => FungameCheck.CurrentFungame?.Feature?.GeyserData;

    private static readonly FieldInfo ActivateTimeField = typeof(GeyserScript).GetField(
        "activateTime", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo RumbleTimeField = typeof(GeyserScript).GetField(
        "rumbleTime", BindingFlags.NonPublic | BindingFlags.Instance);

    [HarmonyPatch("TryRumble")]
    [HarmonyPrefix]
    public static void TryRumblePrefix(GeyserScript __instance)
    {
        var data = GeyserData;
        if (data == null) return;

        try
        {
            // Override cooldown: original checks Time.time - activateTime < 10.0
            // Shift activateTime earlier so the check passes at configured cooldown
            var activateTime = (float)(ActivateTimeField?.GetValue(__instance) ?? 0f);
            float shift = 10f - data.Cooldown;
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
            float activateTime = (float)(ActivateTimeField?.GetValue(__instance) ?? 0f);
            float rumbleTime = (float)(RumbleTimeField?.GetValue(__instance) ?? 0f);

            // Don't modify anything before first activation
            if (activateTime < -50f) return;

            float elapsed = Time.time - activateTime;

            // 1. Shorten activate duration: fast-forward activateTime past 4.5
            if (elapsed >= data.ActivateDuration && elapsed < 4.5f)
                ActivateTimeField?.SetValue(__instance, Time.time - 5f);

            // 2. No liquid: suppress entirely
            if (data.NoLiquid && elapsed < 4.5f)
            {
                // Set activateTime very far in past to skip liquid emission
                var targetTime = Time.time - 10f;
                ActivateTimeField?.SetValue(__instance, targetTime);
            }
        }
        catch
        {
            // Silently ignore
        }
    }
}
