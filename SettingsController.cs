using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
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
        private bool _isIndex;
        private bool _isOculusPlatform;
        private VRPlatformHelper _vrPlatformHelper;

        public OVRHapticsClip hapticsClipNote { get; private set; }
        public OVRHapticsClip hapticsClipSaber { get; private set; }
        public OVRHapticsClip hapticsClipWall { get; private set; }
        public OVRHapticsClip hapticsClipUI { get; private set; }

        [UIParams]
        BSMLParserParams parserParams;

        [UIAction("current_click")]
        private void OnCurrentClick()
        {
            ReadCurrentSettings();
            parserParams.EmitEvent("cancel");
        }

        [UIAction("default_click")]
        private void OnDefaultClick()
        {
            _modEnabled = false;
            _strength = 1.0f;
            _duration = 0.13f;
            _strength_saber = 1.0f;
            _strength_wall = 1.0f;
            _strength_ui = _isOculusPlatform ? 1.0f : 0.25f;
            parserParams.EmitEvent("cancel");
        }

        [UIAction("recommended_click")]
        private void OnRecommendedClick()
        {
            _modEnabled = true;
            _strength = _isOculusPlatform ? 3.0f : _isIndex ? 0.55f : 1.0f;
            _duration = _isOculusPlatform ? 0.13f : 0.16f;
            _strength_saber = 1.0f;
            _strength_wall = 1.0f;
            _strength_ui = _isOculusPlatform ? 1.0f : 0.25f;
            parserParams.EmitEvent("cancel");
        }

        private bool _modEnabled;
        [UIValue("enabled")]
        public bool modEnabled
        {
            get => _modEnabled;
            set => _modEnabled = value;
        }

        private float _strength;
        [UIValue("strength")]
        public float strength
        {
            get => _strength;
            set => _strength = value;
        }

        private float _duration;
        [UIValue("duration")]
        public float duration
        {
            get => _duration;
            set => _duration = value;
        }

        private float _strength_saber;
        [UIValue("strength_saber")]
        public float strength_saber
        {
            get => _strength_saber;
            set => _strength_saber = value;
        }

        private float _strength_wall;
        [UIValue("strength_wall")]
        public float strength_wall
        {
            get => _strength_wall;
            set => _strength_wall = value;
        }

        private float _strength_ui;
        [UIValue("strength_ui")]
        public float strength_ui
        {
            get => _strength_ui;
            set => _strength_ui = value;
        }

        [UIAction("rumble_note_click")]
        private void OnRumbleNoteClick()
        {
            RumbleTest(_strength, _duration);
        }

        [UIAction("rumble_saber_click")]
        private void OnRumbleSaberClick()
        {
            RumbleTest(_strength_saber, 1.0f);
        }

        [UIAction("rumble_wall_click")]
        private void OnRumbleWallClick()
        {
            RumbleTest(_strength_wall, 1.0f);
        }

        [UIAction("rumble_ui_click")]
        private void OnRumbleUIClick()
        {
            RumbleTest(_strength_ui, 0);
        }

        [UIAction("#apply")]
        public void OnApply()
        {
            Logger.log?.Info($"Apply: enabled={_modEnabled}, strength={_strength}, duration={_duration}, strength_saber={_strength_saber}, strength_wall={_strength_wall}, strength_ui={strength_ui}");
            Configuration.PluginConfig.Instance.enabled = _modEnabled;
            Configuration.PluginConfig.Instance.strength = _strength;
            Configuration.PluginConfig.Instance.duration = _duration;
            Configuration.PluginConfig.Instance.strength_saber = _strength_saber;
            Configuration.PluginConfig.Instance.strength_wall = _strength_wall;
            Configuration.PluginConfig.Instance.strength_ui = _strength_ui;

            if (_modEnabled)
            {
                Plugin.ApplyHarmonyPatches();
            }
            else
            {
                Plugin.RemoveHarmonyPatches();
            }
        }

        private void Awake()
        {
            _isIndex = XRDevice.model.IndexOf("index", StringComparison.OrdinalIgnoreCase) >= 0;
            Logger.log?.Info($"XRDevice.model: {XRDevice.model}");

            _isOculusPlatform = XRSettings.loadedDeviceName.IndexOf("oculus", StringComparison.OrdinalIgnoreCase) >= 0;
            Logger.log?.Info($"XRSettings.loadedDeviceName: {XRSettings.loadedDeviceName}");

            ReadCurrentSettings();
            Logger.log?.Info($"Awake: enabled={_modEnabled}, strength={_strength}, duration={_duration}, strength_saber={_strength_saber}, strength_wall={_strength_wall}, strength_ui={_strength_ui}");
        }

        private void ReadCurrentSettings()
        {
            _modEnabled = Configuration.PluginConfig.Instance.enabled;
            _strength = Configuration.PluginConfig.Instance.strength;
            _duration = Configuration.PluginConfig.Instance.duration;
            _strength_saber = Configuration.PluginConfig.Instance.strength_saber;
            _strength_wall = Configuration.PluginConfig.Instance.strength_wall;
            _strength_ui = Configuration.PluginConfig.Instance.strength_ui;
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

        private void RumbleTest(float strength, float duration)
        {
            Logger.log?.Info($"RumbleTest: strength={strength}, duration={duration}");

            if (_vrPlatformHelper == null)
            {
                _vrPlatformHelper = Resources.FindObjectsOfTypeAll<VRPlatformHelper>().FirstOrDefault();
                if (_vrPlatformHelper == null)
                {
                    _vrPlatformHelper = new GameObject().AddComponent<VRPlatformHelper>();
                }
            }

            if (_vrPlatformHelper != null)
            {
                OculusVRHelper oculusVRHelper = _vrPlatformHelper.GetField<OculusVRHelper, VRPlatformHelper>("_oculusVRHelper");
                if (oculusVRHelper != null)
                {
                    oculusVRHelper.SetField<OculusVRHelper, OVRHapticsClip>("_hapticsClip", CreateHapticsClip(strength));
                }

                if (duration == 0)
                {
                    _vrPlatformHelper.TriggerHapticPulse(XRNode.RightHand, _strength_ui);
                    _vrPlatformHelper.TriggerHapticPulse(XRNode.LeftHand, _strength_ui);
                }
                else
                {
                    StartCoroutine(OneShotRumbleCoroutine(XRNode.RightHand, duration, strength));
                    StartCoroutine(OneShotRumbleCoroutine(XRNode.LeftHand, duration, strength));
                }
            }
            else
            {
                Logger.log?.Critical("Error getting VRPlatformHelper");
            }
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

        // make sure call this before patching OculusVRHelper.TriggerHapticPulse
        public void UpdateHapticsClips()
        {
            hapticsClipNote = CreateHapticsClip(Configuration.PluginConfig.Instance.strength);
            hapticsClipSaber = CreateHapticsClip(Configuration.PluginConfig.Instance.strength_saber);
            hapticsClipWall = CreateHapticsClip(Configuration.PluginConfig.Instance.strength_wall);
            hapticsClipUI = CreateHapticsClip(Configuration.PluginConfig.Instance.strength_ui);
        }
    }
}
