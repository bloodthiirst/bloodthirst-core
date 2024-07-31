using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(EntitySpawner))]
    [RequireComponent(typeof(EntitySpawner))]
    public class EntitySpawnerAdapter : MonoBehaviour , IOnSceneLoaded
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            EntitySpawner spawner = GetComponent<EntitySpawner>();
            BProviderRuntime.Instance.RegisterSingleton(spawner);

            IGlobalPool globalPool = BProviderRuntime.Instance.GetSingleton<IGlobalPool>();
            spawner.GenericUnityPool = globalPool;
        }
    }
}
