using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CustomFungamePack.Data;
using CustomFungamePack.Data.Feature.World;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(SoundCannon))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SoundCannonScriptPatch
{
    private static SoundCannonData SoundCannonData => FungameCheck.CurrentFungame?.SoundCannonData;

    private static readonly FieldInfo SpentField = typeof(SoundCannon).GetField(
        "spent", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ChargingField = typeof(SoundCannon).GetField(
        "charging", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ChargeTimeField = typeof(SoundCannon).GetField(
        "chargeTime", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Dictionary<SoundCannon, float> ChargingSince = new();

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
            var isCharging = (bool)(ChargingField?.GetValue(__instance) ?? false);
            var isSpent = (bool)(SpentField?.GetValue(__instance) ?? false);

            if (isCharging)
            {
                if (!ChargingSince.ContainsKey(__instance))
                    ChargingSince[__instance] = Time.time;

                if (ChargingSince.TryGetValue(__instance, out float startTime) &&
                    Time.time - startTime >= data.Cooldown)
                {
                    ChargeTimeField?.SetValue(__instance, 5.1f);
                }
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
            // Silently ignore
        }
    }
}