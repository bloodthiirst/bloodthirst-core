using UnityEngine;
using Bloodthirst.Core.ServiceProvider;

namespace Bloodthirst.Core.BISDSystem
{
    public class BISDSpawnInfo : ISavableSpawnInfo
    {
        [SerializeField]
        private int id;

        [SerializeField]
        private EntityType entityType;

        public int Id { get => id; set => id = value; }
        public EntityType EntityType { get => entityType; set => entityType = value; }

        public GameObject GetInstanceToInject()
        {
            return BProviderRuntime.Instance.
                GetSingleton<EntitySpawner>()
                .SpawnEntity<EntityIdentifier>(e => e.EntityType == EntityType)
                .gameObject;
        }

        public void PostStatesApplied(GameObject gameObject)
        {
            BProviderRuntime.Instance.
                GetSingleton<EntitySpawner>().
                InjectStates<EntityIdentifier>( gameObject.GetComponent<EntityIdentifier>());
        }
    }
}
