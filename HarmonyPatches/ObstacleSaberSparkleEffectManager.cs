using HarmonyLib;
using Libraries.HM.HMLib.VR;

namespace RumbleMod.HarmonyPatches
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager), "Update")]
    internal static class ObstacleSaberSparkleEffectManagerUpdate
    {
        static void Prefix(HapticPresetSO ____rumblePreset)
        {
            ____rumblePreset._strength = Configuration.PluginConfig.Instance.strength_wall;
        }
    }
}
