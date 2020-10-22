using HarmonyLib;
using Libraries.HM.HMLib.VR;

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberClashEffect), "LateUpdate")]
    static class SaberClashEffectLateUpdate
    {
        static void Prefix(HapticPresetSO ____rumblePreset)
        {
            ____rumblePreset._strength = Configuration.PluginConfig.Instance.strength_saber;
        }
    }
}
