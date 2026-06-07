using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(Skills))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SkillsPatch
{
    [HarmonyPatch("Setup")]
    [HarmonyPostfix]
    private static void SetupPostfix(Skills __instance)
    {
        var xpData = FungameCheck.CurrentFungame.XpData;

        __instance.INT = xpData.IntXp;
        __instance.RES = xpData.ResXp;
        __instance.STR = xpData.StrXp;
        __instance.expINT = xpData.ExpInt;
        __instance.expRES = xpData.ExpRes;
        __instance.expSTR = xpData.ExpStr;

        __instance.minINT = xpData.MinInt;
        __instance.minRES = xpData.MinRes;
        __instance.minSTR = xpData.MinStr;

        __instance.maxINT = xpData.MaxInt;
        __instance.maxRES = xpData.MaxRes;
        __instance.maxSTR = xpData.MaxStr;
    }
}