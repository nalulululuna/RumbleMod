using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using VRUIControls;

namespace RumbleMod
{
    public class SettingsController : PersistentSingleton<SettingsController>
    {
        private bool _isIndex;
        private bool _isOculusPlatform;
        private HapticFeedbackController _hapticFeedbackController;
        private HapticPresetSO rumblePreset = new HapticPresetSO();

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
            _duration = 0.14f;
            _strength_saber = 1.0f;
            _strength_wall = 1.0f;
            _strength_ui = 1.0f;
            parserParams.EmitEvent("cancel");
        }

        [UIAction("recommended_click")]
        private void OnRecommendedClick()
        {
            _modEnabled = true;
            _strength = _isIndex ? 0.55f : 1.0f;
            _duration = _isIndex ? 0.16f : 0.14f;
            _strength_saber = 0.25f;
            _strength_wall = 0.25f;
            _strength_ui = 0.5f;
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
            RumbleTest(_strength_ui, 0.02f);
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
                // make sure to remove previous patches
                Plugin.RemoveHarmonyPatches();

                Plugin.ApplyHarmonyPatches();
                HarmonyPatches.NoteCutHapticEffectHitNote.rumblePreset._duration = Configuration.PluginConfig.Instance.duration;
                HarmonyPatches.NoteCutHapticEffectHitNote.rumblePreset._strength = Configuration.PluginConfig.Instance.strength;
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

        private void RumbleTest(float strength, float duration)
        {
            Logger.log?.Info($"RumbleTest: strength={strength}, duration={duration}");

            if (_hapticFeedbackController == null)
            {
                VRInputModule vrInputModule = Resources.FindObjectsOfTypeAll<VRInputModule>().FirstOrDefault();
                if (vrInputModule != null)
                {
                    _hapticFeedbackController = vrInputModule.GetField<HapticFeedbackController, VRInputModule>("_hapticFeedbackController");
                }
            }

            if (_hapticFeedbackController != null)
            {
                rumblePreset._strength = strength;
                rumblePreset._duration = duration;
                _hapticFeedbackController.PlayHapticFeedback(XRNode.LeftHand, rumblePreset);
                _hapticFeedbackController.PlayHapticFeedback(XRNode.RightHand, rumblePreset);
            }
            else
            {
                Logger.log?.Critical("Error getting HapticFeedbackController");
            }
        }
    }
}
