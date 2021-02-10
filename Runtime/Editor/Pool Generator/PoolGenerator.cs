using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using static Bloodthirst.Core.Utils.StringExtensions;
using System.Text;

namespace Bloodthirst.Core.AdvancedPool.Editor
{
    /// <summary>
    /// <para> This class auto-generates pools of the prefabs marked with the <see cref="GeneratePool"></see></para>
    /// <para> The pools are created in the "PoolScene" , a scene that is specifically made to contain thses auto-generated pools</para>
    /// </summary>
    public class PoolGenerator
    {
        #region global pool container
        private const string GLOBAL_POOL_TEMPLATE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Pool Generator/GlobalPoolContainer.cs.txt";
        private const string GLOBAL_POOL_START = "// [START_POOLS]";
        private const string GLOBAL_POOL_END = "// [END_POOLS]";
        #endregion

        #region auto-gen pools
        private const string POOL_TEMPLATE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Pool Generator/Template.Pool.cs.txt";
        private const string POOL_SCRIPTS_PATH = "Assets/Scripts/Pools";
        private const string POOL_SCENE_PATH = "Assets/Scenes/PoolScene";
        private const string REPLACE_KEYWORD = "[BEHAVIOUR]";
        #endregion

        private static readonly string[] filterFiles =
        {
            nameof(GeneratePool),
            nameof(PoolGenerator),
            "Template.Pool.cs",
            "GlobalPoolContainer.cs",
            "GlobalPoolContainer"
        };

        private static List<Type> poolalbeTypes;
        private static List<Type> PoolableTypes
        {
            get
            {
                if (poolalbeTypes == null)
                {
                    poolalbeTypes = GetPoolableTypes();
                }

                return poolalbeTypes;
            }
        }

        private static List<Component> poolablePrefabs;
        private static List<Component> PoolablePrefabs
        {
            get
            {
                if (poolablePrefabs == null)
                {
                    poolablePrefabs = GetAllPoolablePrefabs();
                }
                return poolablePrefabs;
            }
        }

        [MenuItem("Bloodthirst Tools/AutoGen Pools/Refresh Pools")]
        public static void ManualPoolTrigger()
        {
            RefreshPools();
        }

        [DidReloadScripts(SingletonScriptableObjectInit.SINGLETONS_CREATION_CHECK)]
        public static void OnDidReloadScripts()
        {
            // TODO : skip trigger when exiting play mode too
            // do that for every DidReloadScripts call
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            RefreshPools();
        }

        private static void RefreshPools()
        {
            // there are no poolable types that need to be treated
            if (PoolableTypes.Count == 0)
            {
                return;
            }

            // there are no poolable prfabs that need to be treated
            if (PoolablePrefabs.Count == 0)
            {
                return;
            }

            // create folder for the pool scripts
            EditorUtils.CreateFoldersFromPath(POOL_SCRIPTS_PATH);

            bool hasGeneratedScript = false;

            // generate the scripts
            foreach (Type t in PoolableTypes)
            {
                Type searchForPool = TypeUtils.AllTypes.FirstOrDefault(p => p.Name.Equals($"{t.Name}Pool"));

                // pool class already exists
                if (searchForPool != null)
                    continue;

                hasGeneratedScript = true;

                string relativePath = $"{POOL_SCRIPTS_PATH}/{$"{t.Name}Pool.cs"}";
                string pathToProject = EditorUtils.PathToProject;

                File.WriteAllText(pathToProject + relativePath, AssetDatabase.LoadAssetAtPath<TextAsset>(POOL_TEMPLATE).text.Replace(REPLACE_KEYWORD, t.Name));

                AssetDatabase.ImportAsset(relativePath);
            }

            // exit and wait for next assembly reload to repass from here and continue
            if (hasGeneratedScript)
                return;

            // generate global pool container
            Type globalPoolContainerType = TypeUtils.AllTypes.FirstOrDefault(p => p.Name.Equals("GlobalPoolContainer"));

            if (globalPoolContainerType == null)
            {
                string relativePath = $"{POOL_SCRIPTS_PATH}/GlobalPoolContainer.cs";
                string pathToProject = EditorUtils.PathToProject;

                File.WriteAllText(pathToProject + relativePath, AssetDatabase.LoadAssetAtPath<TextAsset>(GLOBAL_POOL_TEMPLATE).text);

                AssetDatabase.ImportAsset(relativePath);

                return;
            }

            // check if we have pool scene or not
            bool hasPoolScene = GetPoolScenePath(out string poolScenePath);

            //create it if it's not found
            // return since the SceneManager script generation will trigger domain reload
            if (!hasPoolScene)
            {
                EditorApplication.update -= CreatePoolScene;
                EditorApplication.update += CreatePoolScene;

                return;
            }

            EditorApplication.update -= CheckForPoolsInScene;
            EditorApplication.update += CheckForPoolsInScene;
        }

