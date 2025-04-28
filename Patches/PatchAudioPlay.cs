using System.Collections.Generic;
using BodySound.Utils;
using HarmonyLib;
using UnityEngine;
using Audio.ReverbSubsystem;
using EFT;

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
            if (clip == null)
                return true;
            
            return AudioUtils.UiInterfaceSound(clip.name) && AudioUtils.InGameSound(clip.name, __instance);
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
                if (clip1 != null)
                {
                    BodyLogger.Info($"[ReverbSimpleSource.Play] clip name : {clip1.name}");
                }

                if (clip2 != null)
                {
                    BodyLogger.Info($"[ReverbSimpleSource.Play] clip name : {clip2.name}");
                }

                var clipName = clip1?.name ?? clip2?.name ?? "NULL";

                if (!AudioUtils.IsLootingAudio(clipName)) return true;

                var player = AudioUtils.FindNearestPlayer(__instance);

                if (player == null || player != GamePlayerOwner.MyPlayer) 
                    return true;

                if (Plugin.LootingVolume.Value) 
                    return true;
                BodyLogger.Log($"[🔇 BLOCK] Playback cancelled for '{clipName}' via ReverbSimpleSource (Looting)");
                return false;
            }
        }
    }
}