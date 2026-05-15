// using HarmonyLib;
//
// namespace CustomFungamePack;
//
// [HarmonyPatch(typeof(PlayerCameraPatch))]
// public class PlayerCameraPatch
// {
//     internal static PlayerCamera PlayerCamera;
//     
//     [HarmonyPatch("Awake")]
//     [HarmonyPostfix]
//     public static void Awake(PlayerCamera __instance)
//     {
//         PlayerCamera = __instance;
//     }
// }