        private static bool GetPoolScenePath(out string poolScenePath)
        {
            string res = null;

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                res = EditorBuildSettings.scenes[i].path;

                string sceneName = res.Remove(res.Length - 6).Split('/').Last();

                if (sceneName.Equals("PoolScene"))
                {
                    poolScenePath = res;
                    return true;
                }
            }

            poolScenePath = null;
            return false;
        }

        private static void RefreshGlobalPoolContainer()
        {

        }

        /// <summary>
        /// Check the pools scene to see if we need to instantial missing pools
        /// </summary>
        private static void CheckForPoolsInScene()
        {
            EditorApplication.update -= CheckForPoolsInScene;

            if (!GetPoolScenePath(out string poolScenePath))
            {
                Debug.LogError("PoolScene not found in the project");
            }

            Scene poolsScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(poolScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);

            Type globalPoolContainerType = TypeUtils.AllTypes.FirstOrDefault(p => p.Name.Equals("GlobalPoolContainer"));

            // get or create global pool container
            GameObject globalPoolContainer = poolsScene
                .GetRootGameObjects()
                .FirstOrDefault(go => go.GetComponent(globalPoolContainerType) != null);

            if (globalPoolContainer == null)
            {
                globalPoolContainer = new GameObject(globalPoolContainerType.Name);
                globalPoolContainer.AddComponent(globalPoolContainerType);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(globalPoolContainer, poolsScene);
            }

            // get all pools
            List<IPoolBehaviour> allPools = poolsScene
                .GetRootGameObjects()
                .Select(go => go.GetComponent<IPoolBehaviour>())
                .Where(c => c != null).ToList();

            List<IPoolBehaviour> poolsInScene = new List<IPoolBehaviour>();

            // TODO : generate a pool for each prefab and all it to the "Pools" scene
            foreach (Component poolablePrefab in PoolablePrefabs)
            {
                IPoolBehaviour pool = allPools.FirstOrDefault(p => p.Prefab == poolablePrefab.gameObject);

                // if pool exists , skip
                if (pool != null)
                {
                    poolsInScene.Add(pool);
                    continue;
                }

                GameObject newPoolGo = new GameObject($"Auto-Generated Pool [ {poolablePrefab.name} ]");
                Type poolType = TypeUtils.AllTypes.FirstOrDefault(t => t.Name.Equals(poolablePrefab.GetType().Name + "Pool"));

                IPoolBehaviour poolComponent = newPoolGo.AddComponent(poolType) as IPoolBehaviour;

                // assign the prefab
                poolComponent.Prefab = poolablePrefab.gameObject;
                poolComponent.Count = 100;
                poolComponent.Initialize();

                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newPoolGo, poolsScene);

                poolsInScene.Add(poolComponent);
            }

            FieldInfo[] poolFields = globalPoolContainerType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (poolFields.Length != poolsInScene.Count)
            {
                RegenerateGlobalPoolFields(poolsInScene);
                return;
            }

            bool regenerate = false;

            Component globalPoolComp = globalPoolContainer.GetComponent(globalPoolContainerType);

            for (int i = 0; i < poolsInScene.Count; i++)
            {
                IPoolBehaviour pool = poolsInScene[i];
                FieldInfo fieldForPool = poolFields[i];

                if (fieldForPool.FieldType != pool.GetType())
                {
                    regenerate = true;
                    break;
                }
                else
                {
                    fieldForPool.SetValue(globalPoolComp, pool);
                }
            }

