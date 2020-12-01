using System;
using HarmonyLib;
using BepInEx;

namespace OutcomeLoader {
    [BepInPlugin("space.leo60228.plugins.outcomeloader", "OutcomeLoader", "1.0.0.0")]
    public class OutcomeLoaderPlugin : BaseUnityPlugin {
        void Awake() {
            Logger.LogInfo("OutcomeLoader awake");
            Patches.Logger = Logger;
            Harmony.CreateAndPatchAll(typeof(Patches));
        }
    }
}
