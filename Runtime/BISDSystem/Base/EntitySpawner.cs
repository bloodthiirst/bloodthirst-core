using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.UnityPool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class EntitySpawner : MonoBehaviour , IQuerySingletonPass
    {
        private GenericUnityPool _genericUnityPool;

        void IQuerySingletonPass.Execute()
        {
            _genericUnityPool = BProviderRuntime.Instance.GetSingleton<GenericUnityPool>();
        }
        public T SpawnEntity<T>(IList<IEntityState> preloadedStates = null) where T : MonoBehaviour
        {
            T entity = _genericUnityPool.Get<T>();

            entity = SetupEntity(entity , preloadedStates);

            return entity;
        }

        public T SpawnEntity<T>(Predicate<T> filter , IList<IEntityState> preloadedStates = null ) where T : MonoBehaviour
        {
            T entity = _genericUnityPool.Get(filter);
            entity = SetupEntity(entity , preloadedStates);

            return entity;
        }

        private T SetupEntity<T>(T entity , IList<IEntityState> preloadedStates = null) where T : MonoBehaviour
        {
            entity = InjectDependencies(entity , preloadedStates);

            // we set the id after since we might be creating new instances of the states
            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();

            id.Id = EntityManager.GetNextId();

            entity.name = $"Spawned {typeof(T).Name} - [{id.Id}]";

            id.TriggerSpawned();
            return entity;
        }

        public void LoadGameState(BISDGameStateData gameData)
        {
            EntityManager.Load(gameData, this);
        }

        public BEHAVIOUR AddBehaviour<BEHAVIOUR, INSTANCE, STATE, DATA>(MonoBehaviour entity) where DATA : EntityData where STATE : class, IEntityState<DATA> , new() where INSTANCE : EntityInstance<DATA, STATE, INSTANCE> , new() where BEHAVIOUR : EntityBehaviour<DATA, STATE, INSTANCE> 
        {
            // get instance register and provider and identifier

            IEntityInstanceRegister instanceRegister = entity.GetComponentInChildren<IInstanceRegisterBehaviour>().InstanceRegister;

            IInstanceProvider instanceProvider = entity.GetComponentInChildren<IInstanceProviderBehaviour>().InstanceProvider;

            EntityIdentifier entityIdentifier = entity.GetComponentInChildren<EntityIdentifier>();

            BEHAVIOUR behaviour = entity.gameObject.AddComponent<BEHAVIOUR>();

            ((IInitializeIdentifier)behaviour).InitializeIdentifier(entityIdentifier);

            // register instances
            ((IHasEntityInstanceRegister)behaviour).InitializeEntityInstanceRegister(instanceRegister);

            // init instancee
           ((IInitializeInstance)behaviour).InitializeInstance(entityIdentifier , null);

            // initialize provider

            ((IHasEntityInstanceProvider)behaviour).InitializeEntityInstanceProvider(instanceProvider);

            // query instance dependencies
            if (behaviour is IQueryInstance query)
            {
                query.QueryInstance(instanceProvider);
            }

            return behaviour;
        }

        public T InjectDependencies<T>(T entity , IList<IEntityState> preloadedStates = null) where T : MonoBehaviour
        {

            // get instance register and provider and identifier

            IEntityInstanceRegister instanceRegister = entity.GetComponentInChildren<IInstanceRegisterBehaviour>().InstanceRegister;

            IInstanceProvider instanceProvider = entity.GetComponentInChildren<IInstanceProviderBehaviour>().InstanceProvider;

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
                IEntityState lookForPreload = preloadedStates.FirstOrDefault(s => s.GetType() == init.StateType);
                init.InitializeInstance(entityIdentifier , lookForPreload);
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

        public void RemoveEntity<T>(T player) where T : MonoBehaviour
        {
            EntityIdentifier identifier = player.GetComponentInChildren<EntityIdentifier>();

            identifier.TriggerRemoved();

            _genericUnityPool.Return(player);
        }

    }
}
