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

            return entity;
        }

        public T SpawnEntity<T>(Predicate<T> filter) where T : MonoBehaviour
        {
            T entity = UnityPool.Instance.Get(filter);

            entity.name = "Spawned " + typeof(T).Name;

            entity = InjectDependencies(entity);

            return entity;
        }

        public T InjectDependencies<T>(T entity) where T : MonoBehaviour
        {
            // init instancee

            foreach (IInitializeInstance init in entity.GetComponentsInChildren<IInitializeInstance>())
            {
                init.InitializeInstance();
            }

            // get instance register and provider

            IInstanceRegister instanceRegister = entity.GetComponentInChildren<IInstanceRegisterBehaviour>().InstanceRegister;

            IInstanceProvider instanceProvider = entity.GetComponentInChildren<IInstanceProviderBehaviour>().InstanceProvider;

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
            foreach(IRemovableBehaviour removable in player.GetComponentsInChildren<IRemovableBehaviour>(true) )
            {
                removable.Removable.Remove();
            }

            UnityPool.Instance.Return(player);
        }
    }
}
