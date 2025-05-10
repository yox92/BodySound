using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BodySound.Utils;
using HarmonyLib;
using UnityEngine;


namespace BodySound
{
   
    [BepInPlugin("com.spt.BodySound", "BodySound", "1.0.3")]
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
        public static ConfigEntry<int> PillsBottleVolume;
        
        public static ConfigEntry<bool> EnableAllHooksOnlyInRaid;
        public static ConfigEntry<int> MaxWeight;
        
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
        public static ConfigEntry<bool> ThermalVolume;
        public static ConfigEntry<bool> HurtVolume;
        public static ConfigEntry<bool> BreathOkVolume;
        public static ConfigEntry<bool> VisorVolume;
        public static ConfigEntry<bool> DrillMagVolume;
        
        private static Harmony HarmonyInstance { get; set; }
        private void Awake()
        {
            _logSource = Logger;
            
            /*********************
             * Parameters
             *********************/       
            MaxWeight = Config.Bind(
                "1) Configuration",
                "Max Weight",
                95,
                new ConfigDescription("If you use an other mod to modify 'WEIGHT', correct the maximum WEIGHT HERE", new AcceptableValueRange<int>(1, 200))
            );
            EnableAllHooksOnlyInRaid = Config.Bind(
                "1) Configuration", 
                "Only On Raids",
                true,
                "If 'true', sound correction will only be active during raids"
            );

                        
            /*********************
             *    Body Audio
             *********************/
            WalkVolume = Config.Bind(
                "2) Body Audio",
                "Walk",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            TurnVolume = Config.Bind(
                "2) Body Audio",
                "Turn",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            SprintVolume = Config.Bind(
                "2) Body Audio",
                "Sprint",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            GearVolume = Config.Bind(
                "2) Body Audio",
                "Gear",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            StopVolume = Config.Bind(
                "2) Body Audio",
                "Stop",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            JumpVolume = Config.Bind(
                "2) Body Audio",
                "Jump",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            VaultVolume = Config.Bind(
                "2) Body Audio",
                "Vault",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            
            /*********************
             *   Medical Audio
             *********************/
            MedKitVolume = Config.Bind(
                "3) Medical Audio",
                "Medkit",
                100,
                new ConfigDescription("MedKit in Tarkov refers to healing items like Salewa, IFAK, AFAK [...]", new AcceptableValueRange<int>(1, 100))
            );
            BandageKitVolume = Config.Bind(
                "3) Medical Audio",
                "Bandage",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            SurgicalKitVolume = Config.Bind(
                "3) Medical Audio",
                "Surgical",
                100,
                new ConfigDescription("CMS, Surv12", new AcceptableValueRange<int>(1, 100))
            );
            RollupKitVolume = Config.Bind(
                "3) Medical Audio",
                "Rollup",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            InjectorKitVolume = Config.Bind(
                "3) Medical Audio",
                "Injector",
                100,
                new ConfigDescription("Morphine, Adrenaline, Propital [...]", new AcceptableValueRange<int>(1, 100))
            );
            BalmKitVolume = Config.Bind(
                "3) Medical Audio",
                "Balm",
                100,
                new ConfigDescription("Golden star,Vaseline ", new AcceptableValueRange<int>(1, 100))
            );
            SplintKitVolume = Config.Bind(
                "3) Medical Audio",
                "Splint",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
            PillsBottleVolume = Config.Bind(
                "3) Medical Audio",
                "PillsBottle",
                100,
                new ConfigDescription("", new AcceptableValueRange<int>(1, 100))
            );
    
            /*********************
             *   In Game Audio
             *********************/
            LootingVolume = Config.Bind(
                "4) In Game Audio",
                "Looting",
                true,
                new ConfigDescription("Play sound during looting if enabled.")
            );
            BagVolume = Config.Bind(
                "4) In Game Audio",
                "Bag",
                true,
                new ConfigDescription("Play sound from bag interactions if enabled.")
            );
            HandGripVolume = Config.Bind(
                "4) In Game Audio",
                "HandGrip",
                true,
                new ConfigDescription("Play hand grip sounds if enabled.")
            );
            SwitcherVolume = Config.Bind(
                "4) In Game Audio",
                "Switcher",
                true,
                new ConfigDescription("Play weapon switcher sounds if enabled.")
            );
            WeaponInOutVolume = Config.Bind(
                "4) In Game Audio",
                "WeaponInOut",
                true,
                new ConfigDescription("Play weapon draw/holster sounds if enabled.")
            );
            ThermalVolume = Config.Bind(
                "4) In Game Audio",
                "Nocturne on/off",
                true,
                new ConfigDescription("Play Nocturne des/activate if enabled.")
            );
            HurtVolume = Config.Bind(
                "4) In Game Audio",
                "Hurt breath",
                true,
                new ConfigDescription("Play Hurt and near dead sound if enabled.")
            );
            BreathOkVolume = Config.Bind(
                "4) In Game Audio",
                "Visor breath",
                true,
                new ConfigDescription("Play Visor breath if enabled.")
            );
            VisorVolume = Config.Bind(
                "4) In Game Audio",
                "Visor on/off",
                true,
                new ConfigDescription("Play Visor on / off if enabled.")
            );
            
            
            /*********************
             *  General Audio
             *********************/            
            EquipVolume = Config.Bind(
                "5) General Audio",
                "Equip",
                true,
                new ConfigDescription("Play 'equipment' stuff sounds on interface if enabled.")
            );
            PickUpVolume = Config.Bind(
                "5) General Audio",
                "PickUp",
                true,
                new ConfigDescription("Play click 'pickup' sounds on interface if enabled.")
            );
            DropVolume = Config.Bind(
                "5) General Audio",
                "Drop",
                true,
                new ConfigDescription("Play click 'drop' sounds on interface if enabled.")
            );
            OverVolume = Config.Bind(
                "5) General Audio",
                "Over",
                true,
                new ConfigDescription("Play 'over' click sounds on interface if enabled.")
            );
            InstallVolume = Config.Bind(
                "5) General Audio",
                "Install",
                true,
                new ConfigDescription("Play click 'installation' sounds on interface if enabled.")
            );
            InspectorVolume = Config.Bind(
                "5) General Audio",
                "Inspector",
                true,
                new ConfigDescription("Play 'inspector' sounds open/close interface if enabled.")
            );
            NotificationVolume = Config.Bind(
                "5) General Audio",
                "Notification",
                true,
                new ConfigDescription("Play 'Notification' sounds if enabled.")
            );
            DrillMagVolume = Config.Bind(
                "5) General Audio",
                "Drill Magazine",
                true,
                new ConfigDescription("Play '(Un)load' sounds if enabled.")
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