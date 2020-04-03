using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRUIControls;

namespace RumbleMod.HarmonyPatches
{
	[HarmonyPatch(typeof(VRInputModule))]
    [HarmonyPatch("HandlePointerExitAndEnter")]
    [HarmonyPatch(new Type[] {
        typeof(PointerEventData),
        typeof(GameObject)})]
    internal static class VRInputModuleHandlePointerExitAndEnter
    {
		/* Beat Saber 1.8.0
		public new virtual void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
		{
			if (newEnterTarget == null || currentPointerData.pointerEnter == null)
			{
				for (int i = 0; i < currentPointerData.hovered.Count; i++)
				{
					ExecuteEvents.Execute<IPointerExitHandler>(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
				}
				currentPointerData.hovered.Clear();
				if (newEnterTarget == null)
				{
					currentPointerData.pointerEnter = newEnterTarget;
					return;
				}
			}
			if (currentPointerData.pointerEnter == newEnterTarget && newEnterTarget)
			{
				return;
			}
			GameObject gameObject = UnityEngine.EventSystems.BaseInputModule.FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
			if (currentPointerData.pointerEnter != null)
			{
				Transform transform = currentPointerData.pointerEnter.transform;
				while (transform != null && (!(gameObject != null) || !(gameObject.transform == transform)))
				{
					ExecuteEvents.Execute<IPointerExitHandler>(transform.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
					currentPointerData.hovered.Remove(transform.gameObject);
					transform = transform.parent;
				}
			}
			if (!base.userInteractionEnabled)
			{
				return;
			}
			currentPointerData.pointerEnter = newEnterTarget;
			if (newEnterTarget != null)
			{
				Transform transform2 = newEnterTarget.transform;
				bool flag = false;
				while (transform2 != null && transform2.gameObject != gameObject)
				{
					this._componentList.Clear();
					transform2.gameObject.GetComponents<Component>(this._componentList);
					for (int j = 0; j < this._componentList.Count; j++)
					{
						Selectable selectable = this._componentList[j] as Selectable;
						Interactable interactable = this._componentList[j] as Interactable;
						if ((selectable != null && selectable.isActiveAndEnabled && selectable.interactable) || (interactable != null && interactable.isActiveAndEnabled && interactable.interactable && !flag))
						{
							flag = true;
							this._vrPlatformHelper.TriggerHapticPulse(this._vrPointer.vrController.node, 0.25f);
							break;
						}
					}
					ExecuteEvents.Execute<IPointerEnterHandler>(transform2.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
					currentPointerData.hovered.Add(transform2.gameObject);
					transform2 = transform2.parent;
				}
			}
		}
		*/
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
