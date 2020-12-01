using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using BepInEx.Logging;

namespace OutcomeLoader.Patcher {
    public static class Patcher {
        public static IEnumerable<string> TargetDLLs { get; } = new[] {"Assembly-CSharp.dll"};
        private static ManualLogSource Logger { get; } = BepInEx.Logging.Logger.CreateLogSource("OutcomeLoaderPatcher");

        public static void Patch(AssemblyDefinition assembly) {
            var module = assembly.MainModule;
            var outcomeBase = module.GetType(null, "OutcomeBase");
            var voidType = module.ImportReference(typeof(void));

            var beforeSerialize = new MethodDefinition("OnBeforeSerialize", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, voidType);
            beforeSerialize.NoInlining = true;
            beforeSerialize.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            TypeReference debug = null;
            module.TryGetTypeReference("UnityEngine.CoreModule", "UnityEngine.Debug", out debug);
            var log = new MethodReference("Log", voidType, debug);
            log.Parameters.Add(new ParameterDefinition(module.ImportReference(typeof(object))));

            var afterDeserialize = new MethodDefinition("OnAfterDeserialize", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, voidType);
            afterDeserialize.NoInlining = true;
            afterDeserialize.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            TypeReference callbackReceiver = null;
            module.TryGetTypeReference("UnityEngine.CoreModule", "UnityEngine.ISerializationCallbackReceiver", out callbackReceiver);
            var impl = new InterfaceImplementation(callbackReceiver);
            var iBeforeSerialize = new MethodReference("OnBeforeSerialize", voidType, callbackReceiver);
            iBeforeSerialize.HasThis = true;
            var iAfterDeserialize = new MethodReference("OnAfterDeserialize", voidType, callbackReceiver);
            iAfterDeserialize.HasThis = true;
            beforeSerialize.Overrides.Add(iBeforeSerialize);
            afterDeserialize.Overrides.Add(iAfterDeserialize);

            outcomeBase.Interfaces.Add(impl);
            outcomeBase.Methods.Add(beforeSerialize);
            outcomeBase.Methods.Add(afterDeserialize);
        }
    }
}
