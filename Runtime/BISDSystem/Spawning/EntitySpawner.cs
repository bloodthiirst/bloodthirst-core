using Bloodthirst.Core.AdvancedPool.Pools;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Core.BISDSystem
{
    public class EntitySpawner : MonoBehaviour
    {
        public IGlobalPool GenericUnityPool { get; set; }

        #region setup
        public void PostInject(GameObject entity)
        {
            // we set the id after since we might be creating new instances of the states
            EntityIdentifier id = entity.GetComponent<EntityIdentifier>();

            id.Id = EntityManager.GetNextId();

            id.TriggerSpawned();
        }

        public void InjectStates(GameObject entity)
        {
            // get instance register and provider and identifier

            EntityIdentifier id = entity.GetComponentInChildren<EntityIdentifier>();
            Assert.IsNotNull(id);

            using (ListPool<IInitializeIdentifier>.Get(out List<IInitializeIdentifier> initIDs))
            using (ListPool<IInitializeInstance>.Get(out List<IInitializeInstance> initInstances))
            using (ListPool<IEntityPostInit>.Get(out List<IEntityPostInit> postInits))
            {
                entity.GetComponentsInChildren(initIDs);
                entity.GetComponentsInChildren(initInstances);
                entity.GetComponentsInChildren(postInits);

                // initialize identifier
                foreach (IInitializeIdentifier init in initIDs)
                {
                    init.InitializeIdentifier(id);
                }

                // init instancee

                foreach (IInitializeInstance init in initInstances)
                {
                    init.InitializeInstance(id);
                }

                // post init
                foreach (IEntityPostInit post in postInits)
                {
                    post.PostInit();
                }
            }
        }
        public BEHAVIOUR AddBehaviour<BEHAVIOUR, INSTANCE, STATE, DATA>(MonoBehaviour entity) where DATA : EntityData where STATE : class, IEntityState<DATA>, new() where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>, new() where BEHAVIOUR : EntityBehaviour<DATA, STATE, INSTANCE>
        {
            // get instance register and provider and identifier

            IEntityInstanceRegister instanceRegister = entity.GetComponentInChildren<IEntityInstanceRegisterBehaviour>().InstanceRegister;

            IEntityInstanceProvider instanceProvider = entity.GetComponentInChildren<IEntityInstanceProviderBehaviour>().InstanceProvider;

            EntityIdentifier entityIdentifier = entity.GetComponentInChildren<EntityIdentifier>();

            BEHAVIOUR behaviour = entity.gameObject.AddComponent<BEHAVIOUR>();

            ((IInitializeIdentifier)behaviour).InitializeIdentifier(entityIdentifier);

            // init instancee
            ((IInitializeInstance)behaviour).InitializeInstance(entityIdentifier);

            // query instance dependencies
            if (behaviour is IQueryInstance query)
            {
                query.QueryInstance(instanceProvider);
            }

            return behaviour;
        }

        #endregion

        #region remove
        public void RemoveEntity(GameObject entity)
        {
            EntityIdentifier id = entity.GetComponentInChildren<EntityIdentifier>();
            Assert.IsNotNull(id);

            id.TriggerRemoved();
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
