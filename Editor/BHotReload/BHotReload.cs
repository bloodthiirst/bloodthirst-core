using Bloodthirst.BJson;
using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BHotReload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Editor.BHotReload
{
    [InitializeOnLoad]
    public static class BHotReload
    {
        private static string HOT_RELOAD_FILE_PATH = "Assets/HotReload/HotReloadData.txt";
        private static string HOT_RELOAD_REFS_PATH = "Assets/HotReload/HotReloadRefs.asset";
        private static string HOT_RELOAD_SETTINGS_PATH = "Assets/HotReload/HotReloadSettings.asset";

        static BHotReload()
        {
            EditorApplication.playModeStateChanged -= HandleEnterPlayMode;
            EditorApplication.playModeStateChanged += HandleEnterPlayMode;

            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;

            AssemblyReloadEvents.afterAssemblyReload -= HandleAfterAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
        }

        private static void HandleEnterPlayMode(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
                return;

            List<Type> staticTypes = TypeUtils.AllTypes.Where(t => t.GetCustomAttribute<BHotReloadStatic>() != null).ToList();

            foreach (Type type in staticTypes)
            {
                FieldInfo[] staticFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                PropertyInfo[] staticProps = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (FieldInfo field in staticFields)
                {
                    Func<object> defaultValue = ReflectionUtils.GetDefaultValue(field.FieldType);
                    object val = defaultValue();
                    field.SetValue(null, val);
                }

                foreach (PropertyInfo property in staticProps)
                {
                    Func<object> defaultValue = ReflectionUtils.GetDefaultValue(property.PropertyType);
                    object val = defaultValue();
                    property.SetValue(null, val);
                }
            }
        }
        private static void CreateFolder()
        {
            EditorUtils.CreateFoldersFromPath(HOT_RELOAD_FILE_PATH);
        }

        private static void RecursivelyQueryComponents(GameObject go, List<int> currentIndices, List<GameObjectSceneData> resultList)
        {
            Component[] allComponents = go.GetComponents<Component>();

            // save the components
            for (int i = 0; i < allComponents.Length; i++)
            {
                Component compValue = allComponents[i];

                GameObjectSceneData sceneData = new GameObjectSceneData()
                {
                    ScenePath = go.scene.path,
                    GameObjectName = go.name,
                    SceneGameObjectIndex = currentIndices.ToList(),
                    ComponentIndex = i,
                    ComponentType = compValue.GetType(),
                    ComponentValue = compValue
                };

                resultList.Add(sceneData);
            }

            // recurse into the child objects
            for (int c = 0; c < go.transform.childCount; c++)
            {
                GameObject child = go.transform.GetChild(c).gameObject;
                List<int> newIndicies = currentIndices.ToList();
                newIndicies.Add(c);

                RecursivelyQueryComponents(child, newIndicies, resultList);
            }
        }
        private static GameStateSnapshot CreateGameStateSnapshot()
        {
            GameStateSnapshot snapshot = new GameStateSnapshot();

            // scene objs
            List<GameObject> rootGOs = new List<GameObject>();

            List<GameObjectSceneData> sceneData = new List<GameObjectSceneData>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene currScene = SceneManager.GetSceneAt(i);

                rootGOs.Clear();
                currScene.GetRootGameObjects(rootGOs);

                for (int j = 0; j < rootGOs.Count; j++)
                {
                    RecursivelyQueryComponents(rootGOs[j], new List<int>() { j }, sceneData);
                }
            }

            // static data
            List<Type> staticTypes = TypeUtils.AllTypes.Where(t => t.GetCustomAttribute<BHotReloadStatic>() != null).ToList();

            List<TypeStaticData> staticData = new List<TypeStaticData>();

            foreach (Type type in staticTypes)
            {
                TypeStaticData data = new TypeStaticData();
                data.TypeReference = type;
                data.StaticFields = new Dictionary<string, object>();
                data.StaticProperties = new Dictionary<string, object>();

                FieldInfo[] staticFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                PropertyInfo[] staticProps = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (FieldInfo field in staticFields)
                {
                    data.StaticFields.Add(field.Name, field.GetValue(null));
                }

                foreach (PropertyInfo property in staticProps)
                {
                    data.StaticProperties.Add(property.Name, property.GetValue(null));
                }

                staticData.Add(data);
            }

            snapshot.SceneObjects = sceneData;
            snapshot.StaticDatas = staticData;

            /// static refs
            return snapshot;
        }
        private static BJsonSettings GetSerializationSettings()
        {
            BHotReloadAssetRef asset = GetOrCreateRefsAsset();

            List<IBJsonFilterInternal> customConverters = new List<IBJsonFilterInternal>()
            {
                new BJsonGameObjectSceneDataFilter(),
                new BJsonRootMonoBehaviourFilter()
            };

            Dictionary<string, GameObject[]> allScenesObjects = new Dictionary<string, GameObject[]>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene curr = SceneManager.GetSceneAt(i);

                allScenesObjects.Add(curr.path, curr.GetRootGameObjects());
            }
            UnityObjectContext ctx = new UnityObjectContext()
            {
                UnityObjects = asset.unityObjs,
                allSceneObjects = allScenesObjects
            };

            BJsonSettings settings = new BJsonSettings(customConverters)
            {
                Formated = true,
                CustomContext = ctx
            };

            return settings;
        }
        private static BHotReloadAssetRef GetOrCreateRefsAsset()
        {
            // make sure the asset exists
            BHotReloadAssetRef asset = AssetDatabase.LoadAssetAtPath<BHotReloadAssetRef>(HOT_RELOAD_REFS_PATH);

            if (asset == null)
            {
                CreateFolder();

                asset = ScriptableObject.CreateInstance<BHotReloadAssetRef>();
                asset.unityObjs = new List<UnityEngine.Object>();

                AssetDatabase.CreateAsset(asset, HOT_RELOAD_REFS_PATH);
            }

            return asset;
        }
        private static BHotReloadSettings GetOrCreateSettingsAsset()
        {
            // make sure the asset exists
            BHotReloadSettings asset = AssetDatabase.LoadAssetAtPath<BHotReloadSettings>(HOT_RELOAD_SETTINGS_PATH);

            if (asset == null)
            {
                CreateFolder();

                asset = ScriptableObject.CreateInstance<BHotReloadSettings>();

                AssetDatabase.CreateAsset(asset, HOT_RELOAD_SETTINGS_PATH);
            }

            return asset;
        }

        private static FileStream GetJsonFile(FileMode fileMode, FileAccess fileAccess)
        {
            // makes sure the txt file exists
            string absPath = EditorUtils.RelativeToAbsolutePath(HOT_RELOAD_FILE_PATH);

            if (!File.Exists(absPath))
            {
                CreateFolder();
                FileStream create_fs = File.Create(absPath);
                create_fs.Dispose();
            }

            return File.Open(absPath, fileMode, fileAccess);
        }

        private static bool WillEnterPlayMode()
        {
            return EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying;
        }

        private static bool TriggerHotReload()
        {
            BHotReloadSettings settings = GetOrCreateSettingsAsset();

            if (!settings.enabled)
                return false;

            if (settings.debugMode)
                return true;

            if (WillEnterPlayMode())
                return false;

            return true;
        }

        private static void HandleBeforeAssemblyReload()
        {
            if (!TriggerHotReload())
                return;

            BJsonSettings settings = GetSerializationSettings();

            GameStateSnapshot snapshot = CreateGameStateSnapshot();

            string jsonTxt = BJsonConverter.ToJson(snapshot, settings);

            Debug.Log("Json Before");
            Debug.Log(jsonTxt);

            byte[] jsonAsByes = Encoding.UTF8.GetBytes(jsonTxt);

            // makes sure the txt file exists
            using (FileStream fs = GetJsonFile(FileMode.Open, FileAccess.Write))
            {
                fs.Position = 0;
                fs.SetLength(jsonAsByes.Length);
                fs.Write(jsonAsByes, 0, jsonAsByes.Length);
            }

            AssetDatabase.ImportAsset(HOT_RELOAD_FILE_PATH, ImportAssetOptions.ForceUpdate);
        }

        private static void HandleAfterAssemblyReload()
        {
            if (!TriggerHotReload())
                return;

            string jsonTxt = string.Empty;

            using (FileStream fs = GetJsonFile(FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs))
            {
                jsonTxt = reader.ReadToEnd();
            }

            Debug.Log("Json After");
            Debug.Log(jsonTxt);

            BJsonSettings settings = GetSerializationSettings();

            GameStateSnapshot snapshot = new GameStateSnapshot()
            {
                SceneObjects = new List<GameObjectSceneData>()
            };

            BJsonConverter.PopulateFromJson(snapshot, jsonTxt, settings);

            
            // assing static values
            foreach (TypeStaticData staticData in snapshot.StaticDatas)
            {
                foreach (KeyValuePair<string, object> kv in staticData.StaticFields)
                {
                    staticData.TypeReference
                        .GetField(kv.Key, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .SetValue(null, kv.Value);
                }


                foreach (KeyValuePair<string, object> kv in staticData.StaticProperties)
                {
                    staticData.TypeReference
                        .GetProperty(kv.Key, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .SetValue(null, kv.Value);
                }
            }

        }

    }
}
