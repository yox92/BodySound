using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BodySound.Utils;
using HarmonyLib;
using UnityEngine;


namespace BodySound
{
   
    [BepInPlugin("com.spt.BodySound", "BodySound", "1.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource _logSource;
        public static ConfigEntry<int> WalkVolume;
        public static ConfigEntry<int> TurnVolume;
        public static ConfigEntry<int> GearVolume;
        public static ConfigEntry<int> StopVolume;
        public static ConfigEntry<int> SprintVolume;
        public static ConfigEntry<int> VaultVolume;
        public static ConfigEntry<int> JumpVolume;
        public static ConfigEntry<int> MedKitVolume;
        public static ConfigEntry<int> BandageKitVolume;
        public static ConfigEntry<int> SurgicalKitVolume;
        public static ConfigEntry<int> RollupKitVolume;
        public static ConfigEntry<int> InjectorKitVolume;
        public static ConfigEntry<int> BalmKitVolume;
        public static ConfigEntry<int> SplintKitVolume;
        public static ConfigEntry<int> PillsbottleKitVolume;
        public static ConfigEntry<bool> LootingVolume;
        public static ConfigEntry<bool> BagVolume;
        public static ConfigEntry<bool> HandGripVolume;
        public static ConfigEntry<bool> NotificationVolume;
        public static ConfigEntry<bool> EquipVolume;
        public static ConfigEntry<bool> PickUpVolume;
        public static ConfigEntry<bool> DropVolume;
        public static ConfigEntry<bool> OverVolume;
        public static ConfigEntry<bool> InstallVolume;
        public static ConfigEntry<bool> InspectorVolume;
        public static ConfigEntry<bool> SwitcherVolume;
        public static ConfigEntry<bool> WeaponInOutVolume;


        private static Harmony HarmonyInstance { get; set; }
        private void Awake()
        {
            _logSource = Logger;
                        
            /*********************
             * Body Audio
             *********************/
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
            VaultVolume = Config.Bind(
                "Body Audio",
                "Vault",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            
            /*********************
             * Medical Audio
             *********************/
            MedKitVolume = Config.Bind(
                "Medical Audio",
                "Medkit",
                100,
                new ConfigDescription("MedKit in Tarkov refers to healing items like Salewa, IFAK, AFAK [...]", new AcceptableValueRange<int>(1, 100))
            );
            BandageKitVolume = Config.Bind(
                "Medical Audio",
                "Bandage",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            SurgicalKitVolume = Config.Bind(
                "Medical Audio",
                "Surgical",
                100,
                new ConfigDescription("CMS Surv12", new AcceptableValueRange<int>(1, 100))
            );
            RollupKitVolume = Config.Bind(
                "Medical Audio",
                "Rollup",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            InjectorKitVolume = Config.Bind(
                "Medical Audio",
                "Injector",
                100,
                new ConfigDescription("Morphine, Adrenaline, Propital [...]", new AcceptableValueRange<int>(1, 100))
            );
            BalmKitVolume = Config.Bind(
                "Medical Audio",
                "Balm",
                100,
                new ConfigDescription("Golden star,Vaseline ", new AcceptableValueRange<int>(1, 100))
            );
            SplintKitVolume = Config.Bind(
                "Medical Audio",
                "Splint",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            PillsbottleKitVolume = Config.Bind(
                "Medical Audio",
                "PillsBottle",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            
            /*********************
             * In Game Audio
             *********************/
            LootingVolume = Config.Bind(
                "In Game Audio",
                "Looting",
                true,
                new ConfigDescription("Play sound during looting if enabled.")
            );
            BagVolume = Config.Bind(
                "In Game Audio",
                "Bag",
                true,
                new ConfigDescription("Play sound from bag interactions if enabled.")
            );
            HandGripVolume = Config.Bind(
                "In Game Audio",
                "HandGrip",
                true,
                new ConfigDescription("Play hand grip sounds if enabled.")
            );
            SwitcherVolume = Config.Bind(
                "In Game Audio",
                "Switcher",
                true,
                new ConfigDescription("Play weapon switcher sounds if enabled.")
            );
            WeaponInOutVolume = Config.Bind(
                "In Game Audio",
                "WeaponInOut",
                true,
                new ConfigDescription("Play weapon draw/holster sounds if enabled.")
            );
            
            
            /*********************
             * General Audio
             *********************/            
            EquipVolume = Config.Bind(
                "General Audio",
                "Equip",
                true,
                new ConfigDescription("Play 'equipment' stuff sounds on interface if enabled.")
            );
            PickUpVolume = Config.Bind(
                "General Audio",
                "PickUp",
                true,
                new ConfigDescription("Play click 'pickup' sounds on interface if enabled.")
            );
            DropVolume = Config.Bind(
                "General Audio",
                "Drop",
                true,
                new ConfigDescription("Play click 'drop' sounds on interface if enabled.")
            );
            OverVolume = Config.Bind(
                "General Audio",
                "Over",
                true,
                new ConfigDescription("Play 'over' click sounds on interface if enabled.")
            );
            InstallVolume = Config.Bind(
                "General Audio",
                "Install",
                true,
                new ConfigDescription("Play click 'installation' sounds on interface if enabled.")
            );
            InspectorVolume = Config.Bind(
                "General Audio",
                "Inspector",
                true,
                new ConfigDescription("Play 'inspector' sounds open interface if enabled.")
            );
            NotificationVolume = Config.Bind(
                "General Audio",
                "Notification",
                true,
                new ConfigDescription("Play 'Notification' sounds if enabled.")
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
            _logSource.LogInfo("[PatchMainMenuReady] PatchBodySoundVolume detected — initializing components");
            BodyLogger.Info("✅ [PatchMainMenuReady] PatchBodySoundVolume detected — initializing components");

            
        }
        
    }
}