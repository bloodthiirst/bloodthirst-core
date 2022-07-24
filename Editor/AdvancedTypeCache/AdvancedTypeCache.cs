using Bloodthirst.Editor.AssetProcessing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Core.Utils
{

    [InitializeOnLoad]
    public static class AdvancedTypeCache
    {
        public class TypeInformation
        {
            [OdinSerialize]
            public Type type;

            [OdinSerialize]
            public TextAsset unityScript;

            [OdinSerialize]
            public string unityScriptPath;
        }

        private struct TextAssetPathPair
        {
            public MonoScript TextAsset { get; set; }
            public string AssetPath { get; set; }
            public string TextContent { get; set; }
        }

        private const string ASSET_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/AdvancedTypeCache/AdvancedTypeCacheAsset.asset";

        private static AdvancedTypeCacheAsset cacheAsset;

        public static AdvancedTypeCacheAsset CacheAsset
        {
            get
            {
                if (cacheAsset == null)
                {
                    cacheAsset = new AdvancedTypeCacheAsset();
                    AssetDatabase.CreateAsset(cacheAsset, ASSET_PATH);
                }

                return cacheAsset;
            }
        }

        private static string pathToProject;

        private static CSharpCodeProvider provider;


        static AdvancedTypeCache()
        {
            cacheAsset = AssetDatabase.LoadAssetAtPath<AdvancedTypeCacheAsset>(ASSET_PATH);
            pathToProject = EditorUtils.PathToProject;

            AssemblyReloadEvents.beforeAssemblyReload += BeforeReload;
            AssemblyReloadEvents.afterAssemblyReload += AfterReload;

            ScriptAssetWatcher.OnScriptCreated += HandleCreated;
            ScriptAssetWatcher.OnScriptMoved += HandleCreated;
            ScriptAssetWatcher.OnScriptEdited += HandleEdited;
            ScriptAssetWatcher.OnScriptRemoved += HandleRemoved;
        }

        private static void BeforeReload()
        {
            if (provider == null)
                return;

            provider.Dispose();
            provider = null;
        }

        private static void AfterReload()
        {
            provider = new CSharpCodeProvider();

            TreatRemovedScripts();
            TreatNewScripts();
        }

        private static void TreatNewScripts()
        {
            List<Type> allTypes = TypeUtils.AllTypes.ToList();

            for (int i = CacheAsset.newlyAddedScripts.Count - 1; i >= 0; i--)
            {
                string curr = CacheAsset.newlyAddedScripts[i];

                TextAsset scriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(curr);

                List<BaseTypeDeclarationSyntax> types = TypesDeclaredInFile(scriptAsset.text);


                for (int j = 0; j < types.Count; j++)
                {
                    BaseTypeDeclarationSyntax c = types[j];
                    string classAsString = c.Identifier.Text;

                    Type findType = allTypes.FirstOrDefault(t => t.Name == classAsString);

                    // if type already exists
                    if (findType == null)
                    {
                        //Debug.Log($"New type to add now found {classAsString}");
                        continue;
                    }

                    if (cacheAsset.cache.TryGetValue(findType, out TypeInformation info))
                    {
                        //Debug.Log($"{findType.Name} already exists");
                    }
                    else
                    {
                        info = new TypeInformation();
                        cacheAsset.cache.Add(findType, info);
                    }

                    info.type = findType;
                    info.unityScriptPath = curr;
                    info.unityScript = scriptAsset;

                }

                // rempve from the newly added scripts
                CacheAsset.newlyAddedScripts.RemoveAt(i);
            }
        }

        private static void TreatRemovedScripts()
        {
            List<Type> typesToRemove = ListPool<Type>.Get();

            for (int i = CacheAsset.removedScripts.Count - 1; i >= 0; i--)
            {
                string currPath = CacheAsset.removedScripts[i];

                foreach (KeyValuePair<Type, TypeInformation> kv in CacheAsset.cache)
                {
                    if (kv.Value.unityScriptPath == currPath)
                    {
                        typesToRemove.Add(kv.Key);
                    }
                }

                // rempve from the newly added scripts
                CacheAsset.removedScripts.RemoveAt(i);
            }

            foreach (Type k in typesToRemove)
            {
                CacheAsset.cache.Remove(k);
            }

            ListPool<Type>.Release(typesToRemove);
        }

        private static void HandleRemoved(MonoImporter monoImporter)
        {
            cacheAsset.removedScripts.Add(monoImporter.assetPath);
        }

        private static void HandleCreated(MonoImporter monoImporter)
        {
            cacheAsset.newlyAddedScripts.Add(monoImporter.assetPath);
        }

        private static void HandleEdited(MonoImporter monoImporter)
        {
            cacheAsset.newlyAddedScripts.Add(monoImporter.assetPath);
            cacheAsset.removedScripts.Add(monoImporter.assetPath);
        }

        internal static void StartThread()
        {
            List<TextAssetPathPair> deps = FetchUnityAssets().ToList();

            Thread t = new(() =>
            {
                cacheAsset.cache = ExtractInfo(deps);
                cacheAsset.newlyAddedScripts.Clear();
            });

            t.Start();
        }

        private static IEnumerable<TextAssetPathPair> FetchUnityAssets()
        {
            foreach (MonoScript t in EditorUtils.FindScriptAssets())
            {
                yield return new TextAssetPathPair() { TextAsset = t, AssetPath = AssetDatabase.GetAssetPath(t), TextContent = t.text };
            }
        }

        private static List<BaseTypeDeclarationSyntax> TypesDeclaredInFile(string fileContent)
        {
            List<BaseTypeDeclarationSyntax> typesList = new List<BaseTypeDeclarationSyntax>();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fileContent);

            CompilationUnitSyntax root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            // get classes inside namespaces
            foreach (NamespaceDeclarationSyntax s in root.Members.OfType<NamespaceDeclarationSyntax>())
            {
                typesList.AddRange(s.Members.OfType<ClassDeclarationSyntax>());
                typesList.AddRange(s.Members.OfType<StructDeclarationSyntax>());
                typesList.AddRange(s.Members.OfType<EnumDeclarationSyntax>());
                typesList.AddRange(s.Members.OfType<InterfaceDeclarationSyntax>());
            }

            // get classes in file directly
            foreach (BaseTypeDeclarationSyntax s in root.Members.OfType<BaseTypeDeclarationSyntax>())
            {
                typesList.Add(s);
            }

            return typesList;
        }

        /// <summary>
        /// Extracts info about the classes that follow the BISD pattern
        /// </summary>
        /// <param name="TypeList"></param>
        /// <param name="TextList"></param>
        private static Dictionary<Type, TypeInformation> ExtractInfo(List<TextAssetPathPair> deps)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            int id = Progress.Start("Registering AdvancedTypeCahce", null, Progress.Options.None);

            Progress.Report(id, 0f);

            try
            {
                Dictionary<Type, TypeInformation> typeList = new Dictionary<Type, TypeInformation>();

                List<Type> allTypes = TypeUtils.AllTypes.ToList();

                for (int i = 0; i < deps.Count; i++)
                {
                    TextAssetPathPair d = deps[i];
                    string relativePath = d.AssetPath;

                    string systemPath = pathToProject + relativePath;

                    List<BaseTypeDeclarationSyntax> typesList = TypesDeclaredInFile(d.TextContent);

                    foreach (BaseTypeDeclarationSyntax c in typesList)
                    {
                        string classAsString = c.Identifier.Text;

                        Type findType = allTypes.FirstOrDefault(t => t.Name == classAsString);

                        if (findType != null)
                        {
                            typeList.TryAdd(findType, new TypeInformation()
                            {
                                type = findType,
                                unityScriptPath = relativePath,
                                unityScript = d.TextAsset
                            });
                        }
                    }

                    Progress.Report(id, i / (float)(deps.Count - 1));

                }

                return typeList;
            }

            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                provider.Dispose();
                Progress.Remove(id);
            }

            return null;

        }
    }
}
