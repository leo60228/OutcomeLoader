using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;

namespace OutcomeLoader {
    public class Patches {
        internal static ManualLogSource Logger;

        [HarmonyPatch(typeof(OutcomeBase), "OnAfterDeserialize")]
        [HarmonyPostfix]
        private static void OutcomeBaseOnAfterDeserialize(OutcomeBase __instance) {
            string json = JsonUtility.ToJson(__instance);
            Encoding enc = Encoding.UTF8;
            byte[] jsonBytes = enc.GetBytes(json);
            byte[] hash;

            using (SHA256 hasher = new SHA256Managed()) {
                hash = hasher.ComputeHash(jsonBytes);
            }

            Logger.LogInfo($"deserialize {BitConverter.ToString(hash)}");
        }
    }
}
