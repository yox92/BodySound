using System;
using System.Collections.Generic;
using BodySound.Patches;
using UnityEngine;
using EFT;

namespace BodySound.Utils
{
    public static class AudioUtils
    {
        
        private static bool IsBodyAudio(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Contains("walk") ||
                   name.Contains("gear") ||
                   name.Contains("stop") ||
                   name.Contains("jump") ||
                   name.Contains("landing") ||
                   name.Contains("turn") ||
                   name.Contains("vault") ||
                   name.Contains("sprint");
        }

        private static bool IsMedkit(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_medkit_");
        }

        private static bool IsBandage(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_bandage_");
        }

        private static bool IsSurgicalKit(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_surgicalkit_");
        }

        private static bool IsRollUpKit(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_rollupkit_");
        }

        private static bool IsInjector(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_injector_");
        }

        private static bool IsZvezda(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_zvezda_");
        }

        private static bool IsSplint(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_splint_");
        }

        private static bool IsPillsBottle(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_pillsbottle_");
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
        
        private static bool ModPickup(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("mod_pickup");
        }
        
        private static bool Switcher(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Contains("weapon") && name.Contains("switcher");
        }

        private static bool Use(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.EndsWith("_use");
        }

        private static bool Pickup(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.EndsWith("_pickup");
        }

        private static bool Drop(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.EndsWith("_drop");
        }

        private static bool Over(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.EndsWith("_over");
        }

        private static bool Install(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_install_");
        }

        private static bool Inspector(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.Contains("_inspector_");
        }


        private static bool WeaponInOut(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Equals("weap_out", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("weap_in", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieves the volume for a given audio name from the plugin configuration.
        /// </summary>
        /// <param name="name">The name of the audio clip.</param>
        /// <returns>Configured volume for the audio, as a float between 0 and 1.</returns>
        private static float GetVolumeFromConfig(string name) =>
            (name switch
            {
                _ when name.Contains("walk") => Plugin.WalkVolume.Value,
                _ when name.Contains("turn") => Plugin.TurnVolume.Value,
                _ when name.Contains("gear") => Plugin.GearVolume.Value,
                _ when name.Contains("stop") => Plugin.StopVolume.Value,
                _ when name.Contains("sprint") => Plugin.SprintVolume.Value,
                _ when name.Contains("vault") => Plugin.VaultVolume.Value,
                _ when name.Contains("_medkit_") => Plugin.MedKitVolume.Value,
                _ when name.Contains("_bandage_") => Plugin.BandageKitVolume.Value,
                _ when name.Contains("_surgicalkit_") => Plugin.SurgicalKitVolume.Value,
                _ when name.Contains("_rollupkit_") => Plugin.RollupKitVolume.Value,
                _ when name.Contains("_injector_") => Plugin.InjectorKitVolume.Value,
                _ when name.Contains("_zvezda_") => Plugin.BalmKitVolume.Value,
                _ when name.Contains("_splint_") => Plugin.SplintKitVolume.Value,
                _ when name.Contains("_pillsbottle_") => Plugin.PillsbottleKitVolume.Value,
                _ when name.Contains("jump") || name.Contains("landing")
                    => Plugin.JumpVolume.Value,
                _ => 100
            }) / 100f;

        /// <summary>
        /// Retrieves the volume for the specified audio clip name, falling back to default if not found.
        /// </summary>
        /// <param name="clipName">The name of the audio clip.</param>
        /// <returns>Volume for the audio clip, as a float between 0 and 1.</returns>
        public static float GetVolumeFromName(string clipName)
        {
            return clipName != null ? GetVolumeFromConfig(clipName) : 1f;
        }
        
        /// <summary>
        /// Retrieves the current GameWorld instance dynamically or from cache.
        /// </summary>
        /// <returns>The GameWorld instance or null if not found.</returns>
        private static GameWorld GetGameWorld()
        {
            if (PatchBodySoundVolume.CachedGameWorld != null)
                return PatchBodySoundVolume.CachedGameWorld;

            PatchBodySoundVolume.CachedGameWorld = UnityEngine.Object.FindObjectOfType<GameWorld>();
            BodyLogger.Log(PatchBodySoundVolume.CachedGameWorld != null
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
        public static Player GetPlayer(
            BetterSource audioSource,
            List<string> logs)
        {
            try
            {
                var directPlayer = GetDirectPlayer(audioSource);
                if (directPlayer != null)
                {
                    logs.Add($"[GetPlayer] 🚀 Direct player retrieved: {directPlayer.Profile?.Nickname}");
                    return directPlayer;
                }

                BodyLogger.Warn($"[GetPlayer] ❌ No player detected.");
                return null;
            }
            catch (Exception ex)
            {
                BodyLogger.Error($"[GetPlayer] 🚨 Exception while detecting player: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the player directly associated with the specified audio source by searching through GameObject hierarchy and nearby players.
        /// </summary>
        /// <param name="audioSource">The audio source whose hierarchy and surroundings are analyzed.</param>
        /// <returns>The associated player component, or null if not found.</returns>
        private static Player GetDirectPlayer(BetterSource audioSource)
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
            var nearest = FindNearestPlayer(audioSource, 2.5f); // Small radius to avoid errors
            return nearest;
        }

        public static bool IsRecognizedAudioType(string primaryClipName, string secondaryClipName)
        {
            return IsBodyAudio(primaryClipName) || IsBodyAudio(secondaryClipName) ||
                   IsMedkit(primaryClipName) || IsMedkit(secondaryClipName) ||
                   IsBandage(primaryClipName) || IsBandage(secondaryClipName) ||
                   IsSurgicalKit(primaryClipName) || IsSurgicalKit(secondaryClipName) ||
                   IsRollUpKit(primaryClipName) || IsRollUpKit(secondaryClipName) ||
                   IsInjector(primaryClipName) || IsInjector(secondaryClipName) ||
                   IsZvezda(primaryClipName) || IsZvezda(secondaryClipName) ||
                   IsSplint(primaryClipName) || IsSplint(secondaryClipName) ||
                   IsPillsBottle(primaryClipName) || IsPillsBottle(secondaryClipName);
        }
                public static bool UiInterfaceSound(string clipName)
        {
            // Check for Bag sounds
            if (IsBag(clipName) && !Plugin.BagVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Bag)");
                return false;
            }

            // Check for Notification sounds
            if (Notification(clipName) && !Plugin.NotificationVolume.Value)
            {
                BodyLogger.Log(
                    $"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Notification)");
                return false;
            }

            // Check for Equip sounds
            if (Use(clipName) && !Plugin.EquipVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Equip)");
                return false;
            }

            // Check for PickUp sounds
            if (Pickup(clipName) && !Plugin.PickUpVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (PickUp)");
                return false;
            }

            // Check for Drop sounds
            if (Drop(clipName) && !Plugin.DropVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Drop)");
                return false;
            }

            // Check for Over sounds
            if (Over(clipName) && !Plugin.OverVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Over)");
                return false;
            }

            // Check for Install sounds
            if (Install(clipName) && !Plugin.InstallVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Install)");
                return false;
            }

            // Check for Inspector sounds
            if (Inspector(clipName) && !Plugin.InspectorVolume.Value)
            {
                BodyLogger.Log(
                    $"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Inspector)");
                return false;
            }

            return true;
        }
                
        public static bool InGameSound(string clipName, AudioSource source)
        {
            var isHandGrip = IsHandGrip(clipName);
            var isSwitcher = Switcher(clipName);
            var isWeaponInOut = WeaponInOut(clipName);
            
            if (!isHandGrip && !isSwitcher && !isWeaponInOut)
            {
                return true;
            }

            // 🎯 2. Then, check if the sound comes from the local player
            if (!IsSoundFromLocalPlayer(source))
            {
                return true;
            }

            // 🎯 3. Block according to types
            if (isHandGrip && !Plugin.HandGripVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (HandGrip)");
                return false;
            }

            if (isSwitcher && !Plugin.SwitcherVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (Switcher)");
                return false;
            }

            if (isWeaponInOut && !Plugin.WeaponInOutVolume.Value)
            {
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via AudioSource.PlayOneShot (WeaponInOut)");
                return false;
            }

            return true;
        }        
        private static bool IsSoundFromLocalPlayer(AudioSource source)
        {
            var player = FindPlayerFromAudioSource(source);
            return player != null && player == GamePlayerOwner.MyPlayer;
        }
        
        private static Player FindPlayerFromAudioSource(AudioSource source)
        {
            var logs = new List<string>();
            if (source == null)
            {
                logs.Add($"[FindPlayerFromAudioSource]❌ no source");
                return null;
            }

            // 1. Search for a Player component directly in GameObject parents
            var player = source.GetComponentInParent<Player>();
            if (player != null)
            {
                logs.Add($"[FindPlayerFromAudioSource]✅ Player found via GetComponentInParent<Player>: {player.Profile?.Nickname}");
                BodyLogger.Block(logs);
                return player;
            }

            // 2. Search via PlayerOwner
            var playerOwner = source.GetComponentInParent<PlayerOwner>();
            if (playerOwner != null && playerOwner.Player != null)
            {
                logs.Add($"[FindPlayerFromAudioSource]✅ Player found via PlayerOwner: {playerOwner.Player.Profile?.Nickname}");
                BodyLogger.Block(logs);
                return playerOwner.Player;
            }

            // 3. Try finding spatially 
            var nearest = FindNearestPlayer(source);
            if (nearest != null)
            {
                logs.Add($"[FindPlayerFromAudioSource]✅ Player found via spatial search (distance): {nearest.Profile?.Nickname}");
                BodyLogger.Block(logs);
                return nearest;
            }

            logs.Add($"[FindPlayerFromAudioSource]❌ No player found related to AudioSource.");
            BodyLogger.Block(logs);
            return null;
        }
        
        public static Player FindNearestPlayer(Component source, float maxDistance = 3f)
        {
            if (source == null)
            {
                BodyLogger.Log("[FindNearestPlayer]❌ Source is null.");
                return null;
            }

            var gameWorld = GetGameWorld();
            if (gameWorld == null)
            {
                BodyLogger.Log("[FindNearestPlayer]❌ GameWorld not initialized.");
                return null;
            }

            var sourcePosition = source.transform.position;

            // Skip if position is abnormal (ex: Vector3.zero or invalid)
            if (float.IsNaN(sourcePosition.x) || float.IsNaN(sourcePosition.y) || float.IsNaN(sourcePosition.z) ||
                sourcePosition == Vector3.zero)
            {
                BodyLogger.Log("[FindNearestPlayer]⚠️ Source position invalid or zero. Skipping spatial search.");
                return null;
            }

            Player closest = null;
            var minDistance = float.MaxValue;

            foreach (var player in gameWorld.AllAlivePlayersList)
            {
                if (player == null) continue;

                var dist = Vector3.Distance(player.Transform.position, sourcePosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = player;
                }
            }

            if (closest != null && minDistance < maxDistance)
            {
                BodyLogger.Log($"[TEST DEBUG PLAYER]✅ Player found near the sound ({minDistance:F2}m): {closest.Profile?.Nickname}");
                return closest;
            }

            BodyLogger.Log($"[TEST DEBUG PLAYER]⚠️ No player near the sound (minimum distance {minDistance:F2}m > threshold {maxDistance}m)");
            return null;
        }

    }
}