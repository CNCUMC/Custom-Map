using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(TurretScript))]
public class TurretScriptPatch
{
    private static TurretData TurretData => FungameCheck.CurrentFungame?.Feature?.TurretData;

    private static readonly FieldInfo BuildField = typeof(TurretScript).GetField(
        "build", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo ExplodeCountField = typeof(TurretScript).GetField(
        "explodeCount", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo DidCountSoundField = typeof(TurretScript).GetField(
        "didcountsound", BindingFlags.NonPublic | BindingFlags.Instance);

    private static float GetCooldown() => TurretData?.Cooldown ?? 15f;

    [HarmonyPatch("Update")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var getCooldownMethod = typeof(TurretScriptPatch).GetMethod(
            nameof(GetCooldown), BindingFlags.Static | BindingFlags.NonPublic);

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)instruction.operand, 15f))
            {
                yield return new CodeInstruction(OpCodes.Call, getCooldownMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(TurretScript __instance)
    {
        var data = TurretData;
        if (data == null) return;

        try
        {
            __instance.shotPowerMultiplier = data.ShotPowerMultiplier;

            if (data.NoLight)
            {
                var fireLight = __instance.fireLight;
                if (fireLight != null)
                    fireLight.intensity = 0f;
            }

            var build = BuildField?.GetValue(__instance) as BuildingEntity;
            if (build == null || !data.Undestroy) return;
            if (build.health < 350f)
                build.health = 99999f;

            ExplodeCountField?.SetValue(__instance, 0f);
            DidCountSoundField?.SetValue(__instance, false);
        }
        catch
        {
            // ignored
        }
    }
}