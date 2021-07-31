using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [InitializeOnLoad]
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

        private static List<IEntityLoader> Loaders { get; set; } = new List<IEntityLoader>();

        private static List<IEntitySaver> Savers { get; set; } = new List<IEntitySaver>();

        static EntityManager()
        {
            IEnumerable<Type> loadTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IEntityLoader)));
            
            IEnumerable<Type> saveTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IEntitySaver)));

            foreach(Type l in loadTypes)
            {
                IEntityLoader loader =(IEntityLoader) Activator.CreateInstance(l);
                Loaders.Add(loader);
            }
            
            foreach(Type s in saveTypes)
            {
                IEntitySaver saver =(IEntitySaver) Activator.CreateInstance(s);
                Savers.Add(saver);
            }
        }

        [Button]
        public static Dictionary<PrefabIDPair, List<IEntityGameData>> GetAllEntityStates()
        {
            Dictionary<PrefabIDPair, List<IEntityGameData>> saveData = new Dictionary<PrefabIDPair, List<IEntityGameData>>();

            SavingContext context = new SavingContext();

            foreach (IEnumerable set in _AllInstanceSets)
            {
                foreach (IEntityInstance ins in set)
                {
                    // if doesn't have id , then we don't save it
                    if (ins.EntityIdentifier == null)
                        continue;

                    PrefabIDPair key = new PrefabIDPair() { Id = ins.EntityIdentifier.Id, PrefabRefernece = ins.EntityIdentifier.PrefabReferenceData.PrefabReference };


                    if (!saveData.TryGetValue(key, out List<IEntityGameData> gameStates))
                    {
                        gameStates = new List<IEntityGameData>();
                        saveData.Add(key, gameStates);
                    }

                    IEntitySaver saver = Savers.FirstOrDefault(s => s.From == ins.StateType);
                    IEntityGameData gameData = saver.GetSave(ins.State , context);
                    gameStates.Add(gameData);
                }
            }

            return saveData;
        }

        // TODO : work on loading
        [Button]
        public static List<EntityIdentifier> Load(BISDGameStateData gameData , bool withPostLoad)
        {
            EntitySpawner spawner = UnityEngine.Object.FindObjectOfType<EntitySpawner>();

            return Load(gameData, spawner , withPostLoad);
        }

        public static List<EntityIdentifier> Load(BISDGameStateData gameData , EntitySpawner spawner , bool withPostLoad)
        {
            List<EntityIdentifier> spawned = new List<EntityIdentifier>();

            LoadingContext context = new LoadingContext();

            foreach (KeyValuePair<PrefabIDPair, List<IEntityGameData>> kv in gameData.GameDatas)
            {
                // get the id component of the entity
                EntityIdentifier prefabId = kv.Key.PrefabRefernece.GetComponent<EntityIdentifier>();

                // copy the preloaded states
                List<IEntityGameData> gameDatasForEntity = kv.Value.ToList();

                List<IEntityState> statesFromLoad = new List<IEntityState>();

                foreach(IEntityGameData s in gameDatasForEntity)
                {
                    IEntityLoader stateLoader = Loaders.FirstOrDefault(l => l.From == s.GetType());

                    IEntityState state = stateLoader.GetState(s, context);

                    statesFromLoad.Add(state);
                }

                // spawn the entity with the preloaded data
                EntityIdentifier loadedEntity = spawner.SpawnEntity<EntityIdentifier>(e => e.EntityType == prefabId.EntityType , statesFromLoad);

                if (withPostLoad)
                {
                    spawned.Add(loadedEntity);
                }


                foreach (IEntityInstance loadable in loadedEntity.GetComponentsInChildren<IEntityInstance>())
                {
                    for (int i = statesFromLoad.Count - 1; i > -1; i--)
                    {
                        IEntityState currState = statesFromLoad[i];

                        if (loadable.StateType == currState.GetType())
                        {
                            loadable.State = currState;
                            statesFromLoad.RemoveAt(i);
                            break;
                        }
                    }
                }

            }

            if (withPostLoad)
            {

                foreach (EntityIdentifier s in spawned)
                {
                    IPostEntityLoaded[] postLoads = s.GetComponentsInChildren<IPostEntityLoaded>(true);

                    foreach (IPostEntityLoaded p in postLoads)
                    {
                        p.PostEntityLoaded();
                    }
                }
            }

            return spawned;
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
