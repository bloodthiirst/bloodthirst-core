using Bloodthirst.BJson;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        static BHotReload()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;

            AssemblyReloadEvents.afterAssemblyReload -= HandleAfterAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
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
        private static List<GameObjectSceneData> SaveSceneData()
        {
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

            return sceneData;
        }

        private static void HandleBeforeAssemblyReload()
        {
            // make sure folder exists
            string folder = Path.GetDirectoryName(HOT_RELOAD_FILE_PATH);
            folder = folder.Replace("\\", "/");
            EditorUtils.CreateFoldersFromPath(folder);


            BHotReloadAssetRef asset = AssetDatabase.LoadAssetAtPath<BHotReloadAssetRef>(HOT_RELOAD_REFS_PATH);

            if (asset == null)
            {
                asset = new BHotReloadAssetRef();
                asset.unityObjs = new List<UnityEngine.Object>();
                
                AssetDatabase.CreateAsset(asset, HOT_RELOAD_REFS_PATH);
            }

            asset.unityObjs.Clear();

            List<GameObjectSceneData> sceneData = SaveSceneData();

            List<IBJsonFilterInternal> customConverters = new List<IBJsonFilterInternal>()
            {
                new BJsonGameObjectSceneDataFilter(),
                new BJsonMonoBehaviourFilter()
            };

            BJsonSettings settings = new BJsonSettings(customConverters)
            {
                Formated = true,
                CustomContext = new UnityObjectContext() { UnityObjects = asset.unityObjs }
            };

            

            string jsonTxt = BJsonConverter.ToJson(sceneData, settings);

            Debug.Log("Jsong Before");
            Debug.Log(jsonTxt);

            string absPath = EditorUtils.RelativeToAbsolutePath(HOT_RELOAD_FILE_PATH);

            byte[] jsonAsByes = Encoding.UTF8.GetBytes(jsonTxt);

            using (FileStream fs = File.Open(absPath, FileMode.OpenOrCreate))
            {
                fs.Position = 0;
                fs.SetLength(jsonAsByes.Length);
                fs.Write(jsonAsByes , 0 , jsonAsByes.Length);
                fs.Close();
            }

            AssetDatabase.ImportAsset(HOT_RELOAD_FILE_PATH, ImportAssetOptions.Default);
            AssetDatabase.SaveAssets();
        }

        private static void HandleAfterAssemblyReload()
        {
            string jsonTxt = string.Empty;
            string absPath = EditorUtils.RelativeToAbsolutePath(HOT_RELOAD_FILE_PATH);

            using (FileStream fs = File.Open(absPath, FileMode.Open))
            using(StreamReader reader = new StreamReader(fs))
            {
                jsonTxt = reader.ReadToEnd();
                reader.Close();
                fs.Close();
            }

            Debug.Log(jsonTxt);


            List<IBJsonFilterInternal> customConverters = new List<IBJsonFilterInternal>()
            {
                new BJsonGameObjectSceneDataFilter(),
                new BJsonMonoBehaviourFilter()
            };

            BHotReloadAssetRef asset = AssetDatabase.LoadAssetAtPath<BHotReloadAssetRef>(HOT_RELOAD_REFS_PATH);

            BJsonSettings settings = new BJsonSettings(customConverters);
            settings.CustomContext = new UnityObjectContext() { UnityObjects = asset.unityObjs };

            List<GameObjectSceneData> inst = new List<GameObjectSceneData>();
            BJsonConverter.PopulateFromJson(inst, jsonTxt, settings);
        }


    }
}
