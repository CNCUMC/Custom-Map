using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(SoundCannon))]
public class SoundCannonScriptPatch
{
    private static SoundCannonData SoundCannonData => FungameCheck.CurrentFungame?.Feature?.SoundCannonData;

    private static readonly FieldInfo SpentField = typeof(SoundCannon).GetField(
        "spent", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ChargingField = typeof(SoundCannon).GetField(
        "charging", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ChargeTimeField = typeof(SoundCannon).GetField(
        "chargeTime", BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Tracks when each cannon instance started charging.
    /// Used to trigger fire at the configured charge time.
    /// </summary>
    private static readonly Dictionary<SoundCannon, float> ChargingSince = new();

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    public static void UpdatePrefix(SoundCannon __instance)
    {
        var data = SoundCannonData;
        if (data == null) return;

        try
        {
            // Set maxDistance BEFORE original Update's distance check
            __instance.maxDistance = data.MaxDistance;
        }
        catch
        {
            // Silently ignore
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(SoundCannon __instance)
    {
        var data = SoundCannonData;
        if (data == null) return;

        try
        {
            bool isCharging = (bool)(ChargingField?.GetValue(__instance) ?? false);
            bool isSpent = (bool)(SpentField?.GetValue(__instance) ?? false);

            // Track charging start time and force fire at configured charge time
            if (isCharging)
            {
                if (!ChargingSince.ContainsKey(__instance))
                    ChargingSince[__instance] = Time.time;

                if (ChargingSince.TryGetValue(__instance, out float startTime) &&
                    Time.time - startTime >= data.ChargeTime)
                {
                    // Force chargeTime past the 5.0 threshold so next frame fires
                    ChargeTimeField?.SetValue(__instance, 5.1f);
                }
            }
            else
            {
                ChargingSince.Remove(__instance);
            }

            // Undestroy: reset spent + chargeTime to allow clean re-trigger
            if (data.Undestroy && isSpent)
            {
                SpentField?.SetValue(__instance, false);
                ChargeTimeField?.SetValue(__instance, 0f);
                ChargingSince.Remove(__instance);
            }
        }
        catch
        {
            // Silently ignore
        }
    }
}
