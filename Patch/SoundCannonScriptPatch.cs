using System.Collections.Generic;
using System.Reflection;
using CustomMap.Data.Feature.World;
using HarmonyLib;
using UnityEngine;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(SoundCannon))]
public class SoundCannonScriptPatch
{
    private static readonly FieldInfo SpentField = typeof(SoundCannon).GetField(
        "spent", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ChargingField = typeof(SoundCannon).GetField(
        "charging", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ChargeTimeField = typeof(SoundCannon).GetField(
        "chargeTime", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Dictionary<SoundCannon, float> ChargingSince = new();
    private static SoundCannonData SoundCannonData => MapCheck.CurrentMap?.CurrentLayer.SoundCannonData;

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    public static void UpdatePrefix(SoundCannon __instance)
    {
        var data = SoundCannonData;
        if (data == null) return;

        try
        {
            __instance.maxDistance = data.MaxDistance;
        }
        catch
        {
            // ignored
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
            var isCharging = (bool)(ChargingField?.GetValue(__instance) ?? false);
            var isSpent = (bool)(SpentField?.GetValue(__instance) ?? false);

            if (isCharging)
            {
                if (!ChargingSince.ContainsKey(__instance))
                    ChargingSince[__instance] = Time.time;

                if (ChargingSince.TryGetValue(__instance, out var startTime) &&
                    Time.time - startTime >= data.Cooldown)
                    ChargeTimeField?.SetValue(__instance, 5.1f);
            }
            else
            {
                ChargingSince.Remove(__instance);
            }

            if (!data.Undestroy || !isSpent) return;
            SpentField?.SetValue(__instance, false);
            ChargeTimeField?.SetValue(__instance, 0f);
            ChargingSince.Remove(__instance);
        }
        catch
        {
            // ignore
        }
    }
}