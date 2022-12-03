#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
#if ODIN_INSPECTOR
	using Sirenix.Serialization;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Bloodthirst.Core.Utils
{
#if ODIN_INSPECTOR
    public class AdvancedTypeCacheAsset : SerializedScriptableObject
#else
    public class AdvancedTypeCacheAsset : ScriptableObject
#endif
    {
        #if ODIN_INSPECTOR[OdinSerialize]#endif
        internal Dictionary<Type, AdvancedTypeCache.TypeInformation> cache = new Dictionary<Type, AdvancedTypeCache.TypeInformation>();

        /// <summary>
        /// Contains relative paths of types we need to add after assemblyReload
        /// </summary>
        #if ODIN_INSPECTOR[OdinSerialize]#endif
        internal List<string> newlyAddedScripts = new List<string>();

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        internal List<string> removedScripts = new List<string>();

        [Header("Assemblies to scan")]
        #if ODIN_INSPECTOR[OdinSerialize]#endif
        internal AssemblyDefinitionAsset[] assmeblyDefs;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        internal AssemblyDefinitionReferenceAsset[] assemblyReferences;

        public IReadOnlyDictionary<Type, AdvancedTypeCache.TypeInformation> Cache => cache;


        #if ODIN_INSPECTOR[Button]#endif
        private void FullRefresh()
        {
            AdvancedTypeCache.StartThread();
        }

        #if ODIN_INSPECTOR[Button]#endif
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
