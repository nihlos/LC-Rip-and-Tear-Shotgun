using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Rip_and_Tear_Shotgun
{

    [BepInPlugin(modGUID, modName, modVersion)]
    public class ShotgunBase : BaseUnityPlugin
    {
        private const string modGUID = "nihl.RipAndTearShotgun";
        private const string modName = "Rip & Tear Shotgun";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static ShotgunBase instance;

        internal ManualLogSource MLS;

        public static AssetBundle MainAssets;

        public static AudioClip doomMusic;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            MLS = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            if (MainAssets == null) 
            {
                MainAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "doomMusic"));
            }

            doomMusic = MainAssets.LoadAsset<AudioClip>("Assets/doomMusic.mp3");

            if (doomMusic != null)
            {
                MLS.LogInfo("Audio Loaded!");
            }

            MLS.LogInfo("Ready to Rip & Tear!");
            harmony.PatchAll(typeof(ShotgunBase));
        }
    }

    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunPatch
    {
        [HarmonyPatch("EquipItem")]
        [HarmonyPostfix]
        public static void EquipAudioPatch(ShotgunItem __instance)
        {
            AudioSource gunMusic = ((Component)__instance).gameObject.AddComponent<AudioSource>();
            gunMusic.clip = ShotgunBase.doomMusic;
            gunMusic.loop = true;
            gunMusic.Play();
        }

        [HarmonyPatch("StopUsingGun")]
        [HarmonyPostfix]
        public static void StopUsingGunAudioPatch(ShotgunItem __instance)
        {
            AudioSource gunMusic = ((Component)__instance).gameObject.AddComponent<AudioSource>();
            gunMusic.Stop();
        }
    }
}
