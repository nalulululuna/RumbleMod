using HarmonyLib;
using System;
using UnityEngine.XR;

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPatch("HitNote")]
    [HarmonyPatch(new Type[] {
        typeof(XRNode)})]
    internal static class HapticFeedbackControllerHitNote
    {
        static bool Prefix(HapticFeedbackController __instance, XRNode node)
        {
            __instance.Rumble(node, Configuration.PluginConfig.Instance.duration, Configuration.PluginConfig.Instance.strength, 0f);
            return false;
        }
    }
}
