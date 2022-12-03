using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IGlobalPool))]
    [RequireComponent(typeof(IGlobalPool))]
    public class GlobalPoolAdapter : MonoBehaviour, ISetupSingletonPass
    {
        void ISetupSingletonPass.Execute()
        {
            IGlobalPool globalPool = GetComponent<IGlobalPool>();

            Assert.IsNotNull(globalPool);

            Type t = Type.GetType("GlobalPoolContainer");

            BProviderRuntime.Instance.RegisterSingleton(t  ,globalPool);
            BProviderRuntime.Instance.RegisterSingleton<IGlobalPool, IGlobalPool>(globalPool);

            globalPool.SetupPools();
        }
    }
}
