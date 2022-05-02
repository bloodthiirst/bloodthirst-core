using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(ISceneInstanceManager))]
    [RequireComponent(typeof(ISceneInstanceManager))]
    public class SceneInstanceManagerAdapter : MonoBehaviour, ISceneInitializationPass, IPostSceneInitializationPass, IBeforeSceneUnload
    {
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
            Debug.Log($"Scene {sceneManager.ScenePath } has been unloaded");
        }

        private void OnDestroy()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            if (sceneManager == null)
                return;

            BProviderRuntime.Instance.RemoveInstance(sceneManager);
            BProviderRuntime.Instance.RemoveSingleton(sceneManager.SceneManagerType, sceneManager);
            Debug.Log($"Scene {sceneManager.ScenePath } has been unloaded");
        }
    }
}
