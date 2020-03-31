using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
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
            if (Configuration.PluginConfig.Instance.duration > 0)
            {
                __instance.Rumble(node, Configuration.PluginConfig.Instance.duration, Configuration.PluginConfig.Instance.strength, 0f);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPatch("LateUpdate")]
    internal static class HapticFeedbackControllerLateUpdate
    {
        private class ContinousRumbleParams
        {
            public bool active;
            public float intervalTimeCounter;
        }

        static bool Prefix(HapticFeedbackController __instance, Dictionary<XRNode, ContinousRumbleParams> ____continuousRumbles, VRPlatformHelper ____vrPlatformHelper)
        {
            foreach (KeyValuePair<XRNode, ContinousRumbleParams> keyValuePair in ____continuousRumbles)
            {
                XRNode key = keyValuePair.Key;
                ContinousRumbleParams value = keyValuePair.Value;
                if (value.intervalTimeCounter > 0f)
                {
                    value.intervalTimeCounter -= Time.deltaTime;
                }
                else if (value.active)
                {
                    value.intervalTimeCounter = 0.01f;
                    if (Configuration.PluginConfig.Instance.strength_continuous > 0)
                    {
                        ____vrPlatformHelper.TriggerHapticPulse(key, Configuration.PluginConfig.Instance.strength_continuous);
                    }
                    value.active = false;
                }
            }
            return false;
        }
    }
}
