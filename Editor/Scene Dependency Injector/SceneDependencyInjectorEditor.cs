using Bloodthirst.Core.GameInitPass;
using Bloodthirst.Core.Setup;
using Bloodthirst.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager.DependencyInjector
{
    [InitializeOnLoad]
    public class SceneDependencyInjectorEditor
    {
        private const string PER_SCENE_INJECTOR_NAME = "[SCENE DEPENDENCY INJECTOR]";

        static SceneDependencyInjectorEditor()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_SCENE_DEPENDENCY_INJECTOR)
                return;

            EditorApplication.playModeStateChanged -= OnStateChanged;
            EditorApplication.playModeStateChanged += OnStateChanged;
        }

        private static void OnStateChanged(PlayModeStateChange state)
        {
            List<GameObject> cache = new List<GameObject>();

            List<Component> injectors = new List<Component>();

            // clean up previous objects
            if (state == PlayModeStateChange.EnteredEditMode)
            {

                // for each scene open try looking for an injector
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    Scene curr = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                    curr.GetRootGameObjects(cache);

                    for (int j = cache.Count - 1; j > -1; j--)
                    {
                        GameObject go = cache[j];
                        if (go.name.Equals(PER_SCENE_INJECTOR_NAME))
                        {
                            Object.DestroyImmediate(go);
                        }
                    }

                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(curr);
                }


            }

            // create the dependency stuff
            if (state != PlayModeStateChange.ExitingEditMode)
                return;


            Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(0);

            // TODO : find a way to mix dependencies from multiple injectors

            // for each scene open try looking for an injector
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                scene.GetRootGameObjects(cache);

                foreach (GameObject go in cache)
                {
                    IEnumerable<Component> injs = go.GetComponentsInChildren<ISceneDependencyInjector>().Cast<Component>();

                    injectors.AddRange(injs);
                }
            }

            if (injectors.Count == 0 || !currentScene.IsValid())
                return;

            // construct the dependency object
            GameObject injectorGO = new GameObject(PER_SCENE_INJECTOR_NAME);
            DependenciesInjectorBehaviour init = injectorGO.AddComponent<DependenciesInjectorBehaviour>();
            GameStart gameSetup = injectorGO.AddComponent<GameStart>();
            gameSetup.ExecuteOnStart = true;

            foreach (Component injector in injectors)
            {
                // use this in order to copy the fields too
                UnityEditorInternal.ComponentUtility.CopyComponent(injector);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(injectorGO);
            }

            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(injectorGO, currentScene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(currentScene);
        }
    }
}
