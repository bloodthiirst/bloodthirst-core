using Bloodthirst.Core.Setup;
using Bloodthirst.Core.SceneManager.DependencyInjector;
using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.GameInitPass
{
    public class GamePassInitiator : MonoBehaviour , IPreGameSetup , IPostGameSetup
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

        private BProvider scriptables;

        private List<ISceneDependencyInjector> sceneDependencyInjector = new List<ISceneDependencyInjector>();

        void IPreGameSetup.Execute()
        {
            scriptables = QueryScriptableObjects();
        }

        void IPostGameSetup.Execute()
        {
            AfterScenesLoaded();
        }

        private BProvider QueryScriptableObjects()
        {
            BProvider scProvider = new BProvider();

            UnityEngine.Object[] scriptables = Resources.LoadAll(string.Empty);

            List<IScriptableObject> scInstances = scriptables.OfType<IScriptableObject>().ToList();
            List<IScriptableObjectSingleton> scSingletons = scriptables.OfType<IScriptableObjectSingleton>().ToList();

            // instances
            foreach (IScriptableObject s in scInstances)
            {
                Type t = s.GetType();

                Type[] interfaces = t.GetInterfaces();
                Type genericType = interfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IScriptableObject<>));

                if (genericType == null)
                    continue;

                if (!genericType.IsGenericType)
                {
                    scProvider.RegisterInstance(t, s);
                }
                else
                {
                    Type genericParam = genericType.GetGenericArguments()[0];
                    scProvider.RegisterInstance(genericParam, s);
                }
            }

            // singletons
            foreach (IScriptableObjectSingleton s in scSingletons)
            {
                Type t = s.GetType();

                Type[] interfaces = t.GetInterfaces();
                Type genericType = interfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IScriptableObjectSingleton<>));

                if (!genericType.IsGenericType)
                {
                    scProvider.RegisterInstance(t, s);
                }
                else
                {
                    Type genericParam = genericType.GetGenericArguments()[0];
                    scProvider.RegisterInstance(genericParam, s);
                }
            }

            return scProvider;
        }

        private void AfterScenesLoaded()
        {
            // scriptables
            BProviderRuntime.Instance.MergeWith(scriptables);

            // scene dependency for single scene stuff
            GetComponents(sceneDependencyInjector);

            foreach (ISceneDependencyInjector inj in sceneDependencyInjector)
            {
                BProvider injectionProvider = inj.GetProvider();
                BProviderRuntime.Instance.MergeWith(injectionProvider);
            }

            // query the rest
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
            foreach (ISceneInitializationPass pass in sceneInitializationPasses)
            {
                pass.Execute();
            }
            foreach (IPostSceneInitializationPass pass in postSceneInitializationPasses)
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
