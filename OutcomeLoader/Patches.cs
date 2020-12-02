using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectFactories;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using NodeEditorFramework;

namespace OutcomeLoader {
    public class Patches {
        internal static ManualLogSource Logger;

        static string ByteArrayToString(byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        [HarmonyPatch(typeof(OutcomeBase), "OnAfterDeserialize")]
        [HarmonyPostfix]
        private static void OutcomeBaseOnAfterDeserialize(OutcomeBase __instance) {
            ISerializer serializer = new SerializerBuilder()
                .WithTagMapping("!OutcomeBase", typeof(OutcomeBase))
                .WithTagMapping("!OutcomeActionAnimation", typeof(OutcomeActionAnimation))
                .WithTagMapping("!OutcomeActionBase", typeof(OutcomeActionBase))
                .WithTagMapping("!OutcomeActionChangeHero", typeof(OutcomeActionChangeHero))
                .WithTagMapping("!OutcomeActionChangeScene", typeof(OutcomeActionChangeScene))
                .WithTagMapping("!OutcomeActionChittr", typeof(OutcomeActionChittr))
                .WithTagMapping("!OutcomeActionConversation", typeof(OutcomeActionConversation))
                .WithTagMapping("!OutcomeActionCounters", typeof(OutcomeActionCounters))
                .WithTagMapping("!OutcomeActionCutscene", typeof(OutcomeActionCutscene))
                .WithTagMapping("!OutcomeActionFade", typeof(OutcomeActionFade))
                .WithTagMapping("!OutcomeActionInventory", typeof(OutcomeActionInventory))
                .WithTagMapping("!OutcomeActionMessage", typeof(OutcomeActionMessage))
                .WithTagMapping("!OutcomeActionMovement", typeof(OutcomeActionMovement))
                .WithTagMapping("!OutcomeActionNPCGoal", typeof(OutcomeActionNPCGoal))
                .WithTagMapping("!OutcomeActionPolynav", typeof(OutcomeActionPolynav))
                .WithTagMapping("!OutcomeActionSound", typeof(OutcomeActionSound))
                .WithTagMapping("!OutcomeActionTrial", typeof(OutcomeActionTrial))
                .WithTagMapping("!OutcomeActionUI", typeof(OutcomeActionUI))
                .WithTagMapping("!OutcomeActionUtility", typeof(OutcomeActionUtility))
                .WithTagMapping("!OutcomeActionVFX", typeof(OutcomeActionVFX))
                .WithTagMapping("!OutcomeActionZoom", typeof(OutcomeActionZoom))
                .WithTagMapping("!CustomOutcome", typeof(CustomOutcome))
                .WithTagMapping("!OutcomeSwitchCameraTarget", typeof(OutcomeSwitchCameraTarget))
                .WithAttributeOverride(typeof(Node), "nodeKnobs", new YamlIgnoreAttribute())
                .WithAttributeOverride(typeof(Node), "Inputs", new YamlIgnoreAttribute())
                .WithAttributeOverride(typeof(Node), "Outputs", new YamlIgnoreAttribute())
                .WithAttributeOverride(typeof(Node), "rect", new YamlIgnoreAttribute())
                .WithAttributeOverride(typeof(Node), "_nodeBoxStyle", new YamlIgnoreAttribute())
                .WithAttributeOverride(typeof(UnityEngine.Object), "hideFlags", new YamlIgnoreAttribute())
                .WithAttributeOverride(typeof(UnityEngine.Object), "name", new YamlIgnoreAttribute())
                .WithTypeInspector(x => new ConditionTypeInspector(x))
                .EnsureRoundtrip()
                .Build();

            string yaml = serializer.Serialize(__instance);
            Encoding enc = Encoding.UTF8;
            byte[] yamlBytes = enc.GetBytes(yaml);
            byte[] hash;

            using (SHA256 hasher = new SHA256Managed()) {
                hash = hasher.ComputeHash(yamlBytes);
            }

            string hashStr = ByteArrayToString(hash);

            string dumpPath = Path.Combine("dumpedOutcomes", $"{hashStr}.yml");
            string overridePath = Path.Combine("overrideOutcomes", $"{hashStr}.yml");

            if (File.Exists(overridePath)) {
                string overrideYaml = File.ReadAllText(overridePath);
                IDeserializer deserializer = new DeserializerBuilder()
                    .WithTagMapping("!OutcomeBase", typeof(OutcomeBase))
                    .WithTagMapping("!OutcomeActionAnimation", typeof(OutcomeActionAnimation))
                    .WithTagMapping("!OutcomeActionBase", typeof(OutcomeActionBase))
                    .WithTagMapping("!OutcomeActionChangeHero", typeof(OutcomeActionChangeHero))
                    .WithTagMapping("!OutcomeActionChangeScene", typeof(OutcomeActionChangeScene))
                    .WithTagMapping("!OutcomeActionChittr", typeof(OutcomeActionChittr))
                    .WithTagMapping("!OutcomeActionConversation", typeof(OutcomeActionConversation))
                    .WithTagMapping("!OutcomeActionCounters", typeof(OutcomeActionCounters))
                    .WithTagMapping("!OutcomeActionCutscene", typeof(OutcomeActionCutscene))
                    .WithTagMapping("!OutcomeActionFade", typeof(OutcomeActionFade))
                    .WithTagMapping("!OutcomeActionInventory", typeof(OutcomeActionInventory))
                    .WithTagMapping("!OutcomeActionMessage", typeof(OutcomeActionMessage))
                    .WithTagMapping("!OutcomeActionMovement", typeof(OutcomeActionMovement))
                    .WithTagMapping("!OutcomeActionNPCGoal", typeof(OutcomeActionNPCGoal))
                    .WithTagMapping("!OutcomeActionPolynav", typeof(OutcomeActionPolynav))
                    .WithTagMapping("!OutcomeActionSound", typeof(OutcomeActionSound))
                    .WithTagMapping("!OutcomeActionTrial", typeof(OutcomeActionTrial))
                    .WithTagMapping("!OutcomeActionUI", typeof(OutcomeActionUI))
                    .WithTagMapping("!OutcomeActionUtility", typeof(OutcomeActionUtility))
                    .WithTagMapping("!OutcomeActionVFX", typeof(OutcomeActionVFX))
                    .WithTagMapping("!OutcomeActionZoom", typeof(OutcomeActionZoom))
                    .WithTagMapping("!CustomOutcome", typeof(CustomOutcome))
                    .WithTagMapping("!OutcomeSwitchCameraTarget", typeof(OutcomeSwitchCameraTarget))
                    .WithAttributeOverride(typeof(Node), "nodeKnobs", new YamlIgnoreAttribute())
                    .WithAttributeOverride(typeof(Node), "Inputs", new YamlIgnoreAttribute())
                    .WithAttributeOverride(typeof(Node), "Outputs", new YamlIgnoreAttribute())
                    .WithAttributeOverride(typeof(Node), "rect", new YamlIgnoreAttribute())
                    .WithAttributeOverride(typeof(Node), "_nodeBoxStyle", new YamlIgnoreAttribute())
                    .WithAttributeOverride(typeof(UnityEngine.Object), "hideFlags", new YamlIgnoreAttribute())
                    .WithAttributeOverride(typeof(UnityEngine.Object), "name", new YamlIgnoreAttribute())
                    .WithTypeInspector(x => new ConditionTypeInspector(x))
                    .WithObjectFactory(new ScriptableObjectFactory(new DefaultObjectFactory()))
                    .Build();
                OutcomeBase newOutcome = deserializer.Deserialize<OutcomeBase>(overrideYaml);
                __instance.StartDelay = newOutcome.StartDelay;
                __instance.ActionsList = newOutcome.ActionsList;
                Logger.LogInfo($"override {hashStr} <- {dumpPath}");
            } else {
                File.WriteAllText(dumpPath, yaml);
                Logger.LogInfo($"deserialize {hashStr} -> {dumpPath}");
            }
        }
    }
}
