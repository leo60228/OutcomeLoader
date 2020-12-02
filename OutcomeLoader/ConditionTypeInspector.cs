using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;
using UnityEngine;
using AdventureCore;
using HarmonyLib;

namespace OutcomeLoader {
    internal class ConditionTypeInspector : TypeInspectorSkeleton {
        private readonly ITypeInspector innerTypeDescriptor;

        private interface IPropertyConverter<T1, T2> {
            T1 Parse(T2 val);
            T2 Generate(T1 val);
        }

        private class ConditionPropertyDescriptor<T1, T2> : IPropertyDescriptor {
            protected readonly FieldInfo field;
            protected readonly IPropertyConverter<T1, T2> converter;

            public Type Type => converter == null ? field.FieldType : typeof(T2);
            public string Name => field.Name;
            public bool CanWrite => true;

            public Type TypeOverride { get; set; } = null;
            public int Order { get; set; } = 0;
            public ScalarStyle ScalarStyle { get; set; } = ScalarStyle.Any;

            public ConditionPropertyDescriptor(string name, IPropertyConverter<T1, T2> converter) {
                this.converter = converter;
                field = AccessTools.Field(typeof(Condition), name);

                if (!typeof(T1).IsAssignableFrom(field.FieldType)) {
                    throw new ArgumentException("Bad T1!");
                }
            }

            public T GetCustomAttribute<T>() where T: Attribute {
                return default(T);
            }

            public IObjectDescriptor Read(object target) {
                object realValue = field.GetValue(target);
                Type realType = Type;
                if (converter != null) {
                    realValue = converter.Generate((T1) realValue);
                    realType = typeof(T2);
                }
                return new ObjectDescriptor(realValue, realValue.GetType(), realType);
            }

            public void Write(object target, object val) {
                if (converter != null) {
                    val = converter.Parse((T2) val);
                }
                field.SetValue(target, val);
            }
        }

        private class ConditionPropertyDescriptor : ConditionPropertyDescriptor<object, object> {
            public ConditionPropertyDescriptor(string name) : base(name, null) {}
        }

        private class StaticDataObjConverter<T> : IPropertyConverter<T, string> where T: StaticDataObj {
            public T Parse(string val) {
                IEnumerable<T> objects = Resources.FindObjectsOfTypeAll(typeof(T)).Cast<T>();
                T obj = objects.First(x => x.Guid == val);
                if (obj == null) {
                    throw new SemanticErrorException($"Missing Guid {val} for {typeof(T).FullName}");
                }
                return obj;
            }

            public string Generate(T val) {
                return val.Guid;
            }
        }

        private class ArrayConverter<T1, T2> : IPropertyConverter<T1[], T2[]> {
            private readonly IPropertyConverter<T1, T2> parent;

            public ArrayConverter(IPropertyConverter<T1, T2> parent) {
                this.parent = parent;
            }

            public T1[] Parse(T2[] val) {
                return val.Select(x => parent.Parse(x)).ToArray();
            }

            public T2[] Generate(T1[] val) {
                return val.Select(x => parent.Generate(x)).ToArray();
            }
        }

        public ConditionTypeInspector(ITypeInspector innerTypeDescriptor) {
            this.innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException(nameof(innerTypeDescriptor));
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) {
            foreach (IPropertyDescriptor property in innerTypeDescriptor.GetProperties(type, container)) {
                yield return property;
            }

            if (type == typeof(Condition)) {
                yield return new ConditionPropertyDescriptor("_never");
                yield return new ConditionPropertyDescriptor("CurrentScene");
                yield return new ConditionPropertyDescriptor("LastScene");
                yield return new ConditionPropertyDescriptor<ItemData[], string[]>("_requiresOneOfTheseItems", new ArrayConverter<ItemData, string>(new StaticDataObjConverter<ItemData>()));
                yield return new ConditionPropertyDescriptor<ItemData[], string[]>("_requiresAllOfTheseItems", new ArrayConverter<ItemData, string>(new StaticDataObjConverter<ItemData>()));
                yield return new ConditionPropertyDescriptor<ItemData[], string[]>("_mustNotHaveTheseItems", new ArrayConverter<ItemData, string>(new StaticDataObjConverter<ItemData>()));
                yield return new ConditionPropertyDescriptor<AbilityData[], string[]>("_mustHaveTheseAbilities", new ArrayConverter<AbilityData, string>(new StaticDataObjConverter<AbilityData>()));
                yield return new ConditionPropertyDescriptor<AbilityData[], string[]>("_mustNotHaveTheseAbilities", new ArrayConverter<AbilityData, string>(new StaticDataObjConverter<AbilityData>()));
            }
        }
    }
}
