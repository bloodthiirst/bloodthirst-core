using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    public class SceneInstanceManagerAdapter : MonoBehaviour, ISceneInitializationPass, IPostSceneInitializationPass, IBeforeSceneUnload
    {
        void ISceneInitializationPass.Execute()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            sceneManager.Initialize(BProviderRuntime.Instance.GetSingleton<LoadingManager>());

            BProviderRuntime.Instance.RegisterInstance(sceneManager);
            BProviderRuntime.Instance.RegisterSingleton( sceneManager.SceneManagerType ,sceneManager);
        }

        void IPostSceneInitializationPass.Execute()
        {
            throw new global::System.NotImplementedException();
        }

        void IBeforeSceneUnload.Execute()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            BProviderRuntime.Instance.RemoveInstance(sceneManager);
            BProviderRuntime.Instance.RemoveSingleton(sceneManager.SceneManagerType , sceneManager);
            Debug.Log($"Scene {sceneManager.ScenePath } has been unloaded");
        }

        private void OnDestroy()
        {
            ISceneInstanceManager sceneManager = GetComponent<ISceneInstanceManager>();

            BProviderRuntime.Instance.RemoveInstance(sceneManager);
            BProviderRuntime.Instance.RemoveSingleton(sceneManager.SceneManagerType, sceneManager);
            Debug.Log($"Scene {sceneManager.ScenePath } has been unloaded");
        }
    }
}
