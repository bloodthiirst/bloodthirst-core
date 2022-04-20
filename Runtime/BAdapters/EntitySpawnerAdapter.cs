using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    public class EntitySpawnerAdapter : MonoBehaviour , IQuerySingletonPass
    {
        void IQuerySingletonPass.Execute()
        {
            IEntitySpawner spawner = GetComponent<IEntitySpawner>();

            // globalpoolcontainer is in the project asembly and not the package , so create a pool container interface
            spawner.GenericUnityPool = BProviderRuntime.Instance.GetSingleton<IGlobalPool>();
        }
    }
}
