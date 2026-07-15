using System.Reflection;
using Bark.Constant;
using CustomMap.Data.Feature.World;
using HarmonyLib;
using UnityEngine;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(Body))]
public static class BodyPatch
{
    private static readonly FieldInfo JumpCooldownField = typeof(Body).GetField(
        "jumpCooldown",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo FirstWallJumpField = typeof(Body).GetField(
        "firstWallJump",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static int _jumpCount;
    private static int _climbCount;
    private static WorldSettingsData WorldSettings => MapCheck.CurrentMap?.CurrentLayer.WorldSettingsData;

    private static bool IsPatchActive =>
        WorldSettings != null
        && JumpCooldownField != null
        && FirstWallJumpField != null;

    private static bool JumpKey => Input.GetKeyDown(Keys.Jump);

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    public static void UpdatePostfix(Body __instance)
    {
        if (!IsPatchActive)
            return;

        HandleMultiJump(__instance);
        HandleMultiClimb(__instance);

        // û���
        // ��ʼ��
        if (!__instance.grounded) return;
        _jumpCount = 0;
        _climbCount = 0;
    }

    private static void HandleMultiJump(Body __instance)
    {
        // ���
        if (__instance.grounded)
            return;

        // û���� ��ͷ��
        if (!JumpKey || _jumpCount >= WorldSettings.JumpLimit)
            return;

        // �ָ�
        JumpCooldownField.SetValue(__instance, 0f);
        __instance.grounded = true;
        __instance.Jump();
        _jumpCount++;
    }

    private static void HandleMultiClimb(Body __instance)
    {
        // // ���
        if (__instance.grounded) return;

        // û���� ��ͷ��
        if (!JumpKey || _climbCount >= WorldSettings.ClimbLimit)
            return;

        // �ָ�
        FirstWallJumpField.SetValue(__instance, true);
        __instance.grounded = true;
        _climbCount++;
    }
}