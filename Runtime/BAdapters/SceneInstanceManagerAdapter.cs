using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(ISceneInstanceManager))]
    [RequireComponent(typeof(ISceneInstanceManager))]
    public class SceneInstanceManagerAdapter : MonoBehaviour, IOnSceneLoaded, IOnSceneUnload
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            Assert.IsNotNull(sceneManager);

            LoadingManager loadingManager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

            sceneManager.Initialize(loadingManager);

            BProviderRuntime.Instance.RegisterInstance(sceneManager);
            BProviderRuntime.Instance.RegisterSingleton(sceneManager.SceneManagerType, sceneManager);
        }

        void IOnSceneUnload.OnUnload(ISceneInstanceManager sceneInstance)
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            Assert.IsNotNull(sceneManager);

            BProviderRuntime.Instance.RemoveInstance(sceneManager);
            BProviderRuntime.Instance.RemoveSingleton(sceneManager.SceneManagerType, sceneManager);

            Debug.Log($"Before Scene Unloaded {sceneManager.ScenePath}");
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
