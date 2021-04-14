using Bloodthirst.Core.GameInitPass;
using System.Collections.Generic;
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
            EditorApplication.playModeStateChanged -= OnStateChanged;
            EditorApplication.playModeStateChanged += OnStateChanged;
        }

        private static void OnStateChanged(PlayModeStateChange state)
        {
            List<GameObject> cache = new List<GameObject>();

            Component injector = null;

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
                }
            }

            // create the dependency stuff
            if (state != PlayModeStateChange.ExitingEditMode)
                return;


            Scene currentScene = default;

            // TODO : find a way to mix dependencies from multiple injectors

            // for each scene open try looking for an injector
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                currentScene.GetRootGameObjects(cache);

                foreach (GameObject go in cache)
                {
                    if(go.TryGetComponent(typeof(ISceneDependencyInjector) , out injector))
                    {
                        break;
                    }
                }

                if (injector != null)
                    break;
            }

            if (injector == null || !currentScene.IsValid())
                return;

            // construct the dependency object
            GameObject injectorGO = new GameObject(PER_SCENE_INJECTOR_NAME);
            GamePassInitiator init = injectorGO.AddComponent<GamePassInitiator>();

            // use this in order to copy the fields too
            UnityEditorInternal.ComponentUtility.CopyComponent(injector);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(injectorGO);

            init.executePassesOnStart = true;

            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(injectorGO , currentScene);

        }
    }
}
