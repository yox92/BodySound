using System;
using BodySound.Utils;
using EFT;
using HarmonyLib;
using UnityEngine;

namespace BodySound.Patches;

public class PatchBodySoundVolume
{
    [HarmonyPatch(typeof(BetterSource), nameof(BetterSource.Play),
        new[]
        {
            typeof(AudioClip), typeof(AudioClip), typeof(float),
            typeof(float), typeof(bool), typeof(bool)
        })]
    public static class PatchBetterSourcePlay
    {
        [HarmonyPrefix]
        public static void Prefix(
            BetterSource __instance,
            AudioClip clip1,
            AudioClip clip2,
            float balance,
            float volume,
            bool forceStereo,
            bool oneShot)
        {
            try
            {
                if (!IsBodyAudio(clip1?.name ?? string.Empty) && !IsBodyAudio(clip2?.name ?? string.Empty)) return;

                var player = __instance?.transform?.root?.GetComponent<Player>();
                if (player == null) return;
                var isLocal = player != null && player == GamePlayerOwner.MyPlayer;
                if (!isLocal) return;

                // 🎧 Récupération des noms
                var clipName1 = clip1?.name;
                var clipName2 = clip2?.name;

                if (clip1 != null)
                    BodyLogger.Log($"[BetterSource.Play] 🎧 Clip1 joué : {clipName1}");
                if (clip2 != null && clip2 != clip1)
                    BodyLogger.Log($"[BetterSource.Play] 🎧 Clip2 joué : {clipName2}");

                var volume1 = clipName1 != null ? GetVolumeFromConfig(clipName1) : 1f;
                var volume2 = clipName2 != null ? GetVolumeFromConfig(clipName2) : 1f;

                var finalVolume = Math.Min(volume1, volume2);
                __instance.SetBaseVolume(finalVolume);
                BodyLogger.Log(
                    $"[BetterSource.Play] 🧲 Volume forcé: {finalVolume} pour {clipName1} / {clipName2}");
            }
            catch (Exception ex)
            {
                BodyLogger.Error($"[BetterSource.Play] ❌ Exception dans Prefix : {ex}");
            }
        }
    }

    private static bool IsBodyAudio(string name) =>
        name.StartsWith("walk_") || name.StartsWith("gear_") || name.StartsWith("stop_") ||
        name.StartsWith("jump_") || name.StartsWith("landing_") || name.StartsWith("turn_") ||
        name.StartsWith("sprint_");

// 🔧 Volume selon config
    private static float GetVolumeFromConfig(string name) =>
        (name switch
        {
            _ when name.StartsWith("walk_") => Plugin.WalkVolume.Value,
            _ when name.StartsWith("turn_") => Plugin.TurnVolume.Value,
            _ when name.StartsWith("gear_") => Plugin.GearVolume.Value,
            _ when name.StartsWith("stop_") => Plugin.StopVolume.Value,
            _ when name.StartsWith("sprint_") => Plugin.SprintVolume.Value,
            _ when name.StartsWith("jump_") || name.StartsWith("landing_") => Plugin.JumpVolume.Value,
            _ => 100
        }) / 100f;
}