using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RumbleMod.HarmonyPatches
{
    // void HitNote(XRNode node)
    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPatch("HitNote")]
    [HarmonyPatch(new Type[] {
        typeof(XRNode)})]
    internal static class HapticFeedbackControllerHitNote
    {
        static bool Prefix(HapticFeedbackController __instance, XRNode node)
        {
            if ((Configuration.PluginConfig.Instance.strength > 0) && (Configuration.PluginConfig.Instance.duration > 0))
            {
                __instance.Rumble(node, Configuration.PluginConfig.Instance.duration, Configuration.PluginConfig.Instance.strength, 0f);
            }
            return false;
        }
    }

    // void LateUpdate()
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
            foreach (KeyValuePair<XRNode, ContinousRumbleParams> nodeAndRumbleParam in ____continuousRumbles)
            {
                XRNode node = nodeAndRumbleParam.Key;
                ContinousRumbleParams rumbleParam = nodeAndRumbleParam.Value;
                if (rumbleParam.intervalTimeCounter <= 0f)
                {
                    if (rumbleParam.active)
                    {
                        rumbleParam.intervalTimeCounter = 0.01f;
                        if (GameState.sabersAreClashing && (Configuration.PluginConfig.Instance.strength_saber > 0))
                        {
                            ____vrPlatformHelper.TriggerHapticPulse(node, Configuration.PluginConfig.Instance.strength_saber);
                        }
                        else if (!GameState.sabersAreClashing && (Configuration.PluginConfig.Instance.strength_wall > 0))
                        {
                            ____vrPlatformHelper.TriggerHapticPulse(node, Configuration.PluginConfig.Instance.strength_wall);
                        }
                        rumbleParam.active = false;
                    }
                }
                else
                {
                    rumbleParam.intervalTimeCounter -= Time.deltaTime;
                }
            }
            return false;
        }
    }
}
