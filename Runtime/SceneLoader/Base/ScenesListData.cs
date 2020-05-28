using Bloodthirst.Core.PersistantAsset;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public class ScenesListData : SingletonScriptableObject<ScenesListData>
    {
        [OdinSerialize]
        [ReadOnly]
        private Dictionary<int, string> scenesDictionary;

        [SerializeField]
        private string InitSceneName = default;
  
        public Dictionary<int, string> ScenesDictionary
        {
            get
            {
                if (scenesDictionary == null)
                {
                    scenesDictionary = new Dictionary<int, string>();
                }
                return scenesDictionary;
            }
        }

        public int callbackOrder => 0;

#if UNITY_EDITOR

        private static bool canOpenScene;

        private static void OnAfterAssemblyReload()
        {
            canOpenScene = true;
        }

        private static void OnBeforeAssemblyReload()
        {
            canOpenScene = false;
        }

        [DidReloadScripts(SingletonScriptableObjectInit.TRACK_ASSEMBLY_RELOAD)]
        public static void ReloadUpdater()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private bool SceneHasManager(string sceneName)
        {

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.Name.Equals(sceneName + "Manager"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        [Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
        public void LoadAllScenesAvailable()
        {

            ScenesDictionary.Clear();

            // load scene assets
            string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");

            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

            foreach (string sceneGUID in scenesGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);

                // has manager

                string sceneName = scenePath.Remove(scenePath.Length - 6).Split('/').Last();

                if (!SceneHasManager(sceneName))
                {
                    continue;
                }

                // is in scene folder

                if (!scenePath.StartsWith("Assets/Scenes"))
                    continue;

                if (!string.IsNullOrEmpty(scenePath))
                {
                    Debug.Log("Scene path found : " + scenePath);

                    // if scene is valid add it to the scenes list in build settings
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }

            editorBuildSettingsScenes = editorBuildSettingsScenes.OrderBy(s => !s.path.Split('/').Last().Equals(InitSceneName + ".unity")).ToList();

            // apply the change to the scenes list in build settings
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();



            // save the scenes info in dictionary
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string sceneName = EditorBuildSettings.scenes[i].path;
                sceneName = sceneName.Remove(sceneName.Length - 6).Split('/').Last();

                int buildIndex = SceneUtility.GetBuildIndexByScenePath(EditorBuildSettings.scenes[i].path);

                ScenesDictionary.Add(buildIndex , sceneName);
                SetupSceneInstanceManager(i);
            }

            Dictionary<int, string> tmp = new Dictionary<int, string>();

            IOrderedEnumerable<KeyValuePair<int, string>> ordered = ScenesDictionary.OrderBy(kv => kv.Key);

            foreach (KeyValuePair<int, string> kv in ordered)
            {
                tmp.Add(kv.Key, kv.Value);
            }

            scenesDictionary = new Dictionary<int, string>(tmp);
        }

#if UNITY_EDITOR
        public void OnPreprocessBuild(BuildReport report)
        {
            LoadAllScenesAvailable();
        }
#endif

        private void SetupSceneInstanceManager(int sceneIndex)
        {
            bool cleanupScene = false;

            Scene sceneRef = default;

            if(!UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
            {
                cleanupScene = true;

                //AssemblyReloadEvents.

                if (canOpenScene)
                {
                    sceneRef = EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(sceneIndex), OpenSceneMode.Additive);
                }
            }

            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);

            if(!scene.IsValid())
            {
                return;
            }

            ISceneInstanceManager sceneInstanceManager = scene
                .GetRootGameObjects()
                .FirstOrDefault(go => go.GetComponent<ISceneInstanceManager>() != null)?
                .GetComponent<ISceneInstanceManager>();

            if (sceneInstanceManager != null)
            {
                sceneInstanceManager.SceneIndex = sceneIndex;
                EditorSceneManager.SaveScene(sceneRef);
            }
            if (cleanupScene)
            {
                EditorSceneManager.CloseScene(sceneRef, true );
            }
        }
#endif
    }
}
