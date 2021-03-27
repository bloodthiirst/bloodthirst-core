using Bloodthirst.Scripts.Core.UnityPool;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class EntitySpawner : MonoBehaviour
    {
        public T SpawnEntity<T>() where T : MonoBehaviour
        {
            T entity = GenericUnityPool.Instance.Get<T>();

            entity.name = "Spawned " + typeof(T).Name;

            entity = InjectDependencies(entity);

            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();

            id.TriggerSpawned();

            return entity;
        }

        public T SpawnEntity<T>(Predicate<T> filter) where T : MonoBehaviour
        {
            T entity = GenericUnityPool.Instance.Get(filter);

            entity.name = "Spawned " + typeof(T).Name;

            entity = InjectDependencies(entity);

            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();

            id.TriggerSpawned();

            return entity;
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
            ((IHasEntityRegisterInstance)behaviour).ProvideEntityInstanceInstance(instanceRegister);

            // init instancee
           ((IInitializeInstance)behaviour).InitializeInstance(entityIdentifier);

            // initialize provider

            ((IInitializeProvider)behaviour).InitializeProvider(instanceProvider);

            // query instance dependencies
            if (behaviour is IQueryInstance query)
            {
                query.QueryInstance(instanceProvider);
            }

            return behaviour;
        }

        public T InjectDependencies<T>(T entity) where T : MonoBehaviour
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

            foreach (IHasEntityRegisterInstance init in entity.GetComponentsInChildren<IHasEntityRegisterInstance>())
            {
                init.ProvideEntityInstanceInstance(instanceRegister);
            }


            // init instancee

            foreach (IInitializeInstance init in entity.GetComponentsInChildren<IInitializeInstance>())
            {
                init.InitializeInstance(entityIdentifier);
            }


            // initialize provider

            foreach (IInitializeProvider init in entity.GetComponentsInChildren<IInitializeProvider>())
            {
                init.InitializeProvider(instanceProvider);
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

            GenericUnityPool.Instance.Return(player);
        }
    }
}
