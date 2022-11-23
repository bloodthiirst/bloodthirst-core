using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.UI;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IWindowLayer))]
    [RequireComponent(typeof(IWindowLayer))]
    public class WindowLayerAdapter : MonoBehaviour, IQuerySingletonPass
    {
        void IQuerySingletonPass.Execute()
        {
            IWindowLayer windowLayer = GetComponent<IWindowLayer>();
            BProviderRuntime.Instance.RegisterSingleton<IWindowLayer, IWindowLayer>(windowLayer);

            windowLayer.OnInitialize();
        }

        private void OnDestroy()
        {
            IWindowLayer windowLayer = GetComponent<IWindowLayer>();
            windowLayer.OnDestroy();
        }
    }
}
