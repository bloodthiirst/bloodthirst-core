using Bloodthirst.Core.AdvancedPool.Pools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class EntitySpawner : MonoBehaviour , IEntitySpawner
    {
        public IGlobalPool GenericUnityPool { get; set; }

        #region spawn
        public T SpawnEntity<T>() where T : MonoBehaviour
        {
            T entity = GenericUnityPool.Get<T>();

            entity = SetupEntity(entity);

            return entity;
        }

        public T SpawnEntity<T>(Predicate<T> filter) where T : MonoBehaviour
        {
            T entity = GenericUnityPool.Get(filter);
            entity = SetupEntity(entity);

            return entity;
        }
        #endregion

        #region setup
        private T SetupEntity<T>(T entity) where T : MonoBehaviour
        {
            entity = InjectStates(entity);

            // we set the id after since we might be creating new instances of the states
            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();

            id.Id = EntityManager.GetNextId();

            entity.name = $"Spawned {typeof(T).Name} - [{id.Id}]";

            id.TriggerSpawned();

            return entity;
        }
        public T InjectStates<T>(T entity) where T : MonoBehaviour
        {

            // get instance register and provider and identifier

            IEntityInstanceRegister instanceRegister = entity.GetComponentInChildren<IEntityInstanceRegisterBehaviour>().InstanceRegister;

            IEntityInstanceProvider instanceProvider = entity.GetComponentInChildren<IEntityInstanceProviderBehaviour>().InstanceProvider;

            EntityIdentifier entityIdentifier = entity.GetComponentInChildren<EntityIdentifier>();

            // initialize identifier
            foreach (IInitializeIdentifier init in entity.GetComponentsInChildren<IInitializeIdentifier>())
            {
                init.InitializeIdentifier(entityIdentifier);
            }

            // register instances

            foreach (IHasEntityInstanceRegister init in entity.GetComponentsInChildren<IHasEntityInstanceRegister>())
            {
                init.InitializeEntityInstanceRegister(instanceRegister);
            }

            // init instancee

            foreach (IInitializeInstance init in entity.GetComponentsInChildren<IInitializeInstance>())
            {
                init.InitializeInstance(entityIdentifier);
            }

            // initialize provider

            foreach (IHasEntityInstanceProvider init in entity.GetComponentsInChildren<IHasEntityInstanceProvider>())
            {
                init.InitializeEntityInstanceProvider(instanceProvider);
            }

            // query instance dependencies

            foreach (IQueryInstance inject in entity.GetComponentsInChildren<IQueryInstance>())
            {
                inject.QueryInstance(instanceProvider);
            }

            return entity;
        }
        public BEHAVIOUR AddBehaviour<BEHAVIOUR, INSTANCE, STATE, DATA>(MonoBehaviour entity) where DATA : EntityData where STATE : class, IEntityState<DATA>, new() where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>, new() where BEHAVIOUR : EntityBehaviour<DATA, STATE, INSTANCE>
        {
            // get instance register and provider and identifier

            IEntityInstanceRegister instanceRegister = entity.GetComponentInChildren<IEntityInstanceRegisterBehaviour>().InstanceRegister;

            IEntityInstanceProvider instanceProvider = entity.GetComponentInChildren<IEntityInstanceProviderBehaviour>().InstanceProvider;

            EntityIdentifier entityIdentifier = entity.GetComponentInChildren<EntityIdentifier>();

            BEHAVIOUR behaviour = entity.gameObject.AddComponent<BEHAVIOUR>();

            ((IInitializeIdentifier)behaviour).InitializeIdentifier(entityIdentifier);

            // register instances
            ((IHasEntityInstanceRegister)behaviour).InitializeEntityInstanceRegister(instanceRegister);

            // init instancee
            ((IInitializeInstance)behaviour).InitializeInstance(entityIdentifier);

            // initialize provider

            ((IHasEntityInstanceProvider)behaviour).InitializeEntityInstanceProvider(instanceProvider);

            // query instance dependencies
            if (behaviour is IQueryInstance query)
            {
                query.QueryInstance(instanceProvider);
            }

            return behaviour;
        }

        #endregion

        #region remove
        public void RemoveEntity<T>(T player) where T : MonoBehaviour
        {
            EntityIdentifier identifier = player.GetComponentInChildren<EntityIdentifier>();

            identifier.TriggerRemoved();

            GenericUnityPool.Return(player);
        }
        #endregion

        #region  load game
        public List<GameObject> LoadGameState(GameStateSaveInstance gameData)
        {
            return SaveLoadManager.Load(gameData, true);
        }
        #endregion
    }
}
