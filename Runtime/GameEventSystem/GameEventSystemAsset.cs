using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Bloodthirst.Core.GameEventSystem
{
    public class GameEventSystemAsset : SerializedScriptableObject
    {
        public struct EnumClassPair
        {
            [OdinSerialize]
            public string enumValue;

            [OdinSerialize]
            public string className;
        }

        public string enumName;
        public string namespaceValue;
        public AssemblyDefinitionAsset assemblyDefinition;

        [SerializeField]
        private List<EnumClassPair> classEnumPairs;

        public EnumClassPair this[int index]
        {
            get
            {
                return classEnumPairs[index];
            }
        }

        public int Count => classEnumPairs.Count;

        public IEnumerable<EnumClassPair> GetAll()
        {
            return classEnumPairs;
        }

        public bool HasClass(string className)
        {
            return classEnumPairs.Exists( p => p.className == className);
        }

        public bool HasEnum(string enumName)
        {
            return classEnumPairs.Exists(p => p.enumValue == enumName);
        }

        public void Add(string enumName , string className)
        {
            classEnumPairs.Add( new EnumClassPair() { className = className, enumValue = enumName } );
        }

        public void RemoveByIndex(int index)
        {
            classEnumPairs.RemoveAt(index);
        }

        public void RemoveByEnum(string enumName)
        {
            classEnumPairs.RemoveAll(p => p.enumValue == enumName);
        }

        public void RemoveByClass(string className)
        {
            classEnumPairs.RemoveAll(p => p.className == className);
        }
    }
}
