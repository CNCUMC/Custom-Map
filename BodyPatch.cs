using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace CustomFungamePack;

[HarmonyPatch(typeof(Body))]
public static class BodyPatch
{
    private static readonly Feature Feature = FungameCheck.CurrentFungame?.Feature;

    private static readonly FieldInfo FirstWallJumpField = typeof(Body).GetField(
        "firstWallJump",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly bool IsPatchActive = Feature != null && FirstWallJumpField != null;
    private static readonly KeyCode JumpKey = KeyBinds.GetBind("jump");
    private static int _jumpCount;

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    public static void Jump(Body __instance)
    {
        if (!IsPatchActive)
            return;

        JumpLimit(__instance);
    }

    private static void JumpLimit(Body __instance)
    {
        if (!Input.GetKey(JumpKey) || _jumpCount >= Feature.JumpLimit)
        {
            _jumpCount = 0;
            return;
        }

        FirstWallJumpField.SetValue(__instance, true);
        _jumpCount++;
    }
}