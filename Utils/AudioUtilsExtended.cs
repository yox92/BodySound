using System;
using System.Collections.Generic;
using BodySound.Patches;
using UnityEngine;
using EFT;

namespace BodySound.Utils
{
    public static class AudioUtils
    {
        /// <summary>
        /// Checks if the audio name corresponds to a body sound.
        /// </summary>
        /// <param name="name">The name of the audio clip.</param>
        /// <returns>True if it is a body audio, otherwise false.</returns>
        public static bool IsBodyAudio(string name)
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

        /// <summary>
        /// Checks if the audio name corresponds to a looting sound.
        /// </summary>
        /// <param name="name">The name of the audio clip.</param>
        /// <returns>True if it is a looting audio, otherwise false.</returns>
        public static bool IsLootingAudio(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Contains("looting");
        }

        /// <summary>
        /// Checks if the audio name corresponds to a bag sound.
        /// </summary>
        /// <param name="name">The name of the audio clip.</param>
        /// <returns>True if it is a bag audio, otherwise false.</returns>
        public static bool IsBag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return name.Equals("backpack_close", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("backpack_open", StringComparison.OrdinalIgnoreCase);
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
        /// Finds the closest player to the source audio, within a specified distance.
        /// </summary>
        /// <param name="source">The source of the audio.</param>
        /// <param name="maxDistance">The maximum allowable distance to a player.</param>
        /// <returns>The nearest player or null if no player is within the maximum distance.</returns>
        public static Player FindNearestPlayerToSource(BetterSource source, float maxDistance = 3f)
        {
            var gameWorld = GetGameWorld();
            if (gameWorld == null)
            {
                BodyLogger.Log("❌ GameWorld not initialized.");
                return null;
            }

            Player closest = null;
            var minDistance = float.MaxValue;
            var sourcePosition = source.transform.position;

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
                BodyLogger.Log($"✅ Player found near the sound ({minDistance:F2}m): {closest.Profile?.Nickname}");
                return closest;
            }

            BodyLogger.Log(
                $"⚠️ No player near the sound (minimum distance {minDistance:F2}m > threshold {maxDistance}m)");
            return null;
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
        /// Attempts to retrieve the player associated with the given audio source.
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
        /// Retrieves the player directly associated with the specified audio source.
        /// </summary>
        /// <param name="audioSource">The audio source whose root transform is analyzed.</param>
        /// <returns>The player component, or null if not found.</returns>
        private static Player GetDirectPlayer(BetterSource audioSource)
        {
            if (audioSource == null || audioSource.transform == null || audioSource.transform.root == null)
                return null;

            return audioSource.transform.root.GetComponent<Player>();
        }
    }
}