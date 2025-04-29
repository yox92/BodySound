using System.Collections.Generic;
using BodySound.Utils;
using HarmonyLib;
using UnityEngine;
using Audio.ReverbSubsystem;
using BodySound.Boot;

namespace BodySound.Patches
{
    [HarmonyPatch(typeof(AudioSource), nameof(AudioSource.PlayOneShot), new[] { typeof(AudioClip), typeof(float) })]
    public static class PatchAudioSourcePlayOneShot
    {
        /// <summary>
        /// Prefix for AudioSource.PlayOneShot. Blocks playback of audio clips if they belong to a bag 
        /// and the BagVolume configuration is set to false.
        /// </summary>
        /// <param name="__instance">Instance of AudioSource</param>
        /// <param name="clip">Audio clip to be played</param>
        /// <param name="volumeScale">Volume scale of the audio clip</param>
        /// <returns>True to allow the original method to run, false to block it</returns>
        [HarmonyPrefix]
        public static bool Prefix_PlayOneShot(AudioSource __instance, AudioClip clip, float volumeScale)
        {
            // 🔒
            if (!CacheStatusSession.AllowHooks)
                return true;
            // 🔒
            if (__instance == null)
                return true;
            // 🔒
            if (clip == null)
                return true;
            var logs = new List<string>();
            // 🔒
            if (clip != null)
            {
                BodyLogger.AddLogSafe(ref logs,$"[AudioSource.PlayOneShot] clip name : {clip.name}");
            }
            
            var result = AudioUtils.UiInterfaceSound(clip.name, logs) && AudioUtils.InGameSound(clip.name, __instance, logs);
            BodyLogger.Block(logs);
            return result;
        }

        [HarmonyPatch(typeof(ReverbSimpleSource), nameof(ReverbSimpleSource.Play))]
        public static class PatchReverbSimpleSourcePlay
        {
            /// <summary>
            /// Prefix for ReverbSimpleSource.Play. Blocks playback of specific looting audio clips 
            /// if they are associated with the loot system and the LootingVolume configuration is set to false.
            /// </summary>
            /// <param name="__instance">Instance of ReverbSimpleSource</param>
            /// <param name="clip1">Primary audio clip</param>
            /// <param name="clip2">Secondary audio clip</param>
            /// <returns>True to allow the original method to run, false to block it</returns>
            [HarmonyPrefix]
            public static bool Prefix(
                ReverbSimpleSource __instance,
                ref AudioClip clip1,
                ref AudioClip clip2,
                ref float balance,
                ref float volume,
                ref bool forceStereo,
                ref bool oneShot)
            {
                // 🔒
                if (!CacheStatusSession.AllowHooks)
                    return true;
                // 🔒
                if (__instance == null) 
                    return true;
                // 🔒
                if (clip1 == null && clip2 == null)
                    return true;
                var logs = new List<string>();
                AudioUtils.LogClipInfo(clip1, clip2, logs);

                var clipName = AudioUtils.DetermineClipName(clip1, clip2);
                
                if (AudioUtils.IsBreathOk(clipName))
                {
                    BodyLogger.AddLogSafe(ref logs,
                        $"[🔇 BLOCK] Playback cancelled for '{clipName}' via ReverbSimpleSource (Breath OK)");
                    BodyLogger.Block(logs);
                    return false;
                }
                if (AudioUtils.Hurt(clipName))
                {
                    BodyLogger.AddLogSafe(ref logs,
                        $"[🔇 BLOCK] Playback cancelled for '{clipName}' via ReverbSimpleSource (Hurt)");
                    BodyLogger.Block(logs);
                    return false;
                }
               
                if (!AudioUtils.IsLootingAudio(clipName))
                {
                    BodyLogger.Block(logs);
                    return true;
                }
               
                if (!Plugin.LootingVolume.Value && AudioUtils.IsLocalPlayerLooting(__instance, clipName, logs))
                {
                    BodyLogger.AddLogSafe(ref logs,
                        $"[🔇 BLOCK] Playback cancelled for '{clipName}' via ReverbSimpleSource (Looting)");
                        BodyLogger.Block(logs);
                    return false;
                }
                BodyLogger.Block(logs);
                return true;
            }
        }
    }
}