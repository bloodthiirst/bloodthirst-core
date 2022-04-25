using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IEntitySpawner))]
    [RequireComponent(typeof(IEntitySpawner))]
    public class EntitySpawnerAdapter : MonoBehaviour , IQuerySingletonPass
    {
        void IQuerySingletonPass.Execute()
        {
            IEntitySpawner spawner = GetComponent<IEntitySpawner>();

            IGlobalPool globalPool = BProviderRuntime.Instance.GetSingleton<IGlobalPool>();
            spawner.GenericUnityPool = globalPool;
        }
    }
}
