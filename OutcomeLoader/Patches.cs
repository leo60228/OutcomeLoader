using System;
using BepInEx.Logging;
using HarmonyLib;

namespace OutcomeLoader {
    public class Patches {
        internal static ManualLogSource Logger;

        [HarmonyPatch(typeof(OutcomeBase), "OnAfterDeserialize")]
        [HarmonyPostfix]
        private static void OutcomeBaseOnAfterDeserialize(OutcomeBase __instance) {
            Logger.LogInfo("deserialize");
        }
    }
}
