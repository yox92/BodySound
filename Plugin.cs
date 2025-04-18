using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BodySound.Utils;
using HarmonyLib;
using UnityEngine;


namespace BodySound
{
    [BepInPlugin("com.spt.BodySound", "BodySound", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LOGSource;
        public static ConfigEntry<int> WalkVolume;
        public static ConfigEntry<int> TurnVolume;
        public static ConfigEntry<int> GearVolume;
        public static ConfigEntry<int> StopVolume;
        public static ConfigEntry<int> SprintVolume;
        public static ConfigEntry<int> JumpVolume;


        private static Harmony HarmonyInstance { get; set; }
        private void Awake()
        {
            LOGSource = Logger;
            WalkVolume = Config.Bind(
                "Body Audio",
                "Walk",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            TurnVolume = Config.Bind(
                "Body Audio",
                "Turn",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            GearVolume = Config.Bind(
                "Body Audio",
                "Gear",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            StopVolume = Config.Bind(
                "Body Audio",
                "Stop",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            SprintVolume = Config.Bind(
                "Body Audio",
                "Sprint",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            JumpVolume = Config.Bind(
                "Body Audio",
                "Jump",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
         
            if (!File.Exists(PathsFile.DebugPath))
            {
                File.WriteAllText(PathsFile.DebugPath, "false");
            }
            
            if (!File.Exists(PathsFile.LogFilePath))
            {
                File.WriteAllText(PathsFile.LogFilePath, "");
            }
            
            Logger.LogInfo("Log dans le fichier :" + PathsFile.LogFilePath);
            
            BodyLogger.Init(EnumLoggerMode.MemoryBuffer);
           
            Application.quitting += BodyLogger.OnApplicationQuit;
            
            HarmonyInstance = new Harmony("com.spt.BodySound");
            HarmonyInstance.PatchAll();
            LOGSource.LogInfo("[PatchMainMenuReady] PatchBodySoundVolume detected — initializing components");
            BodyLogger.Info("✅ [PatchMainMenuReady] PatchBodySoundVolume detected — initializing components");

            
        }
        
    }
}