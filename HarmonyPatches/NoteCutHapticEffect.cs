using HarmonyLib;
using Libraries.HM.HMLib.VR;
using System;
using UnityEngine.XR;

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutHapticEffect), "HitNote")]
    internal static class NoteCutHapticEffectHitNote
    {
        internal static HapticPresetSO rumblePreset = new HapticPresetSO();
        
        [HarmonyPriority(Priority.Low)]
        static bool Prefix(SaberType saberType, HapticFeedbackController ____hapticFeedbackController, HapticPresetSO ____rumblePreset)
        {
            //Logger.log.Debug($"NoteCutHapticEffect: strengh={____rumblePreset._strength}, duration={____rumblePreset._duration}, frequency={____rumblePreset._frequency}, continuous={____rumblePreset._continuous}");
            if ((Configuration.PluginConfig.Instance.strength > 0) && (Configuration.PluginConfig.Instance.duration > 0))
            {
                ____hapticFeedbackController.PlayHapticFeedback(saberType.Node(), rumblePreset);
            }
            return false;
        }
    }
}
