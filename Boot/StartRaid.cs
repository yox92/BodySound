using BodySound.Utils;
using EFT;
using HarmonyLib;

namespace BodySound.Boot;

public abstract class StartRaid
{
    [HarmonyPatch(typeof(GameWorld), "OnGameStarted")]
    public static class PatchGameWorldOnGameStarted
    {
        private static void Postfix()
        {
            BodyLogger.Info("[OnGameStarted] Start raid !");
            CacheStatusSession.InRaid = true;
        }
    }}