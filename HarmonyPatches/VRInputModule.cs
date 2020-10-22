using HarmonyLib;
using Libraries.HM.HMLib.VR;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRUIControls;

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(VRInputModule), "HandlePointerExitAndEnter")]
    static class VRInputModuleHandlePointerExitAndEnter
    {
        static void Prefix(HapticPresetSO ____rumblePreset)
        {
            //Logger.log.Debug($"VRInputModule: strengh={____rumblePreset._strength}, duration={____rumblePreset._duration}, frequency={____rumblePreset._frequency}, continuous={____rumblePreset._continuous}");
            ____rumblePreset._strength = Configuration.PluginConfig.Instance.strength_ui;
        }
    }
}
