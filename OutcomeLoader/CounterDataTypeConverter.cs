using System;
using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using UnityEngine;
using AdventureCore;

namespace OutcomeLoader {
    internal class CounterDataTypeConverter : IYamlTypeConverter {
        public bool Accepts(Type type) {
            return type == typeof(CounterData);
        }

        public object ReadYaml(IParser parser, Type type) {
            string val = parser.Consume<Scalar>().Value;
            if (val == "") {
                return null;
            }
            IEnumerable<CounterData> objects = Resources.FindObjectsOfTypeAll(typeof(CounterData)).Cast<CounterData>();
            CounterData obj = objects.First(x => x.Guid == val);
            if (obj == null) {
                throw new SemanticErrorException($"Missing Guid {val} for CounterData");
            }
            return obj;
        }

        public void WriteYaml(IEmitter emitter, object val, Type type) {
            if (val == null) {
                emitter.Emit(new Scalar(null, null, "", ScalarStyle.Any, true, false));
            } else {
                emitter.Emit(new Scalar(null, null, ((CounterData) val).Guid, ScalarStyle.Any, true, false));
            }
        }
    }
}
