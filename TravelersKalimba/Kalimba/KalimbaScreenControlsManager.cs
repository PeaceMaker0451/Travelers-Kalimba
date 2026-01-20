using UnityEngine;

namespace TravelersKalimba
{
    public class KalimbaScreenControlsManager: MonoBehaviour
    {        
        private PromptManager _promptManager;

        private ScreenPrompt _hidePrompt;
        private ScreenPrompt _showPrompt;
        private ScreenPrompt _playPrompt;

        private void Start()
        {
            _promptManager = Locator.GetPromptManager();
            AddControls();

            GlobalMessenger<Kalimba>.AddListener(ModStaticState.EquipEventName, new Callback<Kalimba>(OnEquipKalimba));
            GlobalMessenger<Kalimba>.AddListener(ModStaticState.UnequipEventName, new Callback<Kalimba>(OnUnequipKalimba));

            GlobalMessenger<OWRigidbody>.AddListener("EnterFlightConsole", new Callback<OWRigidbody>(this.OnEnterFlightConsole));
            GlobalMessenger.AddListener("ExitFlightConsole", new Callback(this.OnExitFlightConsole));

            GlobalMessenger<ProbeLauncher>.AddListener("ProbeLauncherEquipped", new Callback<ProbeLauncher>(this.OnProbeLauncherEqupped));
            GlobalMessenger<ProbeLauncher>.AddListener("ProbeLauncherUnequipped", new Callback<ProbeLauncher>(this.OnProbeLauncherUnequpped));

            GlobalMessenger.AddListener("EquipTranslator", new Callback(this.OnEquipTranslator));
            GlobalMessenger.AddListener("UnequipTranslator", new Callback(this.OnUnequipTranslator));

            GlobalMessenger<Campfire>.AddListener("EnterRoastingMode", new Callback<Campfire>(OnEnterRoasting));
            GlobalMessenger.AddListener("ExitRoastingMode", new Callback(OnStopRoasting));

            KalimbaReady();
        }

        private void OnEnterRoasting(Campfire arg1)
        {
            KalimbaNotReady();
        }

        private void OnStopRoasting()
        {
            KalimbaReady();
        }

        private void AddControls()
        {
            _showPrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, LocalizationManager.GetTranslatedString(ModStaticState.KalimbaEquipStringId));
            _hidePrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, LocalizationManager.GetTranslatedString(ModStaticState.KalimbaUnequipStringId));
            _playPrompt = new ScreenPrompt(InputLibrary.lockOn, LocalizationManager.GetTranslatedString(ModStaticState.KalimbaPlayStringId));

            _promptManager.AddScreenPrompt(_showPrompt, PromptPosition.UpperRight);
            _promptManager.AddScreenPrompt(_hidePrompt, PromptPosition.UpperRight);
            _promptManager.AddScreenPrompt(_playPrompt, PromptPosition.UpperRight);
        }

        private void OnEquipTranslator() 
        {
            KalimbaNotReady();
        }

        private void OnUnequipTranslator()
        {
            KalimbaReady();
        }

        private void OnProbeLauncherEqupped(ProbeLauncher arg1)
        {
            KalimbaNotReady();
        }

        private void OnProbeLauncherUnequpped(ProbeLauncher arg1)
        {
            KalimbaReady();
        }

        private void OnEnterFlightConsole(OWRigidbody arg1)
        {
            KalimbaNotReady();
        }

        private void OnExitFlightConsole()
        {
            KalimbaReady();
        }

        private void OnEquipKalimba(Kalimba kalimba)
        {
            KalimbaEquipped();
        }

        private void OnUnequipKalimba(Kalimba kalimba)
        {
            KalimbaReady();
        }


        private void KalimbaNotReady()
        {
            HideEquipKalimbaControl();
            HideHideKalimbaControl();
            HidePlayKalimbaControl();
        }
        
        private void KalimbaReady()
        {
            ShowEquipKalimbaControl();
            HideHideKalimbaControl();
            HidePlayKalimbaControl();
        }

        private void KalimbaEquipped()
        {
            HideEquipKalimbaControl();
            ShowHideKalimbaControl();
            ShowPlayKalimbaControl();
        }

        private void ShowEquipKalimbaControl()
        {
            _showPrompt.SetVisibility(true);
        }

        private void HideEquipKalimbaControl()
        {
            _showPrompt.SetVisibility(false);
        }

        private void ShowHideKalimbaControl()
        {
            _hidePrompt.SetVisibility(true);
        }

        private void HideHideKalimbaControl()
        {
            _hidePrompt.SetVisibility(false);
        }

        private void ShowPlayKalimbaControl()
        {
            _playPrompt.SetVisibility(true);
        }

        private void HidePlayKalimbaControl()
        {
            _playPrompt.SetVisibility(false);
        }
    }
}
