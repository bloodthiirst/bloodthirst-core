using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.GameInitPass
{
    public class GamePassInitiator : MonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        HashSet<IPostSceneLoadedPass> postSceneLoadedPasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<ISingletonPass> singletonPasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<ICrossSceneLoadingPass> crossSceneLoadingPasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<IInjectPass> injectPasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<IAwakePass> awakePasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<IStaticPass> staticPasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<IEnablePass> enablePasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<IPostEnablePass> postEnablePasses;

        public void OnScenesLoaded()
        {
            QueryAllPasses();

            ExecutePasses();
        }

        private void ExecutePasses()
        {
            foreach (IPostSceneLoadedPass pass in postSceneLoadedPasses)
            {
                pass.DoScenePass();
            }

            foreach (ISingletonPass pass in singletonPasses)
            {
                pass.DoSingletonPass();
            }
            foreach (ICrossSceneLoadingPass pass in crossSceneLoadingPasses)
            {
                pass.DoLoadPass();
            }
            foreach (IInjectPass pass in injectPasses)
            {
                pass.DoInjectPass();
            }

            foreach (IAwakePass pass in awakePasses)
            {
                pass.DoAwakePass();
            }

            foreach (IStaticPass pass in staticPasses)
            {
                pass.DoStaticPass();
            }

            foreach (IEnablePass pass in enablePasses)
            {
                pass.DoEnablePass();
            }

            foreach (IPostEnablePass pass in postEnablePasses)
            {
                pass.DoPostEnablePass();
            }
        }

        private void QueryAllPasses()
        {
            QueryScenes(ref postSceneLoadedPasses);
            QueryScenes(ref singletonPasses);
            QueryScenes(ref crossSceneLoadingPasses);
            QueryScenes(ref injectPasses);
            QueryScenes(ref awakePasses);
            QueryScenes(ref staticPasses);
            QueryScenes(ref enablePasses);
            QueryScenes(ref postEnablePasses);
        }

        private void QueryScenes<T>(ref HashSet<T> list) where T : IGamePass
        {
            if (list == null)
            {
                list = new HashSet<T>();
            }
            else
            {
                list.Clear();
            }

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach (GameObject rootGameObject in rootGameObjects)
                {
                    T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                    foreach (T childInterface in childrenInterfaces)
                    {
                        list.Add(childInterface);
                    }
                }
            }

        }

    }
}
