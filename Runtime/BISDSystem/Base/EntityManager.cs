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

            foreach (Type l in loadTypes)
            {
                IEntityLoader loader = (IEntityLoader)Activator.CreateInstance(l);
                Loaders.Add(loader);
            }

            foreach (Type s in saveTypes)
            {
                IEntitySaver saver = (IEntitySaver)Activator.CreateInstance(s);
                Savers.Add(saver);
            }
        }

        [Button]
        public static Dictionary<PrefabIDPair, List<IEntityGameSave>> GetAllEntityStates()
        {
            Dictionary<PrefabIDPair, List<IEntityGameSave>> saveData = new Dictionary<PrefabIDPair, List<IEntityGameSave>>();

            SavingContext context = new SavingContext();

            foreach (IEnumerable set in _AllInstanceSets)
            {
                foreach (IEntityInstance ins in set)
                {
                    // if doesn't have id , then we don't save it
                    if (ins.EntityIdentifier == null)
                        continue;

                    PrefabIDPair key = new PrefabIDPair() { Id = ins.EntityIdentifier.Id, PrefabRefernece = ins.EntityIdentifier.PrefabReferenceData.PrefabReference };


                    if (!saveData.TryGetValue(key, out List<IEntityGameSave> gameStates))
                    {
                        gameStates = new List<IEntityGameSave>();
                        saveData.Add(key, gameStates);
                    }

                    IEntitySaver saver = Savers.FirstOrDefault(s => s.From == ins.StateType);
                    IEntityGameSave gameData = saver.GetSave(ins.State, context);
                    gameStates.Add(gameData);
                }
            }

            return saveData;
        }

        // TODO : work on loading
        [Button]
        public static List<EntityIdentifier> Load(BISDGameStateData gameData, bool withPostLoad)
        {
            EntitySpawner spawner = UnityEngine.Object.FindObjectOfType<EntitySpawner>();

            return Load(gameData, spawner, withPostLoad);
        }

        public class LoadingInfo
        {
            public IEntityLoader Loader { get; set; }
            public IEntityGameSave GameData { get; set; }
            public IEntityState State { get; set; }
            public IEntityInstance Instance { get; set; }
        }

        public static List<EntityIdentifier> Load(BISDGameStateData gameData, EntitySpawner spawner, bool withPostLoad)
        {
            List<EntityIdentifier> spawned = new List<EntityIdentifier>();

            LoadingContext context = new LoadingContext();

            Dictionary<PrefabIDPair, List<LoadingInfo>> loadingInfo = new Dictionary<PrefabIDPair, List<LoadingInfo>>();

            // for each entity
            foreach (KeyValuePair<PrefabIDPair, List<IEntityGameSave>> kv in gameData.GameDatas)
            {

                List<LoadingInfo> loadingPerInstance = new List<LoadingInfo>();

                // get the id component of the entity
                EntityIdentifier prefabId = kv.Key.PrefabRefernece.GetComponent<EntityIdentifier>();

                // copy the saved gameStates
                List<IEntityGameSave> gameSavesForEntity = kv.Value.ToList();

                // states to inject into the instance
                List<IEntityState> statesForInstance = new List<IEntityState>();


                // generate the runtime state from the saves
                // get the loader
                // get the gameData
                // get the state
                foreach (IEntityGameSave g in gameSavesForEntity)
                {
                    LoadingInfo loading = new LoadingInfo();

                    IEntityLoader stateLoader = Loaders.FirstOrDefault(l => l.From == g.GetType());

                    IEntityState state = stateLoader.GetState(g, context);

                    statesForInstance.Add(state);

                    // cache the data
                    loading.GameData = g;
                    loading.State = state;
                    loading.Loader = stateLoader;

                    loadingPerInstance.Add(loading);
                }

                loadingInfo.Add(kv.Key, loadingPerInstance);

                // spawn the entity with the preloaded state
                EntityIdentifier loadedEntity = spawner.SpawnEntity<EntityIdentifier>(e => e.EntityType == prefabId.EntityType, statesForInstance);

                IBehaviourInstance[] bismBehaviours = loadedEntity.GetComponentsInChildren<IBehaviourInstance>();

                /// assing states to instances
                foreach (IBehaviourInstance bisdBehaviour in bismBehaviours)
                {
                    LoadingInfo loading = loadingPerInstance.FirstOrDefault(i => i.State.GetType() == bisdBehaviour.Instance.StateType);

                    loading.Instance = bisdBehaviour.Instance;

                    bisdBehaviour.Instance.State = loading.State;

                    context.AddInstance(bisdBehaviour.Instance);
                }

                spawned.Add(loadedEntity);
            }

            // link refs
            foreach (KeyValuePair<PrefabIDPair, List<LoadingInfo>> kv in loadingInfo)
            {
                foreach(LoadingInfo v in kv.Value)
                {
                    v.Loader.LinkReferences(v.GameData, v.State, context);
                }
            }

            // after all entities are loaded
            if (withPostLoad)
            {
                foreach (EntityIdentifier s in spawned)
                {
                    IPostEntitiesLoaded[] postLoads = s.GetComponentsInChildren<IPostEntitiesLoaded>(true);

                    foreach (IPostEntitiesLoaded p in postLoads)
                    {
                        p.PostEntitiesLoaded();
                    }
                }
            }

            return loadingInfo.SelectMany(kv => kv.Value).Select( i => i.Instance.EntityIdentifier).ToList();
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
