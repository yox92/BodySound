using BodySound.Utils;
using EFT;
using HarmonyLib;

namespace BodySound.Boot;

public abstract class StopRaid
{
    [HarmonyPatch(typeof(ClientGameWorld), nameof(ClientGameWorld.OnDestroy))]
    private class PatchRaidEnd
    {
        private static void Postfix()
        {
            BodyLogger.Info("[OnDestroy] Raid off");
            CacheStatusSession.InRaid = false;
        }
    }

}