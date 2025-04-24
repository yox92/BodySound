using System;
using System.Collections.Generic;
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
            try
            {
                AdjustVolumeForBodySounds(__instance, clip1, clip2);
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
    private static void AdjustVolumeForBodySounds(
        BetterSource audioSource,
        AudioClip primaryClip,
        AudioClip secondaryClip)
    {
        var logs = new List<string>();

        var primaryClipName = primaryClip?.name ?? "";
        var secondaryClipName = secondaryClip?.name ?? "";

        logs.Add($"🎵 Primary: {primaryClipName}, Secondary: {secondaryClipName}");

        var player = AudioUtils.GetPlayer(audioSource, logs);
        var isLocalPlayer = player != null && player == GamePlayerOwner.MyPlayer;

        if (!isLocalPlayer)
        {
            logs.Add("❌ not from a local player, no adjustment.");
            BodyLogger.Block(logs);
            return;
        }

        logs.Add($"🔍 Player detected: {player}");

        if (!AudioUtils.IsBodyAudio(primaryClipName) && !AudioUtils.IsBodyAudio(secondaryClipName))
        {
            logs.Add("❌ Not a recognized sound, no adjustment.");
            BodyLogger.Block(logs);
            return;
        }

        var vol1 = AudioUtils.GetVolumeFromName(primaryClipName);
        var vol2 = AudioUtils.GetVolumeFromName(secondaryClipName);
        var finalVolume = Math.Min(vol1, vol2);

        logs.Add($"🔈 Calculated volume: Primary = {vol1}, Secondary = {vol2}, Final = {finalVolume}");

        audioSource.SetBaseVolume(finalVolume);
        logs.Add($"✅ Volume applied via SetBaseVolume for Body sound.");
        BodyLogger.Block(logs);
    }
}