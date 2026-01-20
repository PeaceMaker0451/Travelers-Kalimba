using System;
using UnityEngine;

namespace TravelersKalimba
{
    public class KalimbaManager : MonoBehaviour
    {
        private const string SignalScopeBodyGameObjectName = "Props_HEA_Signalscope";
        private const string SignalScopeBodyNewparentGameObjectName = "SignalScopeBodyPosOverride";
        private const string CameraDetectorGameObjectName = "CameraDetector";

        private float _secondaryKalimbaSpatialBlend = 0.2f;
        private float _primaryKalimbaSpatialBlend = 1;

        private Kalimba _primaryKalimba;
        private Transform _primaryKalimbaSignalscopeNewParent;
        private Kalimba _secondaryKalimba;

        private Transform _playerCamera;
        private PlayerCameraFluidDetector _fluidDetector;
        private Signalscope _signalscope;
        private Transform _signalscopeBody;

        KalimbaScreenControlsManager ControlsManager;

        private float _equipButtonPressedDuration;
        private bool _equipButtonPressed;
        
        private bool _isInitialized = false;
        private bool _canEquipSecondaryKalimba = true;
        private bool _canEquipPrimaryKalimba = true;
        private bool _isSignalscopeEquiped = false;
        private bool _isAtCampfire = false;

        private void Start()
        {
            var kalimbaPrefab = Main.Instance.AssetManager.LoadKalimbaPrefab();

            if (kalimbaPrefab == null)
                throw new Exception("Префаб калимбы пуст!");

            _playerCamera = transform;
            if (_playerCamera == null)
                throw new Exception("PlayerCamera не может быть найдена!");

            var cameraDetector = _playerCamera.Find(CameraDetectorGameObjectName);
            if (cameraDetector == null)
                throw new Exception("CameraDetector не может быть найден!");

            _fluidDetector = cameraDetector.GetComponent<PlayerCameraFluidDetector>();
            if (_fluidDetector == null)
                throw new Exception("PlayerCameraFluidDetector не может быть найден!");

            var secondaryKalimbaGameObject = Instantiate(kalimbaPrefab, _playerCamera);
            _secondaryKalimba = SetupKalimba(secondaryKalimbaGameObject);
            _secondaryKalimba.SetSpatialBlend(_secondaryKalimbaSpatialBlend);

            _signalscope = transform.GetComponentInChildren<Signalscope>();
            if (_signalscope == null)
                throw new Exception("Сигналоскоп Не может быть найден!");

            var primaryKalimbaGameObject = Instantiate(kalimbaPrefab, _signalscope.transform);
            _primaryKalimba = SetupKalimba(primaryKalimbaGameObject);
            _primaryKalimba.SetSpatialBlend(_primaryKalimbaSpatialBlend);

            _primaryKalimbaSignalscopeNewParent = _primaryKalimba.transform.Find(SignalScopeBodyNewparentGameObjectName);
            if (_primaryKalimbaSignalscopeNewParent == null)
                throw new Exception($"{SignalScopeBodyNewparentGameObjectName} Не может быть найден!");

            _signalscopeBody = _signalscope.transform.Find(SignalScopeBodyGameObjectName);
            if (_signalscopeBody == null)
                throw new Exception($"{SignalScopeBodyGameObjectName} Не может быть найден!");

            _signalscopeBody.SetParent(_primaryKalimbaSignalscopeNewParent);

            GlobalMessenger<Signalscope>.AddListener("EquipSignalscope", new Callback<Signalscope>(OnEquipSignalscope));
            GlobalMessenger.AddListener("UnequipSignalscope", new Callback(OnUnequipSignalscope));

            GlobalMessenger<ProbeLauncher>.AddListener("ProbeLauncherEquipped", new Callback<ProbeLauncher>(this.OnProbeLauncherEqupped));
            GlobalMessenger<ProbeLauncher>.AddListener("ProbeLauncherUnequipped", new Callback<ProbeLauncher>(this.OnProbeLauncherUnequpped));

            GlobalMessenger.AddListener("EquipTranslator", new Callback(this.OnEquipTranslator));
            GlobalMessenger.AddListener("UnequipTranslator", new Callback(this.OnUnequipTranslator));

            GlobalMessenger<OWRigidbody>.AddListener("EnterFlightConsole", new Callback<OWRigidbody>(this.OnEnterFlightConsole));
            GlobalMessenger.AddListener("ExitFlightConsole", new Callback(this.OnExitFlightConsole));

            GlobalMessenger<Campfire>.AddListener("EnterRoastingMode", new Callback<Campfire>(OnEnterRoasting));
            GlobalMessenger.AddListener("ExitRoastingMode", new Callback(OnStopRoasting));

            ControlsManager = gameObject.AddComponent<KalimbaScreenControlsManager>();

            _isInitialized = true;
        }

