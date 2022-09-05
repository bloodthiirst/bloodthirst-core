using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

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

        [Header("Assemblies to scan")]
        [OdinSerialize]
        internal AssemblyDefinitionAsset[] assmeblyDefs;

        [OdinSerialize]
        internal AssemblyDefinitionReferenceAsset[] assemblyReferences;

        public IReadOnlyDictionary<Type, AdvancedTypeCache.TypeInformation> Cache => cache;


        [Button]
        private void FullRefresh()
        {
            AdvancedTypeCache.StartThread();
        }

        [Button]
        internal IReadOnlyCollection<string> GetAssemblyPaths()
        {
            HashSet<string> assemblyPaths = new HashSet<string>();

            for (int i = 0; i < assmeblyDefs.Length; i++)
            {
                AssemblyDefinitionAsset currAssembly = assmeblyDefs[i];
                string pathToAsm = AssetDatabase.GetAssetPath(currAssembly);

                pathToAsm = Path.GetDirectoryName(pathToAsm);

                pathToAsm = pathToAsm.Replace(Path.DirectorySeparatorChar, '/');

                assemblyPaths.Add(pathToAsm);
            }

            for (int i = 0; i < assemblyReferences.Length; i++)
            {
                AssemblyDefinitionReferenceAsset currAssembly = assemblyReferences[i];

                /*
                AssemblyDefinitionReferenceJsonContent asJson = new AssemblyDefinitionReferenceJsonContent();
                EditorJsonUtility.FromJsonOverwrite(currAssembly.text, asJson);

                bool isValid = GUID.TryParse(asJson.reference.Substring(5), out GUID guid);
                string pathToAsm = AssetDatabase.GUIDToAssetPath(guid);
                */

                string pathToAsm = AssetDatabase.GetAssetPath(currAssembly);

                pathToAsm = Path.GetDirectoryName(pathToAsm);

                pathToAsm = pathToAsm.Replace(Path.DirectorySeparatorChar, '/');

                assemblyPaths.Add(pathToAsm);
            }


            return assemblyPaths;
        }

        internal class AssemblyDefinitionJsonContent
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

        internal class AssemblyDefinitionReferenceJsonContent
        {
            public string reference;
        }
    }
}
