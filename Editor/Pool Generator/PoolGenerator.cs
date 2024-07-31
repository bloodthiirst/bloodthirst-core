using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Runtime.BAdapter;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.AdvancedPool.Editor
{
    /// <summary>
    /// <para> This class auto-generates pools of the prefabs marked with the <see cref="GeneratePool"></see></para>
    /// <para> The pools are created in the "PoolScene" , a scene that is specifically made to contain thses auto-generated pools</para>
    /// </summary>
    public class PoolGenerator
    {

        #region global pool container
        private const string GLOBAL_POOL_FIELD_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Pool Generator/Template.PoolField.cs.txt";

        private const string PREFAB_NAME_TERM = "[PREFAB_NAME]";
        private const string PREFAB_TYPE_TERM = "[PREFAB_TYPE]";
        private const string PREFAB_FIELD_NAME_TERM = "[PREFAB_FIELD_NAME]";

        private const string GLOBAL_POOL_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Pool Generator/GlobalPoolContainer.cs.txt";

        private const string GLOBAL_POOL_START = "// [START_POOLS]";
        private const string GLOBAL_POOL_END = "// [END_POOLS]";
        #endregion

        #region auto-gen pools
        private const string POOL_SCENE_FOLDER_PATH = "Assets/Scenes/Pool";
        private const string POOL_SCENE_ASSET_PATH = POOL_SCENE_FOLDER_PATH + "/Pool.unity";
        private const string CLASS_NAME_REPLACE_KEYWORD = "[BEHAVIOUR]";
        private const string CLASS_NAMESPACE_REPLACE_KEYWORD = "[NAMESPACE]";
        private const string GLOBAL_POOL_LIST_START = "// [LOAD_IN_LIST_START]";
        private const string GLOBAL_POOL_LIST_END = "// [LOAD_IN_LIST_END]";
        #endregion

        private static readonly string[] filterFiles =
        {
            "GeneratePool",
            nameof(PoolGenerator),
            "Template.Pool.cs",
            "GlobalPoolContainer.cs",
            "GlobalPoolContainer"
        };

        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;


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

        [MenuItem("Bloodthirst Tools/AutoGen Pools/Rebuild Pools")]
        public static void RebuildPools()
        {
            Clean();

            Create();
        }

        [MenuItem("Bloodthirst Tools/AutoGen Pools/Fill Pools Data")]
        public static void FillPrefabPools()
        {
            QueryPrefabs();
        }


        [MenuItem("Bloodthirst Tools/AutoGen Pools/Refresh Pools")]
        public static void RefreshPools()
        {
            Create();
        }

        private static void Clean()
        {
            // delete pools scene
            AssetDatabase.DeleteAsset(POOL_SCENE_FOLDER_PATH);
        }

        private static void Create()
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

            //create it if it's not found
            // return since the SceneManager script generation will trigger domain reload
            if (!HasPoolScene())
            {
                EditorTasks.Add(CreatePoolScene);
            }

            EditorTasks.Add(BootstrapGameObjectsInScene);
        }

        private static bool HasSceneManager()
        {
            // open scene
            Scene poolsScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(POOL_SCENE_ASSET_PATH, UnityEditor.SceneManagement.OpenSceneMode.Additive);

            GameObject manager = poolsScene.GetRootGameObjects().FirstOrDefault(go => go.name.Equals("Scene Manager"));

            return manager != null;
        }

        /// <summary>
        /// Does the pool scene exist ?
        /// </summary>
        /// <param name="poolScenePath"></param>
        /// <returns></returns>
        private static bool HasPoolScene()
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(POOL_SCENE_ASSET_PATH);
        }

        /// <summary>
        /// Check the pools scene to see if we need to instantial missing pools
        /// </summary>
        private static void BootstrapGameObjectsInScene()
        {
            if (!HasPoolScene())
            {
                Debug.LogError("Pool.unity not found in the project");
                return;
            }

            Debug.LogWarning("=> LINKING Pool refs");

            // open scene
            Scene poolsScene = SceneManager.GetSceneByPath(POOL_SCENE_ASSET_PATH);
            bool wasOpen = true;

            if (!poolsScene.IsValid())
            {
                wasOpen = false;
                poolsScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(POOL_SCENE_ASSET_PATH, UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }

            // create global pool GO if necessary
            GlobalPoolContainer globalPoolComp = CreateGlobalPoolGameObject(ref poolsScene);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(poolsScene);

            if (!wasOpen)
            {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(poolsScene, true);
            }

            Debug.Log("=> SUCESS AutoGen-Pool setup successfully !");
        }

        private static GlobalPoolContainer CreateGlobalPoolGameObject(ref Scene poolsScene)
        {
            Type adapterType = TypeUtils.AllTypes
                .Where(t => t.GetCustomAttribute(typeof(BAdapterForAttribute)) != null)
                .FirstOrDefault(t => t.GetCustomAttribute<BAdapterForAttribute>().AdapterForType == typeof(IGlobalPool));

            Assert.IsNotNull(adapterType);

            // get or create global pool container
            GameObject globalPoolGO = poolsScene
                .GetRootGameObjects()
                .FirstOrDefault(go => go.TryGetComponent(out GlobalPoolContainer _));

            GlobalPoolContainer globalPool = null;

            if (globalPoolGO == null)
            {
                globalPoolGO = new GameObject(nameof(GlobalPoolContainer));
                SceneManager.MoveGameObjectToScene(globalPoolGO, poolsScene);

                globalPoolGO.AddComponent(adapterType);
                globalPool = globalPoolGO.AddComponent<GlobalPoolContainer>();
            }
            else
            {
                globalPool = globalPoolGO.GetComponent<GlobalPoolContainer>();
            }

            Assert.IsNotNull(globalPool);

            GlobalPoolData poolData = EditorUtils.FindAssets<GlobalPoolData>(Array.Empty<string>()).FirstOrDefault();
            Assert.IsNotNull(poolData);

            globalPool.PoolData = poolData;

            return globalPool;
        }

        private static void CreatePoolScene()
        {
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
            List<GameObject> allPrefabs = EditorUtils.FindAssetsAs<GameObject>("t:prefab");
            List<Component> lst = new List<Component>(allPrefabs.Count);

            for (int i = 0; i < allPrefabs.Count; i++)
            {
                GameObject currPrefab = allPrefabs[i];

                foreach (Type poolableType in PoolableTypes)
                {
                    if (currPrefab.TryGetComponent(poolableType, out Component comp))
                    {
                        lst.Add(comp);
                        break;
                    }
                }
            }

            return lst;
        }

        /// <summary>
        /// Get the list of all the poolable types in project
        /// </summary>
        /// <returns></returns>
        private static List<Type> GetPoolableTypes()
        {
            List<MonoScript> poolableScripts = EditorUtils.FindScriptAssets()
                            .Where(t => !filterFiles.Contains(t.name))
                            .Where(t => t.GetClass() != null)
                            .Where(t => t.GetClass().GetCustomAttribute(typeof(GeneratePool)) != null)
                            .ToList();

            List<Type> validTypes = new List<Type>();

            foreach (MonoScript file in poolableScripts)
            {
                Type type = file.GetClass();

                if (type == null)
                {
                    Debug.LogError($"Couldn't find type named {TypeUtils.GetNiceName(type)} in the assembly");
                    continue;
                }

                if (TypeUtils.IsSubTypeOf(type, typeof(IPoolBehaviour)))
                {
                    // it's a pool type so it should be skipped
                    continue;
                }

                if (!TypeUtils.IsSubTypeOf(type, typeof(MonoBehaviour)))
                {
                    Debug.LogError($"{type.Name} is not derived from {nameof(MonoBehaviour)}");
                    continue;
                }

                if (type.GetCustomAttributes(typeof(GeneratePool), true).Count() == 0)
                {
                    Debug.LogError($"{type.Name} doesn't contain the {nameof(GeneratePool)} attribute");
                    continue;
                }

                validTypes.Add(type);
            }

            return validTypes;
        }


        private static void QueryPrefabs()
        {
            GlobalPoolData poolData = EditorUtils.FindAssets<GlobalPoolData>(Array.Empty<string>()).FirstOrDefault();
            Assert.IsNotNull(poolData);

            foreach (Component p in PoolablePrefabs)
            {
                poolData.Add(p.gameObject, 50);
            }

            // cleanup "null" entries
            poolData.Remove(null);

            EditorUtility.SetDirty(poolData);
            AssetDatabase.SaveAssetIfDirty(poolData);

            Selection.activeObject = poolData;
            EditorGUIUtility.PingObject(poolData);
        }
    }

    #endregion
}

