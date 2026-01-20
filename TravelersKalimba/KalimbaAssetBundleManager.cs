using OWML.Common;
using System;
using System.Text;
using UnityEngine;

namespace TravelersKalimba
{
    public class KalimbaAssetBundleManager
    {

        private const string KalimbaPrefabName = "Kalimba";

        private const string KalimbaTravelerThemeName = "OW_TravelerTheme_kalimba";

        private const string DrawedClipName = "KalimbaDrawed";
        private const string HidedClipName = "KalimbaHided";
        private const string DrawClipName = "DrawKalimba";
        private const string HideClipName = "HideKalimba";

        private const string AlbedoMapName = "Kalimba_Kalimba_Albedo.png";
        private const string SmothnessMetallicMapName = "Kalimba_Kalimba_MetallicSmoothness.png";
        private const string OcclusionMapName = "Kalimba_Kalimba_Occlusion.png";
        private const string NormalMapName = "RGBKalimba_Kalimba_Normal.png";

        public AssetBundle KalimbaBundle { get; private set; }

        public AnimationClip KalimbaDrawedClip { get; private set; }
        public AnimationClip KalimbaDrawClip { get; private set; }
        public AnimationClip KalimbaHidedClip { get; private set; }
        public AnimationClip KalimbaHideClip { get; private set; }

        public Texture2D KalimbaAlbedoMap { get; private set; }
        public Texture2D KalimbaSmothnessMetallicMap { get; private set; }
        public Texture2D KalimbaOcclusionMap { get; private set; }
        public Texture2D KalimbaNormalMap { get; private set; }

        public AudioClip KalimbaTheme {  get; private set; }

        public Material KalimbaViewModelMaterial { get; private set; }

        public KalimbaAssetBundleManager(string bundlePath)
        {
            KalimbaBundle = AssetBundle.LoadFromFile(bundlePath);

            if (KalimbaBundle == null)
                throw new Exception("Asset Bundle не может быть загружен");

            #if (DEBUG)
                Main.Instance.ModHelper.Console.WriteLine(GetBundleLog(KalimbaBundle));
            #endif  

			LoadAnimationClips();
            LoadKalimbaTextures();

            KalimbaTheme = KalimbaBundle.LoadAsset<AudioClip>(KalimbaTravelerThemeName);
            if(KalimbaTheme == null)
                Main.Instance.ModHelper.Console.WriteLine($"Аудиоклип {KalimbaTravelerThemeName} не может быть загружен!", MessageType.Error);

            KalimbaViewModelMaterial = CreateViewModelMaterial(Color.white, KalimbaAlbedoMap, KalimbaSmothnessMetallicMap, 1, KalimbaNormalMap, 0.5f, KalimbaOcclusionMap, null, Color.black);
        }

        public GameObject LoadKalimbaPrefab()
        {
            return KalimbaBundle.LoadAsset<GameObject>(KalimbaPrefabName);
        }

        public static Material CreateViewModelMaterial(
            Color color,
            Texture2D albedo,
            Texture2D metallicGloss,
            float smoothnessScale,
            Texture2D normalMap,
            float normalScale,
            Texture2D occlusion,
            Texture2D emissionMap,
            Color emissionColor)
        {
            Shader shader = Shader.Find(ModStaticState.OWPlayerToolShader);

            if (shader == null)
            {
                throw new Exception($"Не удалось найти исходный шейдер - {ModStaticState.OWPlayerToolShader}");
            }

            Material material = new Material(shader);

            material.SetColor("_Color", color);
            material.SetTexture("_MainTex", albedo);

            material.SetTexture("_MetallicGlossMap", metallicGloss);
            material.SetFloat("_GlossMapScale", smoothnessScale);

            material.SetTexture("_BumpMap", normalMap);
            material.SetFloat("_BumpScale", normalScale);

            material.SetTexture("_OcclusionMap", occlusion);

            material.SetTexture("_EmissionMap", emissionMap);
            material.SetColor("_EmissionColor", emissionColor);

            if (emissionMap != null || emissionColor.maxColorComponent > 0f)
            {
                material.EnableKeyword("_EMISSION");
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }

            return material;
        }

        private string GetBundleLog(AssetBundle bundle)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Бандл \"{bundle.name}\"");
            sb.AppendLine($"Содержащиеся ассеты:");

            string[] assetNames = bundle.GetAllAssetNames();

            foreach (string assetName in assetNames)
            {
                sb.AppendLine($"- {assetName}");
            }

            return sb.ToString();
        }

        private void LoadAnimationClips()
        {
            KalimbaDrawedClip = LoadAnimationClip(DrawedClipName);
            KalimbaDrawClip = LoadAnimationClip(DrawClipName);
            KalimbaHidedClip = LoadAnimationClip(HidedClipName);
            KalimbaHideClip = LoadAnimationClip(HideClipName);
        }

        private void LoadKalimbaTextures()
        {
            KalimbaAlbedoMap = KalimbaBundle.LoadAsset<Texture2D>(AlbedoMapName);
            if(KalimbaAlbedoMap == null)
                Main.Instance.ModHelper.Console.WriteLine($"Карта {AlbedoMapName} не может быть загружена!", MessageType.Error);

            KalimbaSmothnessMetallicMap = KalimbaBundle.LoadAsset<Texture2D>(SmothnessMetallicMapName);
            if (KalimbaSmothnessMetallicMap == null)
                Main.Instance.ModHelper.Console.WriteLine($"Карта {SmothnessMetallicMapName} не может быть загружена!", MessageType.Error);

            KalimbaOcclusionMap = KalimbaBundle.LoadAsset<Texture2D>(OcclusionMapName);
            if (KalimbaOcclusionMap == null)
                Main.Instance.ModHelper.Console.WriteLine($"Карта {OcclusionMapName} не может быть загружена!", MessageType.Error);

            KalimbaNormalMap = KalimbaBundle.LoadAsset<Texture2D>(NormalMapName);
            if (KalimbaNormalMap == null)
                Main.Instance.ModHelper.Console.WriteLine($"Карта {NormalMapName} не может быть загружена!", MessageType.Error);
        }

        private AnimationClip LoadAnimationClip(string name)
        {
            var clip = KalimbaBundle.LoadAsset<AnimationClip>(name);
            
            if(clip == null)
            {
                Main.Instance.ModHelper.Console.WriteLine($"Клип анимации {name} не может быть загружен!", MessageType.Error);
                return null;
            }

            return clip;
        }

    }
}
