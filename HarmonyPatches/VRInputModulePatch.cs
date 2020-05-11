using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRUIControls;

namespace RumbleMod.HarmonyPatches
{
    // void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
    [HarmonyPatch(typeof(VRInputModule))]
    [HarmonyPatch("HandlePointerExitAndEnter")]
    [HarmonyPatch(new Type[] {
        typeof(PointerEventData),
        typeof(GameObject)})]
    internal static class VRInputModuleHandlePointerExitAndEnter
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction code in instructions)
            {
                if ((code.operand as float?) == 0.25f)
                {
                    code.operand = RumbleMod.Configuration.PluginConfig.Instance.strength_ui;
                    return instructions;
                }
            }

            // TriggerHapticPulse param not found
            Logger.log?.Critical("Error applying a Harmony patch to VRInputModule.");
            return instructions;
        }
    }
}
