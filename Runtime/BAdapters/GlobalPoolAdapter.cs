using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IGlobalPool))]
    [RequireComponent(typeof(IGlobalPool))]
    public class GlobalPoolAdapter : MonoBehaviour, IOnSceneLoaded
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            IGlobalPool globalPool = GetComponent<IGlobalPool>();

            Assert.IsNotNull(globalPool);

            BProviderRuntime.Instance.RegisterSingleton<IGlobalPool, IGlobalPool>(globalPool);

            globalPool.SetupPools();
        }
    }
}
