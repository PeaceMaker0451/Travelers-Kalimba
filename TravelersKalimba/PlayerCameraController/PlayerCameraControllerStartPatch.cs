using HarmonyLib;

namespace TravelersKalimba
{
    [HarmonyPatch(typeof(PlayerCameraController), nameof(PlayerCameraController.Start))]
    public class PlayerCameraControllerStartPatch
    {
        public static void Postfix(ToolModeSwapper __instance)
        {
            __instance.gameObject.AddComponent<KalimbaManager>();
        }
    }
}
