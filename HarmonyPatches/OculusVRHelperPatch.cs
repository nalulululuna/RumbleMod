﻿using HarmonyLib;
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
            // unless it's necessary, use the original haptics clip
            if (strength == 1.0f)
            {
                hapticsClip = ____hapticsClip;
            }
            else if (strength == Configuration.PluginConfig.Instance.strength)
            {
                hapticsClip = SettingsController.instance.hapticsClipNote;
            }
            else if (strength == Configuration.PluginConfig.Instance.strength_saber)
            {
                hapticsClip = SettingsController.instance.hapticsClipSaber;
            }
            else if (strength == Configuration.PluginConfig.Instance.strength_wall)
            {
                hapticsClip = SettingsController.instance.hapticsClipWall;
            }
            else if (strength == Configuration.PluginConfig.Instance.strength_ui)
            {
                hapticsClip = SettingsController.instance.hapticsClipUI;
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
