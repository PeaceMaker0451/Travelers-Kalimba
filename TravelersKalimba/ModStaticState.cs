namespace TravelersKalimba
{
    public static class ModStaticState
    {
        public const string SignalAudioMixerGroup = "Signal";
        
        public const string EquipEventName = "EquipKalimba";
        public const string UnequipEventName = "UnequipKalimba";

        public const string StartPlayingEventName = "StartPlayingKalimba";
        public const string StopPlayingEventName = "StopPlayingKalimba";

        public const string KalimbaEquipStringId = "EQUIP_KALIMBA";
        public const string KalimbaUnequipStringId = "UNEQUIP_KALIMBA";
        public const string KalimbaPlayStringId = "PLAY_KALIMBA";

        public const string OWPlayerToolShader = "Outer Wilds/Utility/View Model";

        public static IInputCommands EquipKalimbaCommand { get; private set; } = InputLibrary.toolActionSecondary;
        public static IInputCommands PlayKalimbaCommand { get; private set; } = InputLibrary.lockOn;

        public static float EquipKalimbaButtonMaxPressDuration = 0.2f;

        public static float KalimbaVolume { get; private set; } = 0.4f;

        public static bool IsSignalscopeZoomShouldBeBlocked {  get; private set; } = false;

        public static void SetIsSignalscopeZoomShouldBeBlocked(bool isShouldBeBlocked)
        {
            IsSignalscopeZoomShouldBeBlocked = isShouldBeBlocked;
        }
    }
}
