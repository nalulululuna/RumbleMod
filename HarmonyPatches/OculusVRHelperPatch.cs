using HarmonyLib;
using System;
using UnityEngine.XR;

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(OculusVRHelper))]
    [HarmonyPatch("TriggerHapticPulse")]
    [HarmonyPatch(new Type[] {
        typeof(XRNode),
        typeof(float)})]
    internal static class OculusVRHelperTriggerHapticPulse
    {
        static bool Prefix(XRNode node, float strength, OVRHapticsClip ____hapticsClip)
        {
            OVRHapticsClip hapticsClip;
            if (strength == Configuration.PluginConfig.Instance.strength)
            {
                hapticsClip = SettingsController.instance.hapticsClipHitNote;
            }
            else if (strength == Configuration.PluginConfig.Instance.strength_continuous)
            {
                hapticsClip = SettingsController.instance.hapticsClipContinuous;
            }
            else
            {
                hapticsClip = ____hapticsClip;
            }
            ((node == XRNode.LeftHand) ? OVRHaptics.LeftChannel : OVRHaptics.RightChannel).Mix(hapticsClip);
            return false;
        }
    }
}
