﻿using Bloodthirst.Core.BProvider;
using UnityEngine;

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

            return spawner
                .SpawnEntity<EntityIdentifier>(e => e.EntityType == EntityType)
                .gameObject;
        }

        public void PostStatesApplied(GameObject gameObject)
        {
            BProviderRuntime.Instance.
                GetSingleton<EntitySpawner>().
                InjectStates<EntityIdentifier>(gameObject.GetComponent<EntityIdentifier>());
        }
    }
}
