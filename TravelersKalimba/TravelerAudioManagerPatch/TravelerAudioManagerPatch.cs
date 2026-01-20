using HarmonyLib;
using UnityEngine;

namespace TravelersKalimba
{
    [HarmonyPatch]
    public class TravelerAudioManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TravelerAudioManager), nameof(TravelerAudioManager.SyncTravelers))]
        public static void SyncTravelers_Postfix(TravelerAudioManager __instance) //изменение тайминга аудио в методе синхронизации
        {
            AudioSignal signal = __instance._signals[0];

            OWAudioSource owAudioSource = signal.GetOWAudioSource();

            int timeSample = owAudioSource.timeSamples;
            
            Main.Instance.AudioSynchronizer.InvokeTravelersAudioSync(timeSample, "В SyncTravelers");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TravelerAudioManager), nameof(TravelerAudioManager.OnUnpause))]
        public static void OnUnpause_Postfix() //изменение тайминга аудио после выхода из паузы 
        {
            Main.Instance.AudioSynchronizer.InvokeTravelersAudioSync(0, "В OnUnpause");
        }   

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerAudioManager), nameof(TravelerAudioManager.Update))]
        public static void Update_Prefix(TravelerAudioManager __instance) //изменение тайминга аудио в методе апдейт 
        {
            if(__instance._playAfterDelay && Time.time >= __instance._playAudioTime)
                Main.Instance.AudioSynchronizer.InvokeTravelersAudioSync(0, "В Update");
        }
    }
}
