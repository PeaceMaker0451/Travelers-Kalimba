using HarmonyLib;

namespace TravelersKalimba
{
    [HarmonyPatch]
    public class SignalscopePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Signalscope), nameof(Signalscope.EnterSignalscopeZoom))]
        private static bool EnterSignalscopeZoom_Prefix(Signalscope __instance)
        {
            if(ModStaticState.IsSignalscopeZoomShouldBeBlocked)
                return false;
            else
                return true;
        }
    }
}
