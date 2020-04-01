using HarmonyLib;

namespace RumbleMod
{
    public static class GameState
    {
        public static bool sabersAreClashing;
    }
}

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberClashEffect))]
    [HarmonyPatch("LateUpdate")]
    internal static class SaberClashEffectLateUpdate
    {
        static void Prefix(SaberClashChecker ____saberClashChecker)
        {
            GameState.sabersAreClashing = ____saberClashChecker.sabersAreClashing;
        }
    }
}
