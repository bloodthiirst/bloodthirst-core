using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CodeGenerator;
using Bloodthirst.Runtime.BAdapter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
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
        private const string POOL_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Pool Generator/Template.Pool.cs.txt";
        private const string POOL_SCRIPTS_PATH = "Assets/Scripts/Pools";
        private const string POOL_SCENE_FOLDER_PATH = "Assets/Scenes/PoolScene";
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

        private static Type GetGlobalPoolContainerType()
        {
            return TypeUtils.AllTypes.FirstOrDefault(p => p.Name.Equals("GlobalPoolContainer"));
        }

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

        [MenuItem("Bloodthirst Tools/AutoGen Pools/Refresh Pools")]
        public static void RefreshPools()
        {
            Create();
        }

        private static void Clean()
        {
            // delete global container + pools
            AssetDatabase.DeleteAsset(POOL_SCRIPTS_PATH);

            // delete pools scene
            AssetDatabase.DeleteAsset(POOL_SCENE_FOLDER_PATH);
        }

        private static bool CanRelink()
        {
            // there are no poolable types that need to be treated
            if (PoolableTypes.Count == 0)
            {
                return false;
            }

            // there are no poolable prfabs that need to be treated
            if (PoolablePrefabs.Count == 0)
            {
                return false;
            }

            if (!HasAllPrefabPoolScripts())
            {
                return false;
            }

            if (!HasGlobalPool())
            {
                return false;
            }

            if (!HasAllPoolFields())
            {
                return false;
            }

            // create it if it's not found
            // return since the SceneManager script generation will trigger domain reload
            if (!HasPoolScene())
            {
                return false;
            }

            return true;
        }

        private static bool HasAllPoolFields()
        {
            Type type = GetGlobalPoolContainerType();

            if (type == null)
            {
                return false;
            }

            // pools fiels
            List<FieldInfo> poolFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

            if (poolFields.Count != PoolablePrefabs.Count)
            {
                return false;
            }

            for (int i = 0; i < poolFields.Count; i++)
            {
                FieldInfo field = poolFields[i];

                Component correctPool = PoolablePrefabs.FirstOrDefault();

                if (correctPool == null)
                {
                    Debug.LogError("=> ERROR not all fiels are generated");
                    return false;
                }
            }

            return true;
        }

        private static void Create()
        {
            EditorApplication.delayCall -= Create;

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

            bool isReadyToLinkReferences = true;

            AssetDatabase.StartAssetEditing();

            // create folder for the pool scripts
            EditorUtils.CreateFoldersFromPath(POOL_SCRIPTS_PATH);

            if (!HasAllPrefabPoolScripts())
            {
                CreateAllPrefabPoolScripts();
                isReadyToLinkReferences = false;
            }

            if (!HasGlobalPool())
            {
                isReadyToLinkReferences = false;
                CreateGlobalPool();
                RegenerateGlobalPoolFields();

            }

            if (!HasAllPoolFields())
            {
                isReadyToLinkReferences = false;
                RegenerateGlobalPoolFields();
            }


            AssetDatabase.StopAssetEditing();

            //create it if it's not found
            // return since the SceneManager script generation will trigger domain reload
            if (!HasPoolScene())
            {
                isReadyToLinkReferences = false;

                CreatePoolScene();
            }


            if (!isReadyToLinkReferences)
            {
                Debug.LogWarning("=> NOT READY to link references");

                // wait for the next reload and recheck again
                EditorApplication.delayCall -= Create;
                EditorApplication.delayCall += Create;
                return;
            }

            Debug.LogWarning("=> READY to link references");

            BootstrapGameObjectsInScene();

            /*
            EditorApplication.update -= CheckForPoolsInScene;
            EditorApplication.update += CheckForPoolsInScene;
            */
        }

        private static bool HasSceneManager()
        {
            // open scene
            Scene poolsScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"{POOL_SCENE_FOLDER_PATH}/PoolScene.unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);

            GameObject manager = poolsScene.GetRootGameObjects().FirstOrDefault(go => go.name.Equals("Scene Manager"));

            return manager != null;
        }

        private static bool HasAllPrefabPoolScripts()
        {
            // get all the scripts in the project
            List<MonoScript> scripts = EditorUtils.FindScriptAssets()
                .Where(p => p != null)
                .Where(p => p.GetClass() != null)
                .ToList();

            // look to see if there's a poolable type that doesnt't have it's pool script generated yet
            for (int i = 0; i < PoolableTypes.Count; i++)
            {
                Type t = PoolableTypes[i];
                string poolName = $"{t.Name}Pool";

                TextAsset poolScript = scripts.FirstOrDefault(p => p.GetClass().Name.Equals(poolName));

                // pool class already exists
                if (poolScript == null)
                    return false;
            }

            return true;
        }

        private static void CreateAllPrefabPoolScripts()
        {
            List<MonoScript> scripts = EditorUtils.FindScriptAssets()
                .Where(s => s != null)
                .Where(s => s.GetClass() != null)
                .ToList();

            // generate the scripts
            foreach (Type t in PoolableTypes)
            {
                TextAsset poolScript = scripts.FirstOrDefault(p => p.GetClass().Name.Equals($"{t.Name}Pool"));

                // pool class already exists
                if (poolScript != null)
                    continue;

                string relativePath = $"{POOL_SCRIPTS_PATH}/{$"{t.Name}Pool.cs"}";
                string absolutePath = EditorUtils.RelativeToAbsolutePath(relativePath);

                TextAsset poolTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(POOL_TEMPLATE);
                string scriptText = poolTemplate
                                .text
                                .Replace(CLASS_NAME_REPLACE_KEYWORD, t.Name)
                                .Replace(CLASS_NAMESPACE_REPLACE_KEYWORD, t.Namespace == null ? string.Empty : $"using {t.Namespace};");

                File.WriteAllText(absolutePath, scriptText);

                AssetDatabase.ImportAsset(relativePath);
            }
        }

        private static void CreateGlobalPool()
        {
            string relativePath = $"{POOL_SCRIPTS_PATH}/GlobalPoolContainer.cs";
            string pathToProject = EditorUtils.PathToProject;

            File.WriteAllText(pathToProject + "/" + relativePath, AssetDatabase.LoadAssetAtPath<TextAsset>(GLOBAL_POOL_TEMPLATE).text);

            AssetDatabase.ImportAsset(relativePath);
        }

        private static bool HasGlobalPool()
        {
            return EditorUtils.FindScriptAssets()
                .Where(s => s != null)
                .Where(s => s.GetClass() != null)
                .FirstOrDefault(p => p.GetClass().Name.Equals("GlobalPoolContainer")) != null;
        }

        /// <summary>
        /// Does the pool scene exist ?
        /// </summary>
        /// <param name="poolScenePath"></param>
        /// <returns></returns>
        private static bool HasPoolScene()
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>($"{POOL_SCENE_FOLDER_PATH}/PoolScene.unity");
        }

        /// <summary>
        /// Check the pools scene to see if we need to instantial missing pools
        /// </summary>
        private static void BootstrapGameObjectsInScene()
        {

            if (!HasPoolScene())
            {
                Debug.LogError("PoolScene not found in the project");
                return;
            }

            Debug.LogWarning("=> LINKING Pool refs");

            // open scene
            Scene poolsScene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath($"{POOL_SCENE_FOLDER_PATH}/PoolScene.unity");
            bool wasOpen = true;

            if (!poolsScene.IsValid())
            {
                wasOpen = false;
                poolsScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"{POOL_SCENE_FOLDER_PATH}/PoolScene.unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }
            // create global pool GO if necessary
            Component globalPoolComp = CreateGlobalPoolGameObject(ref poolsScene);

            // create all pools GOs 
            List<IPoolBehaviour> poolsInScene = CreateSubPoolsGameObjects(ref poolsScene);

            // pools fiels
            List<FieldInfo> poolFields = GetGlobalPoolContainerType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

            if (poolFields.Count != poolsInScene.Count)
            {
                Debug.LogError("=> ERROR the number of fields isn't equal to the number of pools available");
                return;
            }

            AssignPoolsToFields(poolFields, poolsInScene, globalPoolComp);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(poolsScene);

            if (!wasOpen)
            {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(poolsScene, true);
            }

            Debug.Log("=> SUCESS AutoGen-Pool setup successfully !");

        }

        private static void AssignPoolsToFields(List<FieldInfo> poolFields, List<IPoolBehaviour> poolsInScene, Component globalPoolComp)
        {
            for (int i = 0; i < poolFields.Count; i++)
            {
                FieldInfo field = poolFields[i];

                IPoolBehaviour correctPool = poolsInScene.FirstOrDefault(p => field.Name.EndsWith(TextInfo.ToTitleCase(p.Prefab.gameObject.name.RemoveWhitespace())));

                if (correctPool == null)
                {
                    Debug.LogError("=> ERROR couldn't link pool field to the correct pool GO");
                    return;
                }

                field.SetValue(globalPoolComp, correctPool);
            }
        }

        private static List<IPoolBehaviour> CreateSubPoolsGameObjects(ref Scene poolsScene)
        {
            List<IPoolBehaviour> allPools = poolsScene
                            .GetRootGameObjects()
                            .Select(go => go.GetComponent<IPoolBehaviour>())
                            .Where(c => c != null).ToList();

            List<IPoolBehaviour> poolsInScene = new List<IPoolBehaviour>();

            // generate a pool for each prefab and add all of of the pools to the "Pools" scene
            foreach (Component poolablePrefab in PoolablePrefabs)
            {
                IPoolBehaviour poolComponent = allPools.FirstOrDefault(p => p.Prefab == poolablePrefab.gameObject);

                // if pool doesn't exists , create
                if (poolComponent == null)
                {
                    GameObject newPoolGo = new GameObject($"Auto-Generated Pool [ {poolablePrefab.name} ]");

                    Type poolType = TypeUtils.AllTypes.FirstOrDefault(t => t.Name.Equals(poolablePrefab.GetType().Name + "Pool"));


                    // an adapter behaviour we add to separate the pool funtionality from the scne initialization
                    PoolAdapter adapter = newPoolGo.AddComponent<PoolAdapter>();

                    // add the pool behaviour
                    poolComponent = newPoolGo.AddComponent(poolType) as IPoolBehaviour;
                }             

                // assign the prefab
                poolComponent.Prefab = poolablePrefab.gameObject;

                // we need to eliminate everyting before the resource folder
                string resPath = EditorUtils.GetResourcesPath(poolablePrefab);

                poolComponent.PrefabPath = resPath;
                poolComponent.Count = 100;
                poolComponent.Initialize();

                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(((Component)poolComponent).gameObject, poolsScene);

                poolsInScene.Add(poolComponent);
            }

            return poolsInScene;
        }

        private static Component CreateGlobalPoolGameObject(ref Scene poolsScene)
        {
            Type type = GetGlobalPoolContainerType();

            // get or create global pool container
            GameObject globalPoolGO = poolsScene
                .GetRootGameObjects()
                .FirstOrDefault(go => go.GetComponent(type) != null);

            if (globalPoolGO == null)
            {
                globalPoolGO = new GameObject(type.Name);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(globalPoolGO, poolsScene);
                Component comp = globalPoolGO.AddComponent(type);

                return comp;
            }

            return globalPoolGO.GetComponent(type);
        }

        /// <summary>
        /// Generates the appropriate name for a pool
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        private static string PrefabToFieldName(Component prefab)
        {
            return $"{prefab.GetType().Name}PoolFor{TextInfo.ToTitleCase(prefab.gameObject.name.RemoveWhitespace())}";
        }

        /// <summary>
        /// Generates the appropriate name for a pool
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        private static string PrefabToFieldGenerator(Component prefab)
        {
            return $"{prefab.GetType().Name}PoolFor{TextInfo.ToTitleCase(prefab.gameObject.name.RemoveWhitespace())}";
        }

        private static void RegenerateGlobalPoolFields()
        {
            string globalPoolTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(GLOBAL_POOL_TEMPLATE).text;
            string globalPoolFieldTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(GLOBAL_POOL_FIELD_TEMPLATE).text;

            TemplateCodeBuilder scriptBuilder = new TemplateCodeBuilder(globalPoolTemplate);

            // write fields
            {
                ITextSection poolFieldsWriter = scriptBuilder
                    .CreateSection(new StartEndTextSection(GLOBAL_POOL_START, GLOBAL_POOL_END));
                poolFieldsWriter
                    .AddWriter(new ReplaceAllTextWriter(string.Empty))
                    .AddWriter(new AppendTextWriter(Environment.NewLine));

                foreach (Component prefab in PoolablePrefabs)
                {
                    TemplateCodeBuilder fieldBuilder = new TemplateCodeBuilder(globalPoolFieldTemplate);
                    fieldBuilder
                        .CreateSection(new EntireTextSection())
                        .AddWriter(new ReplaceTermTextWriter(PREFAB_FIELD_NAME_TERM, PrefabToFieldName(prefab)))
                        .AddWriter(new ReplaceTermTextWriter(PREFAB_TYPE_TERM, prefab.GetType().Name))
                        .AddWriter(new ReplaceTermTextWriter(PREFAB_NAME_TERM, prefab.name));

                    string poolFieldText = fieldBuilder.Build();

                    poolFieldsWriter.AddWriter(new AppendTextWriter(poolFieldText));
                }

                poolFieldsWriter
                    .AddWriter(new AppendTextWriter(Environment.NewLine))
                    .AddWriter(new AppendTextWriter("\t\t"));
            }

            // write initialization
            {
                ITextSection poolIntializationSection = scriptBuilder
                    .CreateSection(new StartEndTextSection(GLOBAL_POOL_LIST_START, GLOBAL_POOL_LIST_END));
                poolIntializationSection
                    .AddWriter(new ReplaceAllTextWriter(string.Empty))
                    .AddWriter(new AppendTextWriter(Environment.NewLine));

                foreach (Component prefab in PoolablePrefabs)
                {
                    poolIntializationSection.AddWriter(new AppendTextWriter($"AllPools.Add({PrefabToFieldName(prefab)});"));
                }

                poolIntializationSection
                    .AddWriter(new AppendTextWriter(Environment.NewLine))
                    .AddWriter(new AppendTextWriter("\t\t"));
            }

            string newScriptText = scriptBuilder.Build();

            string absolutePath = EditorUtils.RelativeToAbsolutePath( $"{POOL_SCRIPTS_PATH}/GlobalPoolContainer.cs");

            File.WriteAllText(absolutePath, newScriptText);
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

#endregion



    }
}
