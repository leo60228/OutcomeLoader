using System;
using YamlDotNet.Serialization;
using UnityEngine;

namespace OutcomeLoader {
    internal class ScriptableObjectFactory : IObjectFactory {
        private readonly IObjectFactory fallback;

        public ScriptableObjectFactory(IObjectFactory fallback) {
            this.fallback = fallback;
        }

        public object Create(Type type) {
            if (typeof(ScriptableObject).IsAssignableFrom(type)) {
                return ScriptableObject.CreateInstance(type);
            } else {
                return fallback.Create(type);
            }
        }
    }
}
