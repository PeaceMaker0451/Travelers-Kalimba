using System;
using System.Drawing;
using UnityEngine;

namespace TravelersKalimba
{
    public class Kalimba : MonoBehaviour
    {
        private const string KalimbaRootGameObjectName = "KalimbaRoot";
        private const string KalimbaGameObjectName = "KalimbaGameObject";
        private const string KalimbaMeshGameObjectName = "KalimbaBody";

        private const string _hideTriggerName = "HideFast";
        private const string _equipTriggerName = "Equip";
        private const string _unequipTriggerName = "Unequip";

        private const string _playStateName = "Play";
        private const string _playTriggerName = "Play";
        private const string _stopTriggerName = "StopPlaying";
        
        private Animator _rigAnimator;
        private Animator _kalimbaAnimator;
        private AudioSource _audioSource;
        private AudioLowPassFilter _audioLowPassFilter;
        
        private float _audioSourceVolumeLerpFactor = 10f;
        private float _playingAudioSourceVolume = ModStaticState.KalimbaVolume;

        private float _audioCutOffFreq = 22000f;
        private float _audioCutOffFreqChangeFactor = 10f;

        private int _allowedDriftSamples = 2048;

        private Transform _kalimbaRoot;

        private bool _isGamePaused;
        
        public bool IsInitialized {  get; private set; }
        public bool IsReady { get; private set; }
        public bool IsDrawed { get; private set; }
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            GlobalMessenger.AddListener("GamePaused", new Callback(OnGamePaused));
            GlobalMessenger.AddListener("GameUnpaused", new Callback(OnGameUnpaused));

            Main.Instance.AudioSynchronizer.TravelersAudioSynchronized += SetAudioTimeSamples;
        }

        private void OnEnable()
        {
            if (IsReady == false)
                return;

            _kalimbaRoot.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (IsReady == false)
                return;

            _kalimbaRoot.gameObject.SetActive(false);
            _rigAnimator.SetTrigger(_hideTriggerName);
        }

        private void Update()
        {
            if (IsReady == false)
                return;

            HandleAudioVolumeChange();
            HandleCutOffChange();

            if(IsPlaying)
            {
                if (_isGamePaused == false)
                    PlaySynchronizelyAnimation();

                int framesToCheck = 30;
                
                if (Time.frameCount % framesToCheck == 0 && Main.Instance.AudioSynchronizer.TryGetTravelerThemeTimeSamples(out int refSamples))
                {
                    int mySamples = _audioSource.timeSamples;
                    int delta = Mathf.Abs(refSamples - mySamples);

                    if (delta > _allowedDriftSamples)
                    {
                        _audioSource.timeSamples = refSamples;

                        #if (DEBUG)
                            Main.Instance.ModHelper.Console.WriteLine($"Рассинхронизация больше допустимой - {delta}/{_allowedDriftSamples}");
                        #endif
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Main.Instance.AudioSynchronizer.TravelersAudioSynchronized -= SetAudioTimeSamples;
        }

        public void Initialize()
        {
            _rigAnimator = GetComponent<Animator>();
            if (_rigAnimator == null)
                throw new Exception($"Аниматор не найден на калимбе!");

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                throw new Exception($"Компонент источника звука не найден на калимбе!");

            _audioSource.Stop();
            _audioSource.volume = 0;
            _audioSource.dopplerLevel = 0;
            _audioSource.outputAudioMixerGroup = Main.Instance.AudioManager.GetSignalAudioMixerGroup();

            _kalimbaRoot = transform.Find(KalimbaRootGameObjectName);
            if (_kalimbaRoot == null)
                throw new Exception($"Рут калимбы не найден! - ({KalimbaRootGameObjectName})");

            var kalimbaBody = _kalimbaRoot.Find(KalimbaGameObjectName);
            if (kalimbaBody == null)
                throw new Exception($"Модель калимбы не найдена! - ({KalimbaRootGameObjectName})");

            Material kalimbaMat = Main.Instance.AssetManager.KalimbaViewModelMaterial; 
            kalimbaBody.Find(KalimbaMeshGameObjectName).GetComponent<SkinnedMeshRenderer>().material = kalimbaMat;

            _kalimbaAnimator = kalimbaBody.GetComponent<Animator>();
            if (_kalimbaAnimator == null)
                throw new Exception($"Аниматор калимбы не найден! - ({KalimbaRootGameObjectName})");
            _kalimbaAnimator.updateMode = AnimatorUpdateMode.Normal;
            _kalimbaAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            _audioLowPassFilter = _audioSource.gameObject.AddComponent<AudioLowPassFilter>();

            _audioSource.Play();

            IsInitialized = true;
        }

        public void Ready()
        {
            if(IsInitialized == false)
                throw new InvalidOperationException("Инициализация калимбы не закончена - калимба не готова.");

            IsReady = true;
            enabled = false;
            IsDrawed = false;
        }

        public void Equip()
        {
            IsDrawed = true;
            enabled = true;
            _rigAnimator.SetTrigger(_equipTriggerName);
        }

        public void Unequip()
        {
            if(IsPlaying)
            {
                IsPlaying = false;
                _kalimbaAnimator.SetTrigger(_stopTriggerName);
            }  
            
            _rigAnimator.SetTrigger(_unequipTriggerName);
            IsDrawed = false;
        }

        public void PlayTravelerTheme()
        {
            int samples = GetTimeSamples();

            _rigAnimator.SetTrigger(_playTriggerName);

            SetAudioTimeSamples(samples);

            _audioSource.volume = 0;

            if(_audioSource.isPlaying == false)
            _audioSource.Play();

            IsPlaying = true;
        }

        public void StopPlayingTravelerTheme()
        {
            IsPlaying = false;

            _rigAnimator.SetTrigger(_stopTriggerName);
            _kalimbaAnimator.SetTrigger(_stopTriggerName);
        }

        public void SetVolume(float volume)
        {
            _playingAudioSourceVolume = volume;
        }

        public void SetCutOff(float cutOffFreq)
        {
            _audioCutOffFreq = cutOffFreq;
        }

        public void SetSpatialBlend(float spatial)
        {
            _audioSource.spatialBlend = spatial;
        }

        private void HandleCutOffChange()
        {
            _audioLowPassFilter.cutoffFrequency = Mathf.Lerp(
                 _audioLowPassFilter.cutoffFrequency,
                 _audioCutOffFreq,
                 _audioCutOffFreqChangeFactor * Time.unscaledDeltaTime
            );
        }

        private void HandleAudioVolumeChange()
        {
            _audioSource.volume = Mathf.Lerp(
                _audioSource.volume,
                GetTargetVolume(),
                _audioSourceVolumeLerpFactor * Time.unscaledDeltaTime
            );
        }

        private static int GetTimeSamples()
        {
            int samples = 0;

            Main.Instance.AudioSynchronizer.TryGetTravelerThemeTimeSamples(out samples);
            return samples;
        }

        private void SetAudioTimeSamples(int timeSamples)
        {
            _audioSource.timeSamples = timeSamples;
        }

        private void PlaySynchronizelyAnimation()
        {
            float t = _audioSource.time % _audioSource.clip.length;
            float normalized = t / _audioSource.clip.length;

            _kalimbaAnimator.Play(_playStateName, 0, normalized);
            _kalimbaAnimator.Update(0f);
        }

        private float GetTargetVolume()
        {
            if (IsPlaying && _isGamePaused == false)
                return _playingAudioSourceVolume;
            else 
                return 0;
        }

        private void OnGameUnpaused()
        {
            _isGamePaused = false;
        }

        private void OnGamePaused()
        {
            _isGamePaused = true;
        }
    }
}
