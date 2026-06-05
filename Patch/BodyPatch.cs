using System.Reflection;
using CustomFungamePack.Data;
using HarmonyLib;
using UnityEngine;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(Body))]
public static class BodyPatch
{
    private static WorldSettingsData WorldSettings => FungameCheck.CurrentFungame?.WorldSettings;

    private static readonly FieldInfo JumpCooldownField = typeof(Body).GetField(
        "jumpCooldown",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo FirstWallJumpField = typeof(Body).GetField(
        "firstWallJump",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static bool IsPatchActive => WorldSettings != null
                                         && JumpCooldownField != null
                                         && FirstWallJumpField != null;

    // 注意: 不直接使用 MossLib.Tool.Key.IsKeyDown("jump")
    // 因为 MossLib 内部引用的 KeyBinds 类型在某些游戏版本中无法加载（TypeLoadException）
    private static bool JumpKey => Input.GetKeyDown(KeyCode.Space);

    private static int _jumpCount;
    private static int _climbCount;

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    public static void UpdatePostfix(Body __instance)
    {
        if (!IsPatchActive)
            return;

        HandleMultiJump(__instance);
        HandleMultiClimb(__instance);

        // 没落地
        // 初始化
        if (!__instance.grounded) return;
        _jumpCount = 0;
        _climbCount = 0;
    }

    private static void HandleMultiJump(Body __instance)
    {
        // 落地
        if (__instance.grounded)
            return;

        // 没按跳 到头了
        if (!JumpKey || _jumpCount >= WorldSettings.JumpLimit)
            return;

        // 恢复
        JumpCooldownField.SetValue(__instance, 0f);
        __instance.grounded = true;
        __instance.Jump();
        _jumpCount++;
    }

    private static void HandleMultiClimb(Body __instance)
    {
        // // 落地
        if (__instance.grounded)
        {
            return;
        }

        // 没按跳 到头了
        if (!JumpKey || _climbCount >= WorldSettings.ClimbLimit)
            return;

        // 恢复
        FirstWallJumpField.SetValue(__instance, true);
        __instance.grounded = true;
        _climbCount++;
    }
}