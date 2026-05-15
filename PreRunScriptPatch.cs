// using HarmonyLib;
//
// namespace CustomFungamePack;
//
// [HarmonyPatch(typeof(PreRunScript))]
// public class PreRunScriptPatch
// {
//     internal static PreRunScript PreRunScript;
//     internal static bool ShouldAutoStart = false;
//     
//     [HarmonyPatch("Awake")]
//     [HarmonyPostfix]
//     public static void Awake(PreRunScript __instance)
//     {
//         PreRunScript = __instance;
//         
//         if (ShouldAutoStart)
//         {
//             __instance.StartRun();
//         }
//     }
// }