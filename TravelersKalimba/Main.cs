using HarmonyLib;
using OWML.ModHelper;
using System.IO;
using System.Reflection;

namespace TravelersKalimba;

public class Main : ModBehaviour
{
    private const string AssetBundlePath = "kalimba";

    public static Main Instance { get; private set; }
    public KalimbaAssetBundleManager AssetManager { get; private set; }
    public TravelerSynchronizationManager AudioSynchronizer { get; private set; }
    public OWAudioManager AudioManager { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        new Harmony("peacemkr_png.KalimbaMod").PatchAll(Assembly.GetExecutingAssembly());

        #if (DEBUG)
            ModHelper.Console.WriteLine("Дебаг билд");
        #endif

        string modPath = ModHelper.Manifest.ModFolderPath;
        string bundlePath = Path.Combine(modPath, AssetBundlePath);

        AssetManager = new KalimbaAssetBundleManager(bundlePath);
        AudioSynchronizer = gameObject.AddComponent<TravelerSynchronizationManager>();
        AudioManager = new OWAudioManager();
    }
}

