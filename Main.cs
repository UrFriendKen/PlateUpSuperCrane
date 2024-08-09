﻿using HarmonyLib;
using KitchenMods;
using PreferenceSystem;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenSuperCrane
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Super Crane";
        public const string MOD_VERSION = "0.1.0";

        internal const string ALLOW_CRANE_DURING_PREP_ID = "allowCraneDuringPrep";
        internal const string ALLOW_CRANE_DURING_PRACTICE_ID = "allowCraneDuringPractice";
        internal const string ALLOW_CRANE_DURING_DAY_ID = "allowCraneDuringDay";
        internal const string ALLOW_CRANE_IN_HQ_ID = "allowCraneInHQ";
        internal const string ALLOW_CRANE_IN_FRANCHISE_BUILDER_ID = "allowCraneInFranchiseBuilder";

        internal const string MOVEMENT_CRANE_SPEED_FACTOR_ID = "movementCraneSpeedFactor";
        internal static readonly float[] AllowedCraneSpeedFactors = new float[] {
            0.2f, 0.4f, 0.6f, 0.8f, 1f,
            1.2f, 1.4f, 1.6f, 1.8f, 2f,
            2.2f, 2.4f, 2.6f, 2.8f, 3f,
            3.2f, 3.4f, 3.6f, 3.8f, 4f,
            4.2f, 4.4f, 4.6f, 4.8f, 5f };

    internal static PreferenceSystemManager PrefManager;

        public Main()
        {
            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);

            PrefManager
                .AddLabel("Super Crane")
                .AddSubmenu("Crane Toggling", "CraneToggling")
                    .AddLabel("Crane Toggling")
                    .AddLabel("During Prep")
                    .AddOption<bool>(ALLOW_CRANE_DURING_PREP_ID,
                        true,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
                    .AddLabel("During Practice")
                    .AddOption<bool>(ALLOW_CRANE_DURING_PRACTICE_ID,
                        true,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
                    .AddLabel("During Day")
                    .AddOption<bool>(ALLOW_CRANE_DURING_DAY_ID,
                        true,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
                    .AddLabel("In HQ")
                    .AddOption<bool>(ALLOW_CRANE_IN_HQ_ID,
                        true,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
                    .AddLabel("In Franchise Builder")
                    .AddOption<bool>(ALLOW_CRANE_IN_FRANCHISE_BUILDER_ID,
                        true,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
                    .AddSpacer()
                    .AddSpacer()
                .SubmenuDone()
                .AddSubmenu("Movement", "Movement")
                    .AddLabel("Movement")
                    .AddLabel("Speed")
                    .AddOption<float>(MOVEMENT_CRANE_SPEED_FACTOR_ID,
                        1f,
                        AllowedCraneSpeedFactors,
                        AllowedCraneSpeedFactors.Select(x => $"{Mathf.Round(x * 100)}%").ToArray())
                    .AddSpacer()
                    .AddSpacer()
                .SubmenuDone()
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        public void PreInject()
        {
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
