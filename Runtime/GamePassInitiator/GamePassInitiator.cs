using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.GameInitPass
{
    public class GamePassInitiator : MonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        List<IBeforeAllScenesInitializationPass> beforeAllScenesInitializationPasses;

        [ShowInInspector]
        [ReadOnly]
        List<ISceneInitializationPass> sceneInitializationPasses;

        [ShowInInspector]
        [ReadOnly]
        List<IPostSceneInitializationPass> postSceneInitializationPasses;

        [ShowInInspector]
        [ReadOnly]
        List<IAfterAllScenesIntializationPass> afterAllScenesIntializationPasses;


        [ShowInInspector]
        [ReadOnly]
        List<ISetupSingletonPass> setupSingletonPasses;

        [ShowInInspector]
        [ReadOnly]
        List<IQuerySingletonPass> querySingletonPasses;

        [ShowInInspector]
        [ReadOnly]
        List<IInjectPass> injectPasses;

        [ShowInInspector]
        [ReadOnly]
        List<IAwakePass> awakePasses;

        [ShowInInspector]
        [ReadOnly]
        List<IPostAwakePass> postAwakePasses;

        [ShowInInspector]
        [ReadOnly]
        List<IEnablePass> enablePasses;

        [ShowInInspector]
        [ReadOnly]
        List<IPostEnablePass> postEnablePasses;

        public void OnScenesLoaded()
        {
            QueryAllPasses();

            ExecutePasses();
        }

        private void ExecutePasses()
        {
            #region scenes
            
            foreach (IBeforeAllScenesInitializationPass pass in beforeAllScenesInitializationPasses)
            {
                pass.Execute();
            }
            foreach (ISceneInitializationPass pass in sceneInitializationPasses.OrderBy(s => s.SceneOrder))
            {
                pass.Execute();
            }
            foreach (IPostSceneInitializationPass pass in postSceneInitializationPasses.OrderBy(s => s.SceneOrder))
            {
                pass.Execute();
            }

            foreach (IAfterAllScenesIntializationPass pass in afterAllScenesIntializationPasses)
            {
                pass.Execute();
            }

            #endregion

            #region singletons

            foreach (ISetupSingletonPass pass in setupSingletonPasses)
            {
                pass.Execute();
            }
            foreach (IQuerySingletonPass pass in querySingletonPasses)
            {
                pass.Execute();
            }
            
            #endregion

            foreach (IInjectPass pass in injectPasses)
            {
                pass.Execute();
            }

            foreach (IAwakePass pass in awakePasses)
            {
                pass.Execute();
            }

            foreach (IPostAwakePass pass in postAwakePasses)
            {
                pass.Execute();
            }

            foreach (IEnablePass pass in enablePasses)
            {
                pass.Execute();
            }

            foreach (IPostEnablePass pass in postEnablePasses)
            {
                pass.Execute();
            }
        }

        private void QueryAllPasses()
        {
            // scenes
            QueryScenes(ref beforeAllScenesInitializationPasses);
            QueryScenes(ref sceneInitializationPasses);
            QueryScenes(ref postSceneInitializationPasses);
            QueryScenes(ref afterAllScenesIntializationPasses);

            // singletons
            QueryScenes(ref setupSingletonPasses);
            QueryScenes(ref querySingletonPasses);

            // the rest
            QueryScenes(ref injectPasses);
            QueryScenes(ref awakePasses);
            QueryScenes(ref postAwakePasses);
            QueryScenes(ref enablePasses);
            QueryScenes(ref postEnablePasses);
        }

        private void QueryScenes<T>(ref List<T> list) where T : IGamePass
        {
            list = CollectionsUtils.CreateOrClear(list);

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach (GameObject rootGameObject in rootGameObjects)
                {
                    GetComponentsOfType(list, rootGameObject);
                }
            }
        }

        private static void GetComponentsOfType<T>(List<T> list, GameObject rootGameObject) where T : IGamePass
        {
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>(true);
            foreach (T childInterface in childrenInterfaces)
            {
                list.Add(childInterface);
            }
        }
    }
}
