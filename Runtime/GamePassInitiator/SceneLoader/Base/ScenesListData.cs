using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public class ScenesListData : SingletonScriptableObject<ScenesListData>
    {
        [SerializeField]
        [HideInInspector]
        private List<string> scenesList = new List<string>();

        public IReadOnlyList<string> ScenesList => scenesList;

#if ODIN_INSPECTOR
        [OdinSerialize]
        [ListDrawerSettings(ShowFoldout = false, AlwaysAddDefaultValue = true)]
        [InfoBox("The first scene MUST contain a " + nameof(LoadingManager), nameof(HasSceneLoadingManager), InfoMessageType = InfoMessageType.Error, VisibleIf = nameof(HasSceneLoadingManager))]
        [InfoBox("The scene setup seems to be correct", nameof(IsValid), InfoMessageType = InfoMessageType.Info, VisibleIf = nameof(IsValid))]
#endif
#if UNITY_EDITOR && ODIN_INSPECTOR
        [OnValueChanged(nameof(ReorderList))]
#endif

        private List<ReadOnlyString> editorList;

        [SerializeField]
        [HideInInspector]
        private bool isValidSetup;

        private bool HasSceneLoadingManager()
        {
            return !isValidSetup;
        }

        private bool IsValid()
        {
            return isValidSetup;
        }

        public int callbackOrder => 0;

#if UNITY_EDITOR

        private bool SceneHasManager(string sceneName)
        {
            string managerTypename = sceneName + "SceneManager";

            for (int i = 0; i < TypeUtils.AllTypes.Count; i++)
            {
                Type t = TypeUtils.AllTypes[i];

                if (t.Name.Equals(managerTypename, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

            }

            return false;
        }
        private void GetAllSceneInTheProjectWithManager(List<SceneAsset> sceneAssets)
        {
            // load scene assets
            string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");

            for (int i = 0; i < scenesGUIDs.Length; i++)
            {
                string sceneGUID = scenesGUIDs[i];
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

                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    sceneAssets.Add(sceneAsset);
                }
            }

        }

#if ODIN_INSPECTOR
        [Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
#else
        [ContextMenu(nameof(ApplyToBuildSettings))]
#endif
        public void ApplyToBuildSettings()
        {
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[scenesList.Count];

            for (int i = 0; i < scenesList.Count; i++)
            {
                editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenesList[i], true);
            }

            // apply the change to the scenes list in build settings
            EditorBuildSettings.scenes = editorBuildSettingsScenes;
        }

        private void SceneListToEditor()
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

            RefreshSceneManagerIndices();

            ApplyToBuildSettings();

        }

#if ODIN_INSPECTOR
        [Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
#else
        [ContextMenu(nameof(InitializeScenes))]
#endif
        public void InitializeScenes()
        {
            RefreshScenesList();

            RefreshSceneManagerIndices();

            ApplyToBuildSettings();

            SceneListToEditor();
        }

        private void RefreshScenesList()
        {
            using (ListPool<SceneAsset>.Get(out List<SceneAsset> allScenesWithManager))
            {
                GetAllSceneInTheProjectWithManager(allScenesWithManager);

                EditorBuildSettingsScene[] editorScenes = new EditorBuildSettingsScene[allScenesWithManager.Count];

                // try to keep the same order of the old list
                for (int i = 0; i < allScenesWithManager.Count; i++)
                {
                    SceneAsset curr = allScenesWithManager[i];

                    string scenePath = AssetDatabase.GetAssetPath(curr);

                    editorScenes[i] = new EditorBuildSettingsScene(scenePath, true);
                }

                scenesList.Clear();

                // save the scenes info in dictionary
                for (int i = 0; i < allScenesWithManager.Count; i++)
                {
                    SceneAsset curr = allScenesWithManager[i];

                    string scenePath = AssetDatabase.GetAssetPath(curr);

                    // add to the final list
                    scenesList.Add(scenePath);
                }
            }
        }
        private void RefreshSceneManagerIndices()
        {
            isValidSetup = false;

            // save the scenes info in dictionary
            for (int i = 0; i < scenesList.Count; i++)
            {
                // setup scene manager index
                SetupSceneInstanceManager(i, out bool hasLoadingManager);

                isValidSetup |= hasLoadingManager;

                if (hasLoadingManager)
                {
                    string tmp = scenesList[i];
                    scenesList[i] = scenesList[0];
                    scenesList[0] = tmp;
                }
            }
        }

#if UNITY_EDITOR
        public void OnPreprocessBuild(BuildReport report)
        {
            InitializeScenes();
        }
#endif

        private void SetupSceneInstanceManager(int sceneIndex, out bool hasLoadingManager)
        {
            hasLoadingManager = false;

            string scenePath = scenesList[sceneIndex];

            Scene sceneRef = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);

            bool cleanupScene = !sceneRef.isLoaded;

            if (!sceneRef.IsValid())
            {
                sceneRef = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

            if (!sceneRef.IsValid())
                return;

            using (ListPool<GameObject>.Get(out List<GameObject> rootGOs))
            {
                sceneRef.GetRootGameObjects(rootGOs);

                LoadingManager sceneLoadingManager = rootGOs
                    .FirstOrDefault(go => go.GetComponent<LoadingManager>() != null)?
                    .GetComponent<LoadingManager>();

                hasLoadingManager = sceneLoadingManager != null;

                ISceneInstanceManager sceneInstanceManager = rootGOs
                    .FirstOrDefault(go => go.GetComponent<ISceneInstanceManager>() != null)?
                    .GetComponent<ISceneInstanceManager>();

                if (sceneInstanceManager != null)
                {
                    sceneInstanceManager.SceneIndex = sceneIndex;
                    sceneInstanceManager.ScenePath = scenePath;
                    EditorSceneManager.SaveScene(sceneRef);
                }

                if (cleanupScene)
                {
                    EditorSceneManager.CloseScene(sceneRef, true);
                }
            }
        }
#endif
    }
}
