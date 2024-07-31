using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BProvider;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.BISDSystem
{
    public class BISDInstanceProvider : ISavableInstanceProvider
    {
        [SerializeField]
        private int id;

        [SerializeField]
        private EntityType entityType;

        public int Id { get => id; set => id = value; }
        public EntityType EntityType { get => entityType; set => entityType = value; }

        public GameObject GetInstanceToInject()
        {
            EntitySpawner spawner = BProviderRuntime.Instance.GetSingleton<EntitySpawner>();
            IGlobalPool globalPool = BProviderRuntime.Instance.GetSingleton<IGlobalPool>();

            IPoolBehaviour pool = null;

            foreach (IPoolBehaviour p in globalPool.AllPools)
            {
                if (!p.Prefab.TryGetComponent(out EntityIdentifier ent))
                {
                    continue;
                }

                if (ent.EntityType != entityType)
                {
                    continue;
                }

                pool = p;
                break;
            }

            Assert.IsNotNull(pool);

            return pool.Pool.Get();
        }

        public void PostStatesApplied(GameObject gameObject)
        {
            BProviderRuntime.Instance.
                GetSingleton<EntitySpawner>().
                InjectStates(gameObject);
        }
    }
}
