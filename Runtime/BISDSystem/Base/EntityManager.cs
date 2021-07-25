using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class InstanceRegister<INSTANCE> where INSTANCE : IEntityInstance
    {
        private static HashSet<INSTANCE> instances;

        private static HashSet<INSTANCE> Instances
        {
            get
            {
                if (instances == null)
                {
                    instances = new HashSet<INSTANCE>();
                    EntityManager._AllInstanceSets.Add(instances);

                }

                return instances;
            }
        }

        /// <summary>
        /// List of the instances alive
        /// </summary>
        public static IReadOnlyCollection<INSTANCE> AvailableInstances => Instances;


        /// <summary>
        /// register the instance as an alive instances
        /// </summary>
        /// <param name="i"></param>
        public static void Register(INSTANCE i)
        {
            if (Instances.Add(i))
            {
                Debug.Log($"Instance of { typeof(INSTANCE) } has been registered");
                OnInstanceAdded<INSTANCE>.Invoke(i);
            }
        }

        /// <summary>
        /// unregister the instance from the alive instances
        /// </summary>
        public static void Unregister(INSTANCE i)
        {
            if (Instances.Remove(i))
            {
                Debug.Log($"Instance of { typeof(INSTANCE) } has been unregistered");
                OnInstanceRemoved<INSTANCE>.Invoke(i);
            }
        }


    }

    public class EntityManager
    {
        private static int entityCount;

        private static int EntityCount => entityCount;

        public static int GetNextId()
        {
            return entityCount++;
        }

        private static List<IEnumerable> _allInstanceSets;

        internal static List<IEnumerable> _AllInstanceSets
        {
            get
            {
                if (_allInstanceSets == null)
                {
                    _allInstanceSets = new List<IEnumerable>();

                }

                return _allInstanceSets;
            }
        }

        [Button]
        public static Dictionary<PrefabIDPair, List<IEntityState>> GetAllEntityStates()
        {
            Dictionary<PrefabIDPair, List<IEntityState>> saveData = new Dictionary<PrefabIDPair, List<IEntityState>>();

            foreach (IEnumerable set in _AllInstanceSets)
            {
                foreach (IEntityInstance ins in set)
                {
                    // if doesn't have id , then we don't save it
                    if (ins.EntityIdentifier == null)
                        continue;

                    PrefabIDPair key = new PrefabIDPair() { Id = ins.EntityIdentifier.Id, PrefabRefernece = ins.EntityIdentifier.PrefabReferenceData.PrefabReference };


                    if (!saveData.TryGetValue(key, out List<IEntityState> states))
                    {
                        states = new List<IEntityState>();
                        saveData.Add(key, states);
                    }

                    states.Add(ins.State);
                }
            }

            return saveData;
        }

        // TODO : work on loading
        [Button]
        public static void Load(BISDGameStateData gameData , bool withPostLoad)
        {
            EntitySpawner spawner = Object.FindObjectOfType<EntitySpawner>();

            Load(gameData, spawner , withPostLoad);
        }

        public static void Load(BISDGameStateData gameData , EntitySpawner spawner , bool withPostLoad)
        {
            List<EntityIdentifier> spawned = withPostLoad ? new List<EntityIdentifier>() : null;

            foreach (KeyValuePair<PrefabIDPair, List<IEntityState>> kv in gameData.States)
            {
                // get the id component of the entity
                EntityIdentifier prefabId = kv.Key.PrefabRefernece.GetComponent<EntityIdentifier>();

                // copy the preloaded states
                List<IEntityState> dataToBeLoaded = kv.Value.ToList();

                // spawn the entity with the preloaded data
                EntityIdentifier loadedEntity = spawner.SpawnEntity<EntityIdentifier>(e => e.EntityType == prefabId.EntityType , dataToBeLoaded);

                if (withPostLoad)
                {
                    spawned.Add(loadedEntity);
                }


                foreach (IEntityInstance loadable in loadedEntity.GetComponentsInChildren<IEntityInstance>())
                {
                    for (int i = dataToBeLoaded.Count - 1; i > -1; i--)
                    {
                        IEntityState currState = dataToBeLoaded[i];

                        if (loadable.StateType == currState.GetType())
                        {
                            loadable.State = currState;
                            dataToBeLoaded.RemoveAt(i);
                            break;
                        }
                    }
                }

            }

            if (!withPostLoad)
                return;

            foreach(EntityIdentifier s in spawned)
            {
                IPostEntityLoaded[] postLoads = s.GetComponentsInChildren<IPostEntityLoaded>(true);

                foreach(IPostEntityLoaded p in postLoads)
                {
                    p.PostEntityLoaded();
                }
            }
        }
    }

    public struct PrefabIDPair
    {
        [SerializeField]
        [ShowInInspector]
        [VerticalGroup("aligned")]
        [HideLabel]
        [ReadOnly]
        private GameObject prefabRefernece;

        [SerializeField]

        [ShowInInspector]
        [VerticalGroup("aligned")]
        [HideLabel]
        [ReadOnly]
        private int id;

        public int Id { get => id; set => id = value; }


        public GameObject PrefabRefernece { get => prefabRefernece; set => prefabRefernece = value; }
    }
}
