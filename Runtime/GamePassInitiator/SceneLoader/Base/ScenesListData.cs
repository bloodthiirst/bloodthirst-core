using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public class ScenesListData : SingletonScriptableObject<ScenesListData>
    {
        [SerializeField]
        [HideInInspector]
        private List<string> scenesList;

        [OdinSerialize]
        [ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true)]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ReorderList))]
#endif
        [InfoBox("The first scene MUST contain a SceneLoadingManager", nameof(HasSceneLoadingManager), InfoMessageType = InfoMessageType.Error, VisibleIf = nameof(HasSceneLoadingManager))]
        private List<ReadOnlyString> editorList;

        [SerializeField]
        [HideInInspector]
        private bool isValidSetup;

        private bool HasSceneLoadingManager()
        {
            return !isValidSetup;
        }

        public List<string> ScenesList
        {
            get
            {
                if (scenesList == null)
                {
                    scenesList = new List<string>();
                }
                return scenesList;
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

        public static void ReloadUpdater()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
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
        public void ApplyToBuildSettings()
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

            for (int i = 0; i < scenesList.Count; i++)
            {
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenesList[i], true));
            }

            // apply the change to the scenes list in build settings
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }

        private void SceneToEditorList()
        {
            editorList = CollectionsUtils.CreateOrClear(editorList);

            for (int i = 0; i < scenesList.Count; i++)
            {
                editorList.Add(scenesList[i]);
            }
        }

        private void EditorToSceneList()
        {
            scenesList = CollectionsUtils.CreateOrClear(scenesList);

            for (int i = 0; i < editorList.Count; i++)
            {
                scenesList.Add(editorList[i]);
            }
        }

        private void ReorderList()
        {
            EditorToSceneList();

            ApplyToBuildSettings();

            RefreshSceneManagerIndices();
        }

        [Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
        public void LoadAllScenesAvailable()
        {
            // copy old scenes
            List<string> oldList = ScenesList.ToList();

            ScenesList.Clear();

            List<EditorBuildSettingsScene> editorBuildSettingsScenes = GetAllSceneInTheProjectWithManager();

            // try to keep the same order of the old list
            for (int i = 0; i < editorBuildSettingsScenes.Count; i++)
            {
                var curr = editorBuildSettingsScenes[i];

                var oldPath = curr.path;
                var sameScene = oldList.FirstOrDefault(s => s.Equals(oldPath));

                if (sameScene != null)
                {
                    int indexInOldSave = oldList.IndexOf(oldPath);
                    oldList.RemoveAt(indexInOldSave);
                    editorBuildSettingsScenes.RemoveAt(i);
                    editorBuildSettingsScenes.Insert(indexInOldSave, curr);
                }
            }

            // apply the change to the scenes list in build settings
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

            // save the scenes info in dictionary
            for (int i = 0; i < editorBuildSettingsScenes.Count; i++)
            {
                string sceneName = editorBuildSettingsScenes[i].path;

                // add to the final list
                ScenesList.Add(sceneName);

                // setup scene manager index
                SetupSceneInstanceManager(i);
            }

            SceneToEditorList();
        }

        private void RefreshSceneManagerIndices()
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

            for (int i = 0; i < ScenesList.Count; i++)
            {
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(ScenesList[i], true));
            }

            // save the scenes info in dictionary
            for (int i = 0; i < editorBuildSettingsScenes.Count; i++)
            {
                // setup scene manager index
                SetupSceneInstanceManager(i);
            }
        }

        private List<EditorBuildSettingsScene> GetAllSceneInTheProjectWithManager()
        {
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

            return editorBuildSettingsScenes;
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

            sceneRef = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);

            if (!sceneRef.IsValid())
            {
                cleanupScene = true;

                //AssemblyReloadEvents.

                if (canOpenScene)
                {
                    sceneRef = EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(sceneIndex), OpenSceneMode.Additive);
                }
            }

            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);

            if (!scene.IsValid())
            {
                return;
            }

            if (sceneIndex == 0)
            {
                LoadingManager sceneLoadingManager = scene
                    .GetRootGameObjects()
                    .FirstOrDefault(go => go.GetComponent<LoadingManager>() != null)?
                    .GetComponent<LoadingManager>();

                if (sceneLoadingManager != null)
                {
                    isValidSetup = true;
                }
                else
                {
                    isValidSetup = false;
                }
            }



            ISceneInstanceManager sceneInstanceManager = scene
                .GetRootGameObjects()
                .FirstOrDefault(go => go.GetComponent<ISceneInstanceManager>() != null)?
                .GetComponent<ISceneInstanceManager>();

            if (sceneInstanceManager != null)
            {
                sceneInstanceManager.SceneIndex = sceneIndex;
                sceneInstanceManager.ScenePath = scene.path;
                EditorSceneManager.SaveScene(sceneRef);
            }
            if (cleanupScene)
            {
                EditorSceneManager.CloseScene(sceneRef, true);
            }
        }
#endif
    }
}
