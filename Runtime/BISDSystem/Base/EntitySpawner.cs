using Assets.Scripts.Core.UnityPool;
using Bloodthirst.Core.BISDSystem;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class EntitySpawner : MonoBehaviour
    {
        public T SpawnEntity<T>() where T : MonoBehaviour
        {
            T entity = UnityPool.Instance.Get<T>();

            entity.name = "Spawned " + typeof(T).Name;

            entity = InjectDependencies(entity);

            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();
            
            id.TriggerSpawned();

            return entity;
        }

        public T SpawnEntity<T>(Predicate<T> filter) where T : MonoBehaviour
        {
            T entity = UnityPool.Instance.Get(filter);

            entity.name = "Spawned " + typeof(T).Name;

            entity = InjectDependencies(entity);

            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();

            id.TriggerSpawned();

            return entity;
        }

        public T InjectDependencies<T>(T entity) where T : MonoBehaviour
        {

            // get instance register and provider and identifier

            IInstanceRegister instanceRegister = entity.GetComponentInChildren<IInstanceRegisterBehaviour>().InstanceRegister;

            IInstanceProvider instanceProvider = entity.GetComponentInChildren<IInstanceProviderBehaviour>().InstanceProvider;

            EntityIdentifier entityIdentifier = entity.GetComponentInChildren<EntityIdentifier>();

            // init instancee

            foreach (IInitializeInstance init in entity.GetComponentsInChildren<IInitializeInstance>())
            {
                init.InitializeInstance(entityIdentifier);
            }

            // initialize identifier
            foreach(IInitializeIdentifier init in entity.GetComponentsInChildren< IInitializeIdentifier>())
            {
                init.InitializeIdentifier(entityIdentifier);
            }

            // initialize provider

            foreach (IInitializeProvider init in entity.GetComponentsInChildren<IInitializeProvider>())
            {
                init.InitializeProvider(instanceProvider);
            }

            // register instances

            foreach (IRegisterInstance init in entity.GetComponentsInChildren<IRegisterInstance>())
            {
                init.RegisterInstance(instanceRegister);
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
            
            UnityPool.Instance.Return(player);
        }
    }
}
