using Assets.Scripts.Core.GamePassInitiator;
using Bloodthirst;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core.GameInitPass
{
    public class GamePassInitiator : MonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        HashSet<IScenePass> scenePasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<ISingletonPass> singletonPasses;

        [ShowInInspector]
        [ReadOnly]
        HashSet<ILoadPass> loadPasses;

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

        public void GlobalGameAwake()
        {
            QueryAllPasses();

            ExecutePasses();
        }

        private void ExecutePasses()
        {
            foreach (IScenePass pass in scenePasses)
            {
                pass.DoScenePass();
            }

            foreach (ISingletonPass pass in singletonPasses)
            {
                pass.DoSingletonPass();
            }
            foreach (ILoadPass pass in loadPasses)
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
            QueryScenes(ref singletonPasses);
            QueryScenes(ref loadPasses);
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

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = SceneManager.GetSceneAt(i).GetRootGameObjects();
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
