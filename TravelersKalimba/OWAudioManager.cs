using UnityEngine;
using UnityEngine.Audio;

namespace TravelersKalimba
{
    public class OWAudioManager
    {
        private AudioMixerGroup _signalAudioMixerGroup;

        public AudioMixerGroup GetSignalAudioMixerGroup()
        {
            if (_signalAudioMixerGroup == null)
                _signalAudioMixerGroup = FindAudioMixerGroup(ModStaticState.SignalAudioMixerGroup);

            return _signalAudioMixerGroup;
        }

        public static AudioMixerGroup FindAudioMixerGroup(string groupName)
        {
            AudioMixerGroup[] groups = Resources.FindObjectsOfTypeAll<AudioMixerGroup>();
            foreach (var g in groups)
            {
                if (g.name == groupName)
                    return g;
            }

            Main.Instance.ModHelper.Console.WriteLine($"AudioMixerGroup \"{groupName}\" не найден!", OWML.Common.MessageType.Warning);
            return null;
        }
    }
}
