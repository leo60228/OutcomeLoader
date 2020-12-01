using System.Collections.Generic;
using Mono.Cecil;

namespace OutcomeLoader.Patcher {
    public static class Patcher {
        public static IEnumerable<string> TargetDLLs { get; } = new[] {"Assembly-CSharp.dll"};

        public static void Patch(AssemblyDefinition assembly) {

        }
    }
}
