﻿using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace RumbleMod
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.nalulululuna.RumbleMod";
        internal static Harmony harmony => new Harmony(HarmonyId);

        internal static Plugin instance { get; private set; }
        internal static string Name => "RumbleMod";

        [Init]
        public Plugin(IPALogger logger, Config conf)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");

            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.log.Debug("Config loaded.");
        }

        [OnEnable]
        public void OnEnable()
        {
            BSMLSettings.instance.AddSettingsMenu("Rumble Mod", $"{Name}.Views.Settings.bsml", SettingsController.instance);
            if (Configuration.PluginConfig.Instance.enabled)
            {
                ApplyHarmonyPatches();
                HarmonyPatches.NoteCutHapticEffectHitNote.rumblePreset._duration = Configuration.PluginConfig.Instance.duration;
                HarmonyPatches.NoteCutHapticEffectHitNote.rumblePreset._strength = Configuration.PluginConfig.Instance.strength;
            }
        }

        [OnDisable]
        public void OnDisable()
        {
            RemoveHarmonyPatches();
            BSMLSettings.instance.RemoveSettingsMenu(SettingsController.instance);
        }

        public static void ApplyHarmonyPatches()
        {
            try
            {
                Logger.log?.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.log?.Critical("Error applying Harmony patches: " + ex.Message);
                Logger.log?.Debug(ex);
            }
        }

        public static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchAll(HarmonyId);
            }
            catch (Exception ex)
            {
                Logger.log?.Critical("Error removing Harmony patches: " + ex.Message);
                Logger.log?.Debug(ex);
            }
        }
    }
}
