using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.UI;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IWindowLayer))]
    [RequireComponent(typeof(IWindowLayer))]
    public class WindowLayerAdapter : MonoBehaviour, IOnSceneLoaded , IOnSceneUnload
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            IWindowLayer windowLayer = GetComponent<IWindowLayer>();
            BProviderRuntime.Instance.RegisterSingleton<IWindowLayer, IWindowLayer>(windowLayer);

            windowLayer.OnInitialize();
        }

        void IOnSceneUnload.OnUnload(ISceneInstanceManager sceneInstance)
        {
            IWindowLayer windowLayer = GetComponent<IWindowLayer>();
            windowLayer.OnDestroy();
        }
    }
}
