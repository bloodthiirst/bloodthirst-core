using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.AdvancedPool.Editor
{
    public class PoolGenerator
    {
        private const string POOL_TEMPLATE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Pool Generator/Template.Pool.cs.txt";
        private const string POOL_SCRIPTS_PATH = "Assets/Scripts/Pools";
        private const string POOL_SCENE_PATH = "Assets/Scenes/PoolScene";
        private const string REPLACE_KEYWORD = "[BEHAVIOUR]";
        private static readonly string[] filterFiles =
        {
            nameof(GeneratePool),
            nameof(PoolGenerator)
        };

        [DidReloadScripts(SingletonScriptableObjectInit.SINGLETONS_CREATION_CHECK)]
        public static void OnDidReloadScripts()
        {
            List<Type> validTypes = GetPoolableTypes();

            if (validTypes.Count == 0)
            {
                return;
            }

            // create folder for the pool scripts
            EditorUtils.CreateFoldersFromPath(POOL_SCRIPTS_PATH);

            bool hasGeneratedScript = false;

            // generate the scripts
            foreach (Type t in validTypes)
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

        /// <summary>
        /// Check the pools scene to see if we need to instantial missing pools
        /// </summary>
        private static void CheckForPoolsInScene()
        {
            EditorApplication.update -= CheckForPoolsInScene;

            List<Type> validTypes = GetPoolableTypes();

            // search for prefabs
            List<Component> allValidPoolablePrefabs = Resources.LoadAll(string.Empty)
                .Where(o => PrefabUtility.GetPrefabAssetType(o) == PrefabAssetType.Regular)
                .OfType<GameObject>()
                .Cast<GameObject>()
                .Select(m =>
                {
                    foreach (Type v in validTypes)
                    {
                        if (m.TryGetComponent(v, out Component comp))
                            return comp;
                    }

                    return null;
                })
                .Where(m => m != null)
                .ToList();

            GetPoolScenePath(out string poolScenePath);

            Scene poolsScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(poolScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);

            // get all pools
            List<IPoolBehaviour> allPools = poolsScene.GetRootGameObjects().Select(go => go.GetComponent<IPoolBehaviour>()).Where(c => c != null).ToList();

            // TODO : generate a pool for each prefab and all it to the "Pools" scene
            foreach (Component poolablePrefab in allValidPoolablePrefabs)
            {
                IPoolBehaviour pool = allPools.FirstOrDefault(p => p.Prefab == poolablePrefab.gameObject);

                // if pool exists , skip
                if (pool != null)
                    continue;

                GameObject newPoolGo = new GameObject($"Auto-Generated Pool [ {poolablePrefab.name} ]");
                Type poolType = TypeUtils.AllTypes.FirstOrDefault(t => t.Name.Equals(poolablePrefab.GetType().Name + "Pool"));

                IPoolBehaviour poolComponent = newPoolGo.AddComponent(poolType) as IPoolBehaviour;
                
                // assign the prefab
                poolComponent.Prefab = poolablePrefab.gameObject;
                poolComponent.Count = 100;
                poolComponent.Initialize();

                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newPoolGo, poolsScene);
            }

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(poolsScene);
            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(poolsScene, true);
        }

        private static void CreatePoolScene()
        {
            EditorApplication.update -= CreatePoolScene;
            SceneCreatorEditor.CreateNewScene("Assets/Scenes", "Pool");
        }

    }
}
