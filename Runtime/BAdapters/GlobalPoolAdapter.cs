using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(GlobalPoolContainer))]
    [RequireComponent(typeof(GlobalPoolContainer))]
    public class GlobalPoolAdapter : MonoBehaviour, ISetupSingletonPass
    {
        void ISetupSingletonPass.Execute()
        {
            GlobalPoolContainer globalPool = GetComponent<GlobalPoolContainer>();

            Assert.IsNotNull(globalPool);

            BProviderRuntime.Instance.RegisterSingleton<GlobalPoolContainer, GlobalPoolContainer>(globalPool);
            BProviderRuntime.Instance.RegisterSingleton<GlobalPoolContainer, IGlobalPool>(globalPool);

            globalPool.SetupPools();
        }
    }
}
