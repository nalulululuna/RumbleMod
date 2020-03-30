using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace RumbleMod
{
    public class SettingsController : PersistentSingleton<SettingsController>
    {
		private bool _initialized;
		protected VRPlatformHelper _vrPlatformHelper;

		public void Initialize()
		{
			_vrPlatformHelper = Resources.FindObjectsOfTypeAll<VRPlatformHelper>().FirstOrDefault();
			if (_vrPlatformHelper == null)
			{
				_vrPlatformHelper = new GameObject().AddComponent<VRPlatformHelper>();
			}

			_modEnabled = Configuration.PluginConfig.Instance.enabled;
			_strength = Configuration.PluginConfig.Instance.strength;
			_duration = Configuration.PluginConfig.Instance.duration;
			_initialized = true;
		}

		private bool _modEnabled;
		[UIValue("enabled")]
		public bool modEnabled
		{
			get => Configuration.PluginConfig.Instance.enabled;
			set => _modEnabled = value;
		}

		private float _strength;
		[UIValue("strength")]
        public float strength
        {
            get => Configuration.PluginConfig.Instance.strength;
            set => _strength = value;
        }

		private float _duration;
		[UIValue("duration")]
		public float duration
		{
			get => Configuration.PluginConfig.Instance.duration;
			set => _duration = value;
		}

		[UIAction("test")]
        private void ButtonClicked()
        {
			if (!_initialized)
			{
				Initialize();
			}

			Logger.log.Info($"RumbleTest: duration={_duration}, strength={_strength}");
            PersistentSingleton<SharedCoroutineStarter>.instance.StartCoroutine(OneShotRumbleCoroutine(XRNode.RightHand, _duration, _strength, 0));
			PersistentSingleton<SharedCoroutineStarter>.instance.StartCoroutine(OneShotRumbleCoroutine(XRNode.LeftHand, _duration, _strength, 0));
		}

		public virtual IEnumerator OneShotRumbleCoroutine(XRNode node, float duration, float impulseStrength, float intervalDuration = 0f)
		{
			long startTicks = DateTime.UtcNow.Ticks;
			float num = (float)(DateTime.UtcNow.Ticks - startTicks) / 1E+07f;
			float startTime = num;
			this._vrPlatformHelper.TriggerHapticPulse(node, impulseStrength);
			while (num - startTime < duration)
			{
				float intervalStartTime = num;
				yield return null;
				num = (float)(DateTime.UtcNow.Ticks - startTicks) / 1E+07f;
				while (num - intervalStartTime < intervalDuration)
				{
					yield return null;
					num = (float)(DateTime.UtcNow.Ticks - startTicks) / 1E+07f;
				}
				this._vrPlatformHelper.TriggerHapticPulse(node, impulseStrength);
			}
			yield break;
		}

		[UIAction("#apply")]
		public void OnApply()
		{
			Logger.log.Info($"Apply: enabled={enabled}, duration={_duration}, strength={_strength}");
			Configuration.PluginConfig.Instance.enabled = _modEnabled;
			Configuration.PluginConfig.Instance.strength = _strength;
			Configuration.PluginConfig.Instance.duration = _duration;
			if (_modEnabled)
			{
				Plugin.ApplyHarmonyPatches();
			}
			else
			{
				Plugin.RemoveHarmonyPatches();
			}
		}
	}
}
