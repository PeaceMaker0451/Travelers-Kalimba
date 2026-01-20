using System;
using UnityEngine;

namespace TravelersKalimba
{
    public class TravelerSynchronizationManager: MonoBehaviour
    {
        private string[] TravelerSignalSourceGameObjectNames = { "Signal_Banjo", "Signal_Mask", "Signal_Prisoner", "Signal_Harmonica", "Signal_Flute", "Signal_Drums", "Signal_Whistling" };
        
        private AudioSource _travelerAudioSource;

        public bool HaveTimingReference => _travelerAudioSource != null;

        public event Action<int> TravelersAudioSynchronized;

        public void InvokeTravelersAudioSync(int timeSamples, string reason = "default")
        {
            TravelersAudioSynchronized?.Invoke(timeSamples);

            #if (DEBUG)
                Main.Instance.ModHelper.Console.WriteLine($"Синхронизация музыки путешественников!!! - {timeSamples} ({reason})");
            #endif
        }

        public bool TryGetCurrentTravelerThemeTime(out float time)
        {
            time = 0;

            if (VerifySignalSource() == false)
                return false;

            time = _travelerAudioSource.time;
            return true;
        }

        public bool TryGetTotalTravelerThemeTime(out float time)
        {
            time = 0;

            if (VerifySignalSource() == false)
                return false;

            time = _travelerAudioSource.clip.length;
            return true;
        }

        public bool TryGetTravelerThemeTimeSamples(out int timeSamples)
        {
            timeSamples = 0;

            if (VerifySignalSource() == false)
                return false;

            timeSamples = _travelerAudioSource.timeSamples;
            return true;
        }

        public bool TryGetTiming(out float time, out float clipLenth)
        {
            time = 0;
            clipLenth = 0;

            if (VerifySignalSource() == false)
                return false;

            time = _travelerAudioSource.time;
            clipLenth = _travelerAudioSource.clip.length;
            return true;
        }

        private bool VerifySignalSource()
        {
            if (_travelerAudioSource != null && _travelerAudioSource.isPlaying == false)
                _travelerAudioSource = null;
            
            if (_travelerAudioSource == null)
                FindSignalAudioSource();

            if (_travelerAudioSource == null)
                return false;
            else
                LogSignalSource();

            return true;
        }
         
        private void FindSignalAudioSource()
        {
            _travelerAudioSource = TryFindOWVSignalSource();

            if (_travelerAudioSource == null)
                Main.Instance.ModHelper.Console.WriteLine("Невозможно найти источник сигнала ПДК!", OWML.Common.MessageType.Warning);
        }

        private AudioSource TryFindOWVSignalSource()
        {
            GameObject signalSource;

            #if (DEBUG)
                Main.Instance.ModHelper.Console.WriteLine($"Поиск источника для сигнала...");
            #endif

            foreach (string name in TravelerSignalSourceGameObjectNames)
            {
                signalSource = GameObject.Find(name);

                #if (DEBUG)
                    Main.Instance.ModHelper.Console.WriteLine($"{name} - {signalSource}");
                #endif

                if (signalSource != null)
                {
                    AudioSource audioSource = signalSource.GetComponent<AudioSource>();

                    if (audioSource == null)
                        continue;

                    if(audioSource.isPlaying)
                        return audioSource;
                }
            }

            return null;
        }

        private void LogSignalSource()
        {
            if (_travelerAudioSource == null)
                return;

            #if (DEBUG)
                Main.Instance.ModHelper.Console.WriteLine($"Источник сигнала {_travelerAudioSource.name}" +
                $"\ntime - {_travelerAudioSource.time}" +
                $"\ntimeSamples - {_travelerAudioSource.timeSamples}" +
                $"\nclip - {_travelerAudioSource.clip}" +
                $"\nvolume - {_travelerAudioSource.volume}");
            #endif
        }
    }
}
