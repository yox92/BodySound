using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Audio.ReverbSubsystem;
using BepInEx.Configuration;
using BodySound.Patches;
using UnityEngine;
using EFT;

namespace BodySound.Utils
{
    public static class AudioUtils
    {
        private const float MinWeight = 4f;

        private static readonly Regex RecognizedAudioBodyRegex = new(
            @"(walk|gear|stop|jump|landing|turn|vault|sprint)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static readonly Regex RecognizedAudioMedical = new(
            @"(_medkit_|_bandage_|_surgicalkit_|_rollupkit_|_injector_|_zvezda_|_splint_|_pillsbottle_)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static readonly Regex HurtRegex = new(
            @"usec.*breath.*(hurt|neardeath)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static readonly Regex BreathOkRegex = new(
            @"usec.*breath.*ok",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );


        public static bool Hurt(string name)
        {
            return !string.IsNullOrEmpty(name) && HurtRegex.IsMatch(name) && !Plugin.HurtVolume.Value;
        }

        private static bool BreathOk(string name)
        {
            return !string.IsNullOrEmpty(name) && BreathOkRegex.IsMatch(name);
        }

        private static bool Visor(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Equals("glassshield_on", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("glassshield_off", StringComparison.OrdinalIgnoreCase);
        }


        public static bool IsLootingAudio(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("looting");
        }

        private static bool IsBag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Equals("backpack_close", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("backpack_open", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHandGrip(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("hand_grip");
        }

        private static bool Notification(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("notification");
        }

        private static bool Switcher(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Contains("weapon") && name.Contains("switcher");
        }

        private static bool Use(string name)
        {
            return !string.IsNullOrEmpty(name) && name.EndsWith("_use", StringComparison.OrdinalIgnoreCase);
        }

        private static bool Pickup(string name)
        {
            return !string.IsNullOrEmpty(name) && name.EndsWith("_pickup", StringComparison.OrdinalIgnoreCase);
        }


        private static bool Drop(string name)
        {
            return !string.IsNullOrEmpty(name) && name.EndsWith("_drop", StringComparison.OrdinalIgnoreCase);
        }

        private static bool Over(string name)
        {
            return !string.IsNullOrEmpty(name) && name.EndsWith("_over", StringComparison.OrdinalIgnoreCase);
        }

        private static bool Install(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_install_");
        }

        private static bool Inspector(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Contains("_inspector_") ||
                   name.Equals("menu_escape", StringComparison.OrdinalIgnoreCase);
        }

        private static bool WeaponInOut(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Equals("weap_out", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("weap_in", StringComparison.OrdinalIgnoreCase);
        }

        private static bool Thermal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Equals("thermal_activate", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("thermal_deactivate", StringComparison.OrdinalIgnoreCase);
        }

        private static bool DrillMag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.StartsWith("ammo_unload", StringComparison.OrdinalIgnoreCase) ||
                   name.StartsWith("ammo_load", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieves the volume for a given audio body name from the plugin configuration.
        /// </summary>
        /// <param name="name">The name of the audio clip.</param>
        /// <returns>Configured volume for the audio, as a float between 0 and 1.</returns>
        private static float GetVolumeMedicFromConfig(string name) =>
            name switch
            {
                _ when name.Contains("_medkit_") => Plugin.MedKitVolume.Value,
                _ when name.Contains("_bandage_") => Plugin.BandageKitVolume.Value,
                _ when name.Contains("_surgicalkit_") => Plugin.SurgicalKitVolume.Value,
                _ when name.Contains("_rollupkit_") => Plugin.RollupKitVolume.Value,
                _ when name.Contains("_injector_") => Plugin.InjectorKitVolume.Value,
                _ when name.Contains("_zvezda_") => Plugin.BalmKitVolume.Value,
                _ when name.Contains("_splint_") => Plugin.SplintKitVolume.Value,
                _ when name.Contains("_pillsbottle_") => Plugin.PillsBottleVolume.Value,
                _ => 100
            } / 100f;
        
        /// <summary>
        /// Retrieves the volume for a given audio medical name from the plugin configuration.
        /// </summary>
        /// <param name="name">The name of the audio clip.</param>
        /// <returns>Configured volume for the audio, as a float between 0 and 1.</returns>
        private static float GetVolumeBodyFromConfig(string name) =>
            name switch
            {
                _ when name.Contains("walk") => Plugin.WalkVolume.Value,
                _ when name.Contains("turn") => Plugin.TurnVolume.Value,
                _ when name.Contains("gear") => Plugin.GearVolume.Value,
                _ when name.Contains("stop") => Plugin.StopVolume.Value,
                _ when name.Contains("sprint") => Plugin.SprintVolume.Value,
                _ when name.Contains("vault") => Plugin.VaultVolume.Value,
                _ when name.Contains("jump") || name.Contains("landing")
                    => Plugin.JumpVolume.Value,
                _ => 100
            } / 100f;
        
        /// <summary>
        /// Retrieves both the volume and type for a given clip name.
        /// </summary>
        /// <param name="clipName">The name of the clip.</param>
        /// <returns>ClipVolumeInfo including name, volume, and skipWeightAdjust flag.</returns>
        public static ClipVolumeInfo GetClipVolumeInfo(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
                return new ClipVolumeInfo(string.Empty, 1f, false);

            var bodyVolume = GetVolumeBodyFromConfig(clipName);
            if (!Mathf.Approximately(bodyVolume, 1f))
                return new ClipVolumeInfo(clipName, bodyVolume, !Mathf.Approximately(bodyVolume, 1f));

            var medicVolume = GetVolumeMedicFromConfig(clipName);
            if (!Mathf.Approximately(medicVolume, 1f))
                return new ClipVolumeInfo(clipName, medicVolume, false); // Medic sounds

            // Default case
            return new ClipVolumeInfo(clipName, 1f, false);
        }

        /// <summary>
        /// Retrieves the current GameWorld instance dynamically or from cache.
        /// </summary>
        /// <returns>The GameWorld instance or null if not found.</returns>
        private static GameWorld GetGameWorld(List<string> logs)
        {
            if (PatchBodySoundVolume.CachedGameWorld != null)
                return PatchBodySoundVolume.CachedGameWorld;

            PatchBodySoundVolume.CachedGameWorld = UnityEngine.Object.FindObjectOfType<GameWorld>();
            BodyLogger.AddLogSafe(ref logs, PatchBodySoundVolume.CachedGameWorld != null
                ? $"✅ GameWorld found dynamically: {PatchBodySoundVolume.CachedGameWorld.name}"
                : "❌ GameWorld not found via FindObjectOfType.");

            return PatchBodySoundVolume.CachedGameWorld;
        }

        /// <summary>
        /// Attempts to retrieve the player associated with the given audio source by searching through 
        /// GameObject hierarchy (Player or PlayerOwner components), or finding the nearest player in a scene.
        /// </summary>
        /// <param name="audioSource">The audio source to analyze.</param>
        /// <param name="logs">List to store diagnostic information.</param>
        /// <returns>The associated player or null if not found.</returns>
        private static Player GetPlayer(
            BetterSource audioSource,
            List<string> logs)
        {
            try
            {
                var directPlayer = GetDirectPlayer(audioSource, logs);
                if (directPlayer != null)
                {
                    BodyLogger.AddLogSafe(ref logs,
                        $"[GetPlayer] 🚀 Direct player retrieved: {directPlayer.Profile?.Nickname}");
                    return directPlayer;
                }

                BodyLogger.AddLogSafe(ref logs, $"[GetPlayer] ❌ No player detected.");
                return null;
            }
            catch (Exception ex)
            {
                BodyLogger.AddLogSafe(ref logs, $"[GetPlayer] 🚨 Exception while detecting player: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the player directly associated with the specified audio source by searching through GameObject hierarchy and nearby players.
        /// </summary>
        /// <param name="audioSource">The audio source whose hierarchy and surroundings are analyzed.</param>
        /// <param name="logs">log list</param>
        /// <returns>The associated player component, or null if not found.</returns>
        private static Player GetDirectPlayer(BetterSource audioSource, List<string> logs)
        {
            if (audioSource == null || audioSource.transform == null)
                return null;

            // 1. Search for a Player component directly in the GameObject or its parents
            var player = audioSource.GetComponentInParent<Player>();
            if (player != null)
                return player;

            // 2. If not found, look for a PlayerOwner component
            var owner = audioSource.GetComponentInParent<PlayerOwner>();
            if (owner != null)
                return owner.Player;

            // 3. If still not found, try to find the closest player in the scene
            var nearest = FindNearestPlayer(audioSource, 3f, logs); // Small radius to avoid errors
            return nearest;
        }

        public static bool IsBodyAudio(string clipName1, string clipName2, List<string> logs)
        {
            var match1 = !string.IsNullOrEmpty(clipName1) && RecognizedAudioBodyRegex.IsMatch(clipName1);
            var match2 = !string.IsNullOrEmpty(clipName2) && RecognizedAudioBodyRegex.IsMatch(clipName2);
            BodyLogger.AddLogSafe(ref logs, $"[IsBodyAudio] clip 1 ? : {match1}");
            BodyLogger.AddLogSafe(ref logs, $"[IsBodyAudio] clip 2 ? : {match2}");
            return match1 || match2;
        }

        public static bool IsMedicalAudio(string clipName1, string clipName2, List<string> logs)
        {
            var match1 = !string.IsNullOrEmpty(clipName1) && RecognizedAudioMedical.IsMatch(clipName1);
            var match2 = !string.IsNullOrEmpty(clipName2) && RecognizedAudioMedical.IsMatch(clipName2);
            BodyLogger.AddLogSafe(ref logs, $"[IsMedicalAudio] clip 1 ? : {match1}");
            BodyLogger.AddLogSafe(ref logs, $"[IsMedicalAudio] clip 2 ? : {match2}");
            return match1 || match2;
        }
        // Dictionnaire pour vérifier efficacement les types d'interface et leurs volumes
        private static readonly Dictionary<Func<string, bool>, (ConfigEntry<bool> ConfigEntry, string TypeName)>
            UiSoundCheckers =
                new Dictionary<Func<string, bool>, (BepInEx.Configuration.ConfigEntry<bool>, string)>
                {
                    { IsBag, (Plugin.BagVolume, "Bag") },
                    { Notification, (Plugin.NotificationVolume, "Notification") },
                    { Use, (Plugin.EquipVolume, "Equip") },
                    { Pickup, (Plugin.PickUpVolume, "PickUp") },
                    { Drop, (Plugin.DropVolume, "Drop") },
                    { Over, (Plugin.OverVolume, "Over") },
                    { Install, (Plugin.InstallVolume, "Install") },
                    { Inspector, (Plugin.InspectorVolume, "Inspector") },
                    { Visor, (Plugin.VisorVolume, "Visor") },
                    { Thermal, (Plugin.ThermalVolume, "Thermal") },
                    { DrillMag, (Plugin.DrillMagVolume, "Unloading/Loading") }
                };

        public static bool UiInterfaceSound(string clipName, List<string> logs)
        {
            if (string.IsNullOrEmpty(clipName))
                return true;

            var checker = UiSoundCheckers.FirstOrDefault(c => c.Key(clipName));
            if (checker.Key == null || checker.Value.ConfigEntry.Value)
                return true;

            BodyLogger.AddLogSafe(ref logs,
                $"🔇 [AudioSource.PlayOneShot] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot ({checker.Value.TypeName})");
            return false;
        }

        // Dictionnaire pour vérifier efficacement les types de sons ingame et leurs volumes
        private static readonly Dictionary<Func<string, bool>, (ConfigEntry<bool> ConfigEntry, string TypeName)>
            InGameSoundCheckers =
                new Dictionary<Func<string, bool>, (ConfigEntry<bool>, string)>
                {
                    { IsHandGrip, (Plugin.HandGripVolume, "HandGrip") },
                    { Switcher, (Plugin.SwitcherVolume, "Switcher") },
                    { WeaponInOut, (Plugin.WeaponInOutVolume, "WeaponInOut") }
                };

        // Optimisation pour détecter si le son est un son ingame
        private static readonly Func<string, bool>[] InGameSoundChecks =
        {
            IsHandGrip, Switcher, WeaponInOut
        };

        public static bool InGameSound(string clipName, AudioSource source, List<string> logs)
        {
            if (string.IsNullOrEmpty(clipName))
                return true;

            // Vérifie si c'est un son qui nous intéresse
            var isInGameSoundType = InGameSoundChecks.Any(check => check(clipName));

            // Si ce n'est pas un son d'ingame qui nous intéresse, on laisse passer
            if (!isInGameSoundType)
            {
                BodyLogger.AddLogSafe(ref logs, "[AudioSource.PlayOneShot] ❌ Not an ingame sound type, no adjustment needed.");
                return true;
            }
            
            // Vérifie les types spécifiques pour bloquer ou non via clipName uniquement
            foreach (var checker in InGameSoundCheckers
                         .Where(checker => checker.Key(clipName) && !checker.Value.ConfigEntry.Value))
            {
                // ➔ Ici, avant de bloquer réellement, on vérifie si c'est le joueur local
                if (!IsSoundFromLocalPlayer(source, logs))
                    return true;

                BodyLogger.AddLogSafe(ref logs,
                    $"🔇 [AudioSource.PlayOneShot] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot ({checker.Value.TypeName})");
                return false;
            }

            return true;
        }

        private static bool IsSoundFromLocalPlayer(AudioSource source, List<string> logs)
        {
            var player = FindPlayerFromAudioSource(source, logs);
            return player != null && player == GamePlayerOwner.MyPlayer;
        }

        private static Player FindPlayerFromAudioSource(AudioSource source, List<string> logs)
        {
            if (source == null)
            {
                BodyLogger.AddLogSafe(ref logs, $"[FindPlayerFromAudioSource]❌ no source");
                return null;
            }

            // 1. Search for a Player component directly in GameObject parents
            var player = source.GetComponentInParent<Player>();
            if (player != null)
            {
                BodyLogger.AddLogSafe(ref logs,
                    $"[FindPlayerFromAudioSource]✅ Player found via GetComponentInParent<Player>: {player.Profile?.Nickname}");
                return player;
            }

            // 2. Search via PlayerOwner
            var playerOwner = source.GetComponentInParent<PlayerOwner>();
            if (playerOwner != null && playerOwner.Player != null)
            {
                BodyLogger.AddLogSafe(ref logs,
                    $"[FindPlayerFromAudioSource]✅ Player found via PlayerOwner: {playerOwner.Player.Profile?.Nickname}");
                return playerOwner.Player;
            }

            // 3. Try finding spatially 
            var nearest = FindNearestPlayer(source, 3f, logs);
            if (nearest != null)
            {
                BodyLogger.AddLogSafe(ref logs,
                    $"[FindPlayerFromAudioSource]✅ Player found via spatial search (distance): {nearest.Profile?.Nickname}");
                return nearest;
            }

            logs.Add($"[FindPlayerFromAudioSource]❌ No player found related to AudioSource.");
            BodyLogger.Block(logs);
            return null;
        }

        private static Player FindNearestPlayer(Component source, float maxDistance, List<string> logs)
        {
            if (source == null)
            {
                BodyLogger.AddLogSafe(ref logs, "[FindNearestPlayer]❌ Source is null.");
                return null;
            }

            var gameWorld = GetGameWorld(logs);
            if (gameWorld == null)
            {
                BodyLogger.AddLogSafe(ref logs, "[FindNearestPlayer]❌ GameWorld not initialized.");
                return null;
            }

            var sourcePosition = source.transform.position;

            // Skip if position is abnormal (ex: Vector3.zero or invalid)
            if (float.IsNaN(sourcePosition.x) ||
                float.IsNaN(sourcePosition.y) ||
                float.IsNaN(sourcePosition.z) ||
                sourcePosition == Vector3.zero)
            {
                BodyLogger.AddLogSafe(ref logs,
                    "[FindNearestPlayer]⚠️ Source position invalid or zero. Skipping spatial search.");
                return null;
            }

            Player closest = null;
            var minDistance = float.MaxValue;

            foreach (var player in gameWorld.AllAlivePlayersList)
            {
                if (player == null)
                    continue;

                var dist = Vector3.Distance(player.Transform.position, sourcePosition);
                if (!(dist < minDistance))
                    continue;
                minDistance = dist;
                closest = player;
            }

            if (closest != null && minDistance < maxDistance)
            {
                BodyLogger.AddLogSafe(ref logs,
                    $"[TEST DEBUG PLAYER]✅ Player found near the sound ({minDistance:F2}m): {closest.Profile?.Nickname}");
                return closest;
            }

            BodyLogger.AddLogSafe(ref logs,
                $"[TEST DEBUG PLAYER]⚠️ No player near the sound (minimum distance {minDistance:F2}m > threshold {maxDistance}m)");
            return null;
        }

        public static void LogClipInfo(AudioClip clip1, AudioClip clip2, List<string> logs)
        {
            if (clip1 != null)
                BodyLogger.AddLogSafe(ref logs, $"[ReverbSimpleSource.Play] clip name : {clip1.name}");
            if (clip2 != null)
                BodyLogger.AddLogSafe(ref logs, $"[ReverbSimpleSource.Play] clip name : {clip2.name}");
        }

        public static string DetermineClipName(AudioClip clip1, AudioClip clip2)
        {
            return clip1?.name ?? clip2?.name ?? "NULL";
        }

        public static bool IsBreathOk(string clipName)
        {
            return BreathOk(clipName) && !Plugin.BreathOkVolume.Value;
        }

        public static bool IsLocalPlayerLooting(ReverbSimpleSource instance, string clipName, List<string> logs)
        {
            var player = FindNearestPlayer(instance, 3f, logs);
            return player != null && player == GamePlayerOwner.MyPlayer;
        }

        public static bool ValidateLocalPlayer(BetterSource audioSource, List<string> logs, out Player player)
        {
            var foundPlayer = GetPlayer(audioSource, logs);
            var isLocalPlayer = foundPlayer != null && foundPlayer == GamePlayerOwner.MyPlayer;

            if (!isLocalPlayer)
            {
                BodyLogger.AddLogSafe(ref logs, "❌ not from a local player, no adjustment.");
                player = null;
                return false;
            }

            BodyLogger.AddLogSafe(ref logs, $"🔍 Player detected: {foundPlayer}");
            player = foundPlayer;
            return true;
        }
        
        /// <summary>
        /// Applies volume adjustment based on the player's total weight.
        /// </summary>
        /// <param name="audioSource">The audio source to modify</param>
        /// <param name="player">The player whose weight affects the volume</param>
        /// <param name="logs">logs for debugging</param>
        /// <param name="primary">first clip </param>
        /// <param name="secondary">second clip</param>
        public static void ApplyWeightBasedVolume(
            BetterSource audioSource,
            Player player,
            List<string> logs,
            ClipVolumeInfo primary,
            ClipVolumeInfo secondary)
        {
            if (primary.SkipWeightAdjust || secondary.SkipWeightAdjust)
            {
                BodyLogger.AddLogSafe(ref logs,
                    $" Skipped WeightBasedVolume adjustment due to customized clip volume " +
                    $"(Primary: {primary.Volume:0.00}," +
                    $" Secondary: {secondary.Volume:0.00})");
                ApplyVolumeAdjustment(audioSource, logs, primary, secondary);
                return;
            }
            
            if (player?.Physical?.iobserverToPlayerBridge_0?.TotalWeight == null)
                return;

            var maxWeight = Plugin.MaxWeight.Value;
            
            var playerWeight = player.Physical.iobserverToPlayerBridge_0.TotalWeight;
            BodyLogger.AddLogSafe(ref logs, $"[WeightBasedVolume] Player weight: {playerWeight:F2} kg");
            
            var clampedWeight = Mathf.Clamp(playerWeight, MinWeight, maxWeight);
            // Normalize weight to [0,1] ranges
            var normalizedVolume = (clampedWeight - MinWeight) / (maxWeight - MinWeight);
            // Apply a quadratic curve for logarithmic volume scaling:
            // Examples:
            // 0.3 -> 0.09 
            // 0.5 -> 0.25
            // 0.7 -> 0.49
            // 0.9 -> 0.81
            normalizedVolume = Mathf.Clamp01(Mathf.Pow(normalizedVolume, 2f));

            BodyLogger.AddLogSafe(ref logs,
                $"🔈 Weight-based volume calculated (logarithmic): {normalizedVolume:0.00} (player {playerWeight:F2}kg / max {maxWeight}kg)");

            audioSource.SetBaseVolume(normalizedVolume);

            BodyLogger.AddLogSafe(ref logs, "✅ Volume applied via WeightBasedVolume.");
            BodyLogger.Block(logs);
        }

        public static void LogClipNames(string primaryClip, string secondaryClip, List<string> logs)
        {
            if (!string.IsNullOrEmpty(primaryClip))
            {
                BodyLogger.AddLogSafe(ref logs, $"[BetterSource.Play] clip name : {primaryClip}");
            }

            if (!string.IsNullOrEmpty(secondaryClip))
            {
                BodyLogger.AddLogSafe(ref logs, $"[BetterSource.Play] clip name : {secondaryClip}");
            }
        }

        public static void ApplyVolumeAdjustment(
            BetterSource audioSource,
            List<string> logs,
            ClipVolumeInfo primary,
            ClipVolumeInfo secondary)
        {
            var finalVolume = Math.Min(primary.Volume, secondary.Volume);

            BodyLogger.AddLogSafe(ref logs,
                $"🔈 Calculated volume:" +
                $" Primary = {primary.Volume:0.00} ({primary.Name})," +
                $" Secondary = {secondary.Volume:0.00} ({secondary.Name})," +
                $" Final = {finalVolume:0.00}");

            audioSource.SetBaseVolume(finalVolume);

            BodyLogger.AddLogSafe(ref logs, "✅ Volume applied via SetBaseVolume for Body/Medical sound.");
        }    
    }
}