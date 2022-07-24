using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace Bloodthirst.Core.Utils
{
    public class AdvancedTypeCacheAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        internal Dictionary<Type, AdvancedTypeCache.TypeInformation> cache = new Dictionary<Type, AdvancedTypeCache.TypeInformation>();

        /// <summary>
        /// Contains relative paths of types we need to add after assemblyReload
        /// </summary>
        [OdinSerialize]
        internal List<string> newlyAddedScripts = new List<string>();

        [OdinSerialize]
        internal List<string> removedScripts = new List<string>();

        [OdinSerialize]
        internal AssemblyDefinitionAsset[] assmeblyDefs;

        public IReadOnlyDictionary<Type, AdvancedTypeCache.TypeInformation> Cache => cache;


        [Button]
        private void FullRefresh()
        {
            AdvancedTypeCache.StartThread();
        }

        [Button]
        private void Test()
        {

            foreach (AssemblyDefinitionAsset asm in assmeblyDefs)
            {
                AssemblyDefinitionImporter asmImporter = (AssemblyDefinitionImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asm));

                AssemblyDefinitionData asmData = new AssemblyDefinitionData();
                EditorJsonUtility.FromJsonOverwrite(asm.text , asmData);
            }

        }

        private class AssemblyDefinitionData
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public string[] versionDefines;
            public bool noEngineReferences;
        }
    }
}
