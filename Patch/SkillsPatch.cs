using System;
using HarmonyLib;

namespace CustomMap.Patch;

[HarmonyPatch(typeof(Skills))]
public class SkillsPatch
{
    private const int MaxSafeLevel = 30;

    private const int MaxLevelUpsPerCheck = 20;

    [HarmonyPatch("Setup")]
    [HarmonyPrefix]
    private static bool SetupPrefix(Skills __instance, int chr)
    {
        var currentMap = MapCheck.CurrentMap;
        if (currentMap?.XpData == null)
            return true;

        var xpData = currentMap.XpData;

        __instance.STR = Math.Min(xpData.StrXp, MaxSafeLevel);
        __instance.RES = Math.Min(xpData.ResXp, MaxSafeLevel);
        __instance.INT = Math.Min(xpData.IntXp, MaxSafeLevel);

        // 重算边界（确保 min/max/exp 与等级一致）
        __instance.UpdateExpBoundaries();

        // 钳制经验值到 [min, max)，防止加载时触发升级循环
        __instance.expSTR = ClampExp(xpData.ExpStr, __instance.minSTR, __instance.maxSTR);
        __instance.expRES = ClampExp(xpData.ExpRes, __instance.minRES, __instance.maxRES);
        __instance.expINT = ClampExp(xpData.ExpInt, __instance.minINT, __instance.maxINT);

        return false;
    }

    [HarmonyPatch("CheckForLevelUp")]
    [HarmonyPrefix]
    private static bool CheckForLevelUpPrefix(
        Skills __instance,
        ref int level,
        float xp,
        ref int xpToLevel,
        ref bool __result)
    {
        var iterations = 0;
        while (xp >= (double)xpToLevel)
        {
            ++level;
            __instance.UpdateExpBoundaries();
            if (++iterations < MaxLevelUpsPerCheck) continue;
            // 达到上限，将当前属性的经验值钳制到 max-1
            // xp 是 this.expSTR/RES/INT 的副本，通过值匹配确定是哪个属性
            ConsumeExcessXp(__instance, xp);
            break;
        }

        __result = iterations > 0;
        return false;
    }

    private static void ConsumeExcessXp(Skills __instance, float xp)
    {
        // xp 是进入 CheckForLevelUp 时 this.expSTR/RES/INT 的副本
        // 循环中 exp 字段未被修改，所以 xp 仍匹配对应字段的当前值
        if (Math.Abs(xp - __instance.expSTR) < 0.5f)
            __instance.expSTR = __instance.maxSTR - 1;
        else if (Math.Abs(xp - __instance.expRES) < 0.5f)
            __instance.expRES = __instance.maxRES - 1;
        else if (Math.Abs(xp - __instance.expINT) < 0.5f)
            __instance.expINT = __instance.maxINT - 1;
    }

    private static float ClampExp(float exp, int min, int max)
    {
        if (max <= min) return min;
        return Math.Max(min, Math.Min(exp, max - 1));
    }
}