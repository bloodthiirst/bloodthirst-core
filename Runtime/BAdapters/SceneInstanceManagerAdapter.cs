using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Setup;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(ISceneInstanceManager))]
    [RequireComponent(typeof(ISceneInstanceManager))]
    public class SceneInstanceManagerAdapter : MonoBehaviour, ISceneInitializationPass, IPostSceneInitializationPass, IBeforeSceneUnload , IGameEnd
    {
        private bool isAlive;
        int IGameEnd.Order => -GetComponent<ISceneInstanceManager>().SceneIndex;

        void ISceneInitializationPass.Execute()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            Assert.IsNotNull(sceneManager);

            sceneManager.Initialize(BProviderRuntime.Instance.GetSingleton<LoadingManager>());

            BProviderRuntime.Instance.RegisterInstance(sceneManager);
            BProviderRuntime.Instance.RegisterSingleton( sceneManager.SceneManagerType ,sceneManager);
        }

        void IPostSceneInitializationPass.Execute()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            Assert.IsNotNull(sceneManager);

            sceneManager.OnPostInitialization();
        }

        void IBeforeSceneUnload.Execute()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            Assert.IsNotNull(sceneManager);

            BProviderRuntime.Instance.RemoveInstance(sceneManager);
            BProviderRuntime.Instance.RemoveSingleton(sceneManager.SceneManagerType , sceneManager);
            Debug.Log($"Before Scene Unloaded {sceneManager.ScenePath }");
        }

        IAsynOperationWrapper IGameEnd.GetAsyncOperations()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            if (sceneManager.IsConfigScene)
            {
                return null;
            }

            return new UnloadSceneAsyncWrapper(sceneManager.ScenePath);
        }

        private void OnDestroy()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            if (sceneManager == null)
                return;

            bool rmvInstance = BProviderRuntime.Instance.RemoveInstance(sceneManager);
            bool rmvSingleton = BProviderRuntime.Instance.RemoveSingleton(sceneManager.SceneManagerType, sceneManager);

            if (rmvInstance || rmvSingleton)
            {
                Debug.Log($"Scene {sceneManager.ScenePath} has been unloaded OnDestroy");
            }
        }
    }
}