        private void Update()
        {
            if (_isInitialized == false)
                return;
            
            if (_secondaryKalimba.IsPlaying && _fluidDetector != null)
            {
                HandleVolumeChange();
                HandleCutOffChange();
            }

            if (OWInput.IsNewlyPressed(ModStaticState.EquipKalimbaCommand, InputMode.Character))
            {
                _equipButtonPressedDuration = 0;
                _equipButtonPressed = true;

                return;
            }

            if(_equipButtonPressed)
            {
                _equipButtonPressedDuration += Time.deltaTime;
            }

            if(OWInput.IsNewlyReleased(ModStaticState.EquipKalimbaCommand, InputMode.Character))
            {
                _equipButtonPressed = false;

                if (_equipButtonPressedDuration <= ModStaticState.EquipKalimbaButtonMaxPressDuration)
                {
                    if (_isSignalscopeEquiped)
                    {
                        if (_primaryKalimba.IsDrawed == false)
                            DrawPrimary();
                        else
                            HidePrimary();
                    }
                    else
                    {
                        if (_secondaryKalimba.IsDrawed == false)
                            DrawSecondary();
                        else
                            HideSecondary();
                    }
                }

                return;
            }

            if (OWInput.IsNewlyPressed(ModStaticState.PlayKalimbaCommand, InputMode.Character))
            {
                if (_primaryKalimba.IsDrawed && _primaryKalimba.IsPlaying == false)
                {
                    PlayPrimary();
                }
                else if (_secondaryKalimba.IsDrawed && _secondaryKalimba.IsPlaying == false)
                {
                    PlaySecondary();
                }
            }
            else if (OWInput.IsNewlyReleased(ModStaticState.PlayKalimbaCommand, InputMode.Character))
            {
                if (_primaryKalimba.IsDrawed && _primaryKalimba.IsPlaying)
                {
                    StopPrimary();
                }
                else if (_secondaryKalimba.IsDrawed && _secondaryKalimba.IsPlaying)
                {
                    StopSecondary();
                }
            }
        }

        private void OnDestroy()
        {
            GlobalMessenger<Signalscope>.RemoveListener("EquipSignalscope", new Callback<Signalscope>(OnEquipSignalscope));
            GlobalMessenger.RemoveListener("UnequipSignalscope", new Callback(OnUnequipSignalscope));
        }

        private void HandleCutOffChange()
        {
            float cutoffFreq = 22000f;

            if (_fluidDetector.InFluidType(FluidVolume.Type.WATER))
                cutoffFreq = 800f;
            else if (_fluidDetector.InFluidType(FluidVolume.Type.FOG))
                cutoffFreq = 5000f;
            else if (_fluidDetector.InFluidType(FluidVolume.Type.SAND))
                cutoffFreq = 3000f;
            else if (_fluidDetector.InFluidType(FluidVolume.Type.AIR))
                cutoffFreq = 22000f;
            else
                cutoffFreq = 500f;

            _secondaryKalimba.SetCutOff(cutoffFreq);
        }

        private void HandleVolumeChange()
        {
            if (_fluidDetector.InFluidType(FluidVolume.Type.AIR) || _fluidDetector.InFluidType(FluidVolume.Type.WATER) || _fluidDetector.InFluidType(FluidVolume.Type.FOG) || _fluidDetector.InFluidType(FluidVolume.Type.SAND))
                _secondaryKalimba.SetVolume(ModStaticState.KalimbaVolume);
            else
                _secondaryKalimba.SetVolume(0);
        }

