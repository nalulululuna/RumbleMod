using BeatSaberMarkupLanguage.Attributes;
using IPA.Utilities;
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
        private VRPlatformHelper _vrPlatformHelper;

        public OVRHapticsClip hapticsClipHitNote { get; private set; }
        public OVRHapticsClip hapticsClipContinuous { get; private set; }

        private void Initialize()
        {
            _vrPlatformHelper = Resources.FindObjectsOfTypeAll<VRPlatformHelper>().FirstOrDefault();
            if (_vrPlatformHelper == null)
            {
                _vrPlatformHelper = new GameObject().AddComponent<VRPlatformHelper>();
            }

            _modEnabled = Configuration.PluginConfig.Instance.enabled;
            _strength = Configuration.PluginConfig.Instance.strength;
            _duration = Configuration.PluginConfig.Instance.duration;
            _strength_continuous = Configuration.PluginConfig.Instance.strength_continuous;
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

        private float _strength_continuous;
        [UIValue("strength_continuous")]
        public float strength_continuous
        {
            get => Configuration.PluginConfig.Instance.strength_continuous;
            set => _strength_continuous = value;
        }

        private OVRHapticsClip CreateHapticsClip(float strength)
        {
            int count = Mathf.RoundToInt(strength * 4.0f);
            if (count > 2)
            {
                OVRHapticsClip hapticsClip = new OVRHapticsClip(2 + count);
                hapticsClip.WriteSample(0);
                hapticsClip.WriteSample(0);
                for (int i = 0; i < count; i++)
                {
                    byte sample = (i == 0) ? (byte)128 : byte.MaxValue;
                    hapticsClip.WriteSample(sample);
                }
                return hapticsClip;
            }
            else
            {
                OVRHapticsClip hapticsClip = new OVRHapticsClip(4);
                hapticsClip.WriteSample(0);
                hapticsClip.WriteSample((byte)(16 * count));
                hapticsClip.WriteSample((byte)(32 * count));
                hapticsClip.WriteSample((byte)(64 * count));
                return hapticsClip;
            }
        }

        private void CreateHapticsClipIfOculus(float strength)
        {
            OculusVRHelper _oculusVRHelper = _vrPlatformHelper.GetField<OculusVRHelper, VRPlatformHelper>("_oculusVRHelper");
            if (_oculusVRHelper != null)
            {
                _oculusVRHelper.SetField<OculusVRHelper, OVRHapticsClip>("_hapticsClip", CreateHapticsClip(strength));
            }
        }

        [UIAction("test_hitnote")]
        private void ButtonHitNoteClicked()
        {
            if (!_initialized)
            {
                Initialize();
            }

            Logger.log.Info($"RumbleTest: strength={_strength}, duration={_duration}");
            CreateHapticsClipIfOculus(_strength);
            StartCoroutine(OneShotRumbleCoroutine(XRNode.RightHand, _duration, _strength));
            StartCoroutine(OneShotRumbleCoroutine(XRNode.LeftHand, _duration, _strength));
        }

        [UIAction("test_saberwall")]
        private void ButtonSaberWallClicked()
        {
            if (!_initialized)
            {
                Initialize();
            }

            Logger.log.Info($"RumbleTest: strength={_strength_continuous}, duration={1}");
            CreateHapticsClipIfOculus(_strength_continuous);
            StartCoroutine(OneShotRumbleCoroutine(XRNode.RightHand, 1, _strength_continuous));
            StartCoroutine(OneShotRumbleCoroutine(XRNode.LeftHand, 1, _strength_continuous));
        }

        private IEnumerator OneShotRumbleCoroutine(XRNode node, float duration, float impulseStrength, float intervalDuration = 0f)
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
            Logger.log.Info($"Apply: enabled={enabled}, strength={_strength}, duration={_duration}, _strength_continuous={_strength_continuous}");
            Configuration.PluginConfig.Instance.enabled = _modEnabled;
            Configuration.PluginConfig.Instance.strength = _strength;
            Configuration.PluginConfig.Instance.duration = _duration;
            Configuration.PluginConfig.Instance.strength_continuous = _strength_continuous;

            if (_modEnabled)
            {
                Plugin.ApplyHarmonyPatches();
            }
            else
            {
                Plugin.RemoveHarmonyPatches();
            }
        }

        // make sure call this before patching OculusVRHelper.TriggerHapticPulse
        public void UpdateHapticsClips()
        {
            hapticsClipHitNote = CreateHapticsClip(Configuration.PluginConfig.Instance.strength);
            hapticsClipContinuous = CreateHapticsClip(Configuration.PluginConfig.Instance.strength_continuous);
        }
    }
}
