using System;
using System.Collections.Generic;
using BodySound.Boot;
using BodySound.Utils;
using EFT;
using HarmonyLib;
using UnityEngine;

namespace BodySound.Patches;

public abstract class PatchBodySoundVolume
{
    public static GameWorld CachedGameWorld;

    /// <summary>
    /// Harmony patch class for the BetterSource.Play method.
    /// This modifies the behavior to adjust body sound volumes.
    /// </summary>
    [HarmonyPatch(typeof(BetterSource), nameof(BetterSource.Play),
        new[]
        {
            typeof(AudioClip), typeof(AudioClip), typeof(float),
            typeof(float), typeof(bool), typeof(bool)
        })]
    public static class PatchBetterSourcePlay
    {
        /// <summary>
        /// Harmony prefix method that intercepts calls to BetterSource.Play.
        /// Adjusts the volume of body sounds before execution of the original method.
        /// </summary>
        /// <param name="__instance">The instance of BetterSource being called.</param>
        /// <param name="clip1">The primary audio clip.</param>
        /// <param name="clip2">The secondary audio clip.</param>
        /// <param name="balance">The balance of audio playback.</param>
        /// <param name="volume">The volume of playback (modifiable).</param>
        /// <param name="forceStereo">Flag to enforce stereo playback.</param>
        /// <param name="oneShot">Flag to indicate whether playback is one-shot.</param>
        [HarmonyPrefix]
        public static void Prefix(
            BetterSource __instance,
            AudioClip clip1,
            AudioClip clip2,
            float balance,
            ref float volume,
            bool forceStereo,
            bool oneShot)
        {
            if (!CacheStatusSession.AllowHooks)
                return;
               
            try
            {
                AdjustVolume(__instance, clip1, clip2);
            }
            catch (Exception ex)
            {
                BodyLogger.Error($"[BetterSource.Play] ❌ Exception in Prefix: {ex}");
            }
        }
    }

    /// <summary>
    /// Adjusts the volume of body sounds if detected.
    /// Ensures specific criteria are met before applying volume adjustments.
    /// </summary>
    /// <param name="audioSource">The BetterSource instance used for playing audio.</param>
    /// <param name="primaryClip">The primary audio clip being played.</param>
    /// <param name="secondaryClip">The secondary audio clip being played.</param>
    private static void AdjustVolume(
        BetterSource audioSource,
        AudioClip primaryClip,
        AudioClip secondaryClip)
    {
        // 🔒
        if (audioSource == null)
            return;
        // 🔒
        if (primaryClip == null && secondaryClip == null)
            return;
        
        var primaryClipName = primaryClip?.name;
        var secondaryClipName = secondaryClip?.name;
        
        var logs = new List<string>();
        
        // 🔒
        if (string.IsNullOrEmpty(primaryClipName) && string.IsNullOrEmpty(secondaryClipName))
        {
            BodyLogger.AddLogSafe(ref logs, "[BetterSource.Play]❌ Skipped AdjustVolume because both clips are null or empty.");
            BodyLogger.Block(logs);
            return;
        }
        
        if (!AudioUtils.ValidateLocalPlayer(audioSource, logs, out var player))
        {
            BodyLogger.Block(logs);
            return;
        }
        
        AudioUtils.LogClipNames(primaryClipName, secondaryClipName, logs);
        
        var isRecognizedBody = AudioUtils.IsBodyAudio(primaryClipName, secondaryClipName, logs);
        var isRecognizedMedic = AudioUtils.IsMedicalAudio(primaryClipName, secondaryClipName, logs);
        
        // 🔒
        if (!isRecognizedBody && !isRecognizedMedic)
        {
            BodyLogger.AddLogSafe(ref logs, "[BetterSource.Play]❌ No volume adjustment required (settings)");
            BodyLogger.Block(logs);
            return;
        }
        
        var primaryVolumeInfo = AudioUtils.GetClipVolumeInfo(primaryClipName);
        var secondaryVolumeInfo = AudioUtils.GetClipVolumeInfo(secondaryClipName);

        if (!isRecognizedMedic)
        {
            AudioUtils.ApplyWeightBasedVolume(
                audioSource,
                player,
                logs,
                primaryVolumeInfo,
                secondaryVolumeInfo);
            BodyLogger.Block(logs);
            return;
        }
        
        AudioUtils.ApplyVolumeAdjustment(
                audioSource,
                logs,
                primaryVolumeInfo,
                secondaryVolumeInfo);
        BodyLogger.Block(logs);
    }
}