        private void DrawPrimary()
        {
            if (_primaryKalimba.IsDrawed || _canEquipPrimaryKalimba == false || _isAtCampfire)
                return;

            HideSecondary();

            _primaryKalimba.Equip();
            ModStaticState.SetIsSignalscopeZoomShouldBeBlocked(true);
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.EquipEventName, _primaryKalimba);
        }

        private void HidePrimary()
        {
            if (_primaryKalimba.IsDrawed == false)
                return;

            _primaryKalimba.Unequip();
            ModStaticState.SetIsSignalscopeZoomShouldBeBlocked(false);
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.UnequipEventName, _primaryKalimba);
        }

        private void PlayPrimary()
        {
            if (_primaryKalimba.IsPlaying)
                return;
            
            _primaryKalimba.PlayTravelerTheme();
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.StartPlayingEventName, _primaryKalimba);
        }

        private void StopPrimary()
        {
            if (_primaryKalimba.IsPlaying == false)
                return;

            _primaryKalimba.StopPlayingTravelerTheme();
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.StopPlayingEventName, _primaryKalimba);
        }

        private void DrawSecondary()
        {
            if (_secondaryKalimba.IsDrawed || _canEquipSecondaryKalimba == false || _isAtCampfire)
                return;

            HidePrimary();
            _secondaryKalimba.Equip();
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.EquipEventName, _secondaryKalimba);
        }

        private void HideSecondary()
        {
            if (_secondaryKalimba.IsDrawed == false)
                return;

            _secondaryKalimba.Unequip();
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.UnequipEventName, _secondaryKalimba);
        }

        private void PlaySecondary()
        {
            if (_secondaryKalimba.IsPlaying)
                return;

            _secondaryKalimba.PlayTravelerTheme();
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.StartPlayingEventName, _secondaryKalimba);
        }

        private void StopSecondary()
        {
            if (_secondaryKalimba.IsPlaying == false)
                return;

            _secondaryKalimba.StopPlayingTravelerTheme();
            GlobalMessenger<Kalimba>.FireEvent(ModStaticState.StopPlayingEventName, _secondaryKalimba);
        }

        private Kalimba SetupKalimba(GameObject kalimbaGameObject)
        {
            Kalimba kalimba = kalimbaGameObject.AddComponent<Kalimba>();

            kalimba.Initialize();
            kalimba.Ready();
            kalimba.SetVolume(ModStaticState.KalimbaVolume);

            return kalimba;
        }

        private void OnEquipSignalscope(Signalscope signalscope)
        {
            _isSignalscopeEquiped = true;
            HideSecondary();
        }

        private void OnUnequipSignalscope()
        {
            _isSignalscopeEquiped = false;
            
            if(_primaryKalimba.IsDrawed)
            HidePrimary();
        }

        private void OnProbeLauncherEqupped(ProbeLauncher arg1)
        {
            _canEquipSecondaryKalimba = false;
            HideSecondary();
        }

        private void OnProbeLauncherUnequpped(ProbeLauncher arg1)
        {
            _canEquipSecondaryKalimba = true;
        }

        private void OnEquipTranslator()
        {
            _canEquipSecondaryKalimba = false;
            HideSecondary();
        }

        private void OnUnequipTranslator()
        {
            _canEquipSecondaryKalimba = true;
        }

        private void OnEnterFlightConsole(OWRigidbody arg1)
        {
            _canEquipSecondaryKalimba = false;
            _canEquipPrimaryKalimba = false;
            HideSecondary();
        }

        private void OnExitFlightConsole()
        {
            _canEquipSecondaryKalimba = true;
            _canEquipPrimaryKalimba = true;
        }


        private void OnEnterRoasting(Campfire arg1)
        {
            _isAtCampfire = true;
            HideSecondary();
        }

        private void OnStopRoasting()
        {
            _isAtCampfire = false;
        }
    }
}