            if (regenerate)
            {
                RegenerateGlobalPoolFields(poolsInScene);
                return;
            }

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(poolsScene);

            if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
            {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(poolsScene, true);
            }

        }

        private static void RegenerateGlobalPoolFields(List<IPoolBehaviour> poolsInScene)
        {
            string oldScript = AssetDatabase.LoadAssetAtPath<TextAsset>(GLOBAL_POOL_TEMPLATE).text;

            List<Tuple<StringExtensions.SECTION_EDGE, int, int>> sections = oldScript.StringReplaceSection(GLOBAL_POOL_START, GLOBAL_POOL_END);

            int padding = 0;

            for (int i = 0; i < sections.Count - 1; i++)
            {
                Tuple<StringExtensions.SECTION_EDGE, int, int> start = sections[i];
                Tuple<StringExtensions.SECTION_EDGE, int, int> end = sections[i + 1];
                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    var replacementText = new StringBuilder();

                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    foreach (IPoolBehaviour pool in poolsInScene)
                    {
                        var templateText = $"public {pool.GetType().Name} {pool.GetType().Name}_{pool.Prefab.name.RemoveWhitespace()};";
                        replacementText
                            .Append('\t')
                            .Append('\t')
                            .Append(templateText)
                            .Append(Environment.NewLine)
                            .Append(Environment.NewLine);
                    }

                    replacementText
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }

            string relativePath = $"{POOL_SCRIPTS_PATH}/GlobalPoolContainer.cs";
            string pathToProject = EditorUtils.PathToProject;

            File.WriteAllText(pathToProject + relativePath, oldScript);
        }

        private static void CreatePoolScene()
        {
            EditorApplication.update -= CreatePoolScene;
            SceneCreatorEditor.CreateNewScene("Assets/Scenes", "Pool");
        }

        #region project wide queries

        /// <summary>
        /// Get all poolable prefabs in the project
        /// </summary>
        /// <returns></returns>
        private static List<Component> GetAllPoolablePrefabs()
        {
            // search for prefabs
            return Resources.LoadAll(string.Empty)
                .Where(o => PrefabUtility.GetPrefabAssetType(o) == PrefabAssetType.Regular)
                .OfType<GameObject>()
                .Cast<GameObject>()
                .Select(m =>
                {
                    foreach (Type v in PoolableTypes)
                    {
                        if (m.TryGetComponent(v, out Component comp))
                            return comp;
                    }

                    return null;
                })
                .Where(m => m != null)
                .ToList();
        }

        /// <summary>
        /// Get the list of all the poolable types in project
        /// </summary>
        /// <returns></returns>
        private static List<Type> GetPoolableTypes()
        {
            List<TextAsset> poolableScripts = EditorUtils.FindTextAssets()
                            .Where(t => !filterFiles.Contains(t.name))
                            .Where(t => t.text.Contains(nameof(GeneratePool)))
                            .ToList();

            List<Type> validTypes = new List<Type>();

            foreach (TextAsset file in poolableScripts)
            {
                string className = file.name;

                Type type = TypeUtils.AllTypes.FirstOrDefault(t => t.Name.Equals(className));

                if (type == null)
                {
                    Debug.LogError($"Couldn't find type named {className} in the assembly");
                    continue;
                }

                if (TypeUtils.IsSubTypeOf(type, typeof(IPoolBehaviour)))
                {
                    // it's a pool type so it should be skipped
                    continue;
                }

                if (!TypeUtils.IsSubTypeOf(type, typeof(MonoBehaviour)))
                {
                    Debug.LogError($"{ type.Name } is not derived from { nameof(MonoBehaviour) }");
                    continue;
                }

                if (type.GetCustomAttributes(typeof(GeneratePool), true).Count() == 0)
                {
                    Debug.LogError($"{ type.Name } doesn't contain the { nameof(GeneratePool) } attribute");
                    continue;
                }

                validTypes.Add(type);
            }

            return validTypes;
        }

        #endregion



    }
}
