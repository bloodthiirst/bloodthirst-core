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
        }

        void IOnSceneUnload.OnUnload(ISceneInstanceManager sceneInstance)
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();
            Assert.IsNotNull(sceneManager);

            Debug.Log($"Before Scene Unloaded {sceneManager.ScenePath}");
        }


        private void OnDestroy()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            if (sceneManager == null)
            {
                return;
            }

            bool rmvInstance = BProviderRuntime.Instance.RemoveInstance(sceneManager);

            if (rmvInstance)
            {
                Debug.Log($"Scene {sceneManager.ScenePath} has been unloaded OnDestroy");
            }
        }
    }
}
