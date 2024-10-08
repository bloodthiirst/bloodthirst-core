using Bloodthirst.Core.Utils;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Bloodthirst.Core.BISDSystem
{

    public struct SavedEntityEntry
    {
        public ISavableInstanceProvider instanceProvider;
        public List<ISavableGameSave> states;
    }

    public class SaveLoadManager
    {
        private struct SpawnInstanceIdPair
        {
            public GameObject SpawnedInstance { get; set; }
            public ISavableInstanceProvider SpawnInfo { get; set; }
        }

        private static List<IGameStateLoader> Loaders { get; set; } = new List<IGameStateLoader>();

        private static List<IGameStateSaver> Savers { get; set; } = new List<IGameStateSaver>();

        public static void Initialize()
        {
            IEnumerable<Type> loadTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IGameStateLoader)));

            IEnumerable<Type> saveTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IGameStateSaver)));

            Loaders.Clear();
            foreach (Type l in loadTypes)
            {
                IGameStateLoader loader = (IGameStateLoader)Activator.CreateInstance(l);
                Loaders.Add(loader);
            }

            Savers.Clear();
            foreach (Type s in saveTypes)
            {
                IGameStateSaver saver = (IGameStateSaver)Activator.CreateInstance(s);
                Savers.Add(saver);
            }
        }


#if ODIN_INSPECTOR
        [Button]
#endif

        public static void SaveRuntimeState(IReadOnlyList<GameObject> gos, List<SavedEntityEntry> results)
        {
            SavingContext context = new SavingContext();

            // get all savables
            using (DictionaryPool<ISavableIdentifier, List<ISavableGameSave>>.Get(out Dictionary<ISavableIdentifier, List<ISavableGameSave>> savableToGamesaves))
            using (ListPool<ISavableBehaviour>.Get(out List<ISavableBehaviour> savables))
            using (ListPool<ISavableIdentifier>.Get(out List<ISavableIdentifier> ids))
            {
                GameObjectUtils.GetAllComponents(ref savables, gos, false);
                GameObjectUtils.GetAllComponents(ref ids, gos, false);

                // add the IDs
                foreach(ISavableIdentifier id in ids )
                {
                    savableToGamesaves.Add(id, new List<ISavableGameSave>());
                }

                // get all the savables and organize themm into key/value pairs of instance -> states
                foreach (ISavableBehaviour saveBhv in savables)
                {
                    Component casted = (Component)saveBhv;
                    Assert.IsNotNull(casted);

                    ISavableIdentifier id = casted.GetComponent<ISavableIdentifier>();
                    Assert.IsNotNull(id);

                    ISavable savable = saveBhv.GetSavable();

                    if (!savableToGamesaves.TryGetValue(id, out List<ISavableGameSave> gameStates))
                    {
                        gameStates = new List<ISavableGameSave>();
                        savableToGamesaves.Add(id, gameStates);
                    }

                    // get the correct saver
                    IGameStateSaver saver = Savers.FirstOrDefault(s => s.From == savable.SavableStateType);
                    Assert.IsNotNull(saver);

                    // create the gamesave from the state
                    ISavableGameSave gameData = saver.GetSave(savable.GetSavableState(), context);
                    //Assert.IsNotNull(gameData);

                    gameStates.Add(gameData);
                }

                // finally after all the states are grouped by instance
                // we save them with the key being a class that contains all the necessary info to spawn or load
                foreach (KeyValuePair<ISavableIdentifier, List<ISavableGameSave>> savable in savableToGamesaves)
                {
                    ISavableInstanceProvider instanceProvider = savable.Key.GetInstanceProvider();
                    List<ISavableGameSave> states = savable.Value;

                    results.Add(new SavedEntityEntry()
                    {
                        instanceProvider = instanceProvider,
                        states = states
                    });
                }
            }
        }
        
        public static void SaveRuntimeState(List<SavedEntityEntry> results)
        {
            using (ListPool<GameObject>.Get(out var gos))
            {
                GameObjectUtils.GetAllRootGameObjects(gos);
                SaveRuntimeState(gos, results);
            }
        }

        public static void LoadEntities(IReadOnlyList<SavedEntityEntry> savedEntities, List<GameObject> spawnedEntities, bool withPostLoad)
        {
            Assert.IsTrue(spawnedEntities.Count == 0);

            LoadingContext context = new LoadingContext();

            using (ListPool<SpawnInstanceIdPair>.Get(out List<SpawnInstanceIdPair> allSpawned))
            using (ListPool<ISavableGameSave>.Get(out List<ISavableGameSave> gameSavesForEntity))
            using (ListPool<ISavableState>.Get(out List<ISavableState> statesForInstance))
            using (ListPool<ISavableBehaviour>.Get(out List<ISavableBehaviour> savablesToLoad))
            using (ListPool<IPostEntitiesLoaded>.Get(out List<IPostEntitiesLoaded> postLoads))
            using (DictionaryPool<GameObject, List<LoadingInfo>>.Get(out var loadingInfo))
            {
                // for each entity
                foreach (SavedEntityEntry kv in savedEntities)
                {
                    List<LoadingInfo> loadingPerInstance = new List<LoadingInfo>();

                    // get the id component of the entity
                    GameObject spawned = kv.instanceProvider.GetInstanceToInject();

                    EntityIdentifier id = spawned.GetComponent<EntityIdentifier>();
                    Assert.IsNotNull(id);

                    EntitySpawner.IntializeEntityIdentifier(id);

                    // TODO : set state with the instance
                    EntitySpawner.IntializeInstances(id);

                    // copy the saved gameStates
                    gameSavesForEntity.Clear();
                    gameSavesForEntity.AddRange(kv.states);

                    // states to inject into the instance
                    statesForInstance.Clear();

                    // generate the runtime state from the saves
                    // get the loader
                    // get the gameData
                    // get the state
                    for (int i = 0; i < gameSavesForEntity.Count; i++)
                    {
                        ISavableGameSave g = gameSavesForEntity[i];
                        LoadingInfo loading = new LoadingInfo();

                        IGameStateLoader stateLoader = Loaders.FirstOrDefault(l => l.From == g.GetType());

                        ISavableState state = stateLoader.GetState(g, context);

                        statesForInstance.Add(state);

                        // cache the data
                        loading.GameData = g;
                        loading.State = state;
                        loading.Loader = stateLoader;

                        loadingPerInstance.Add(loading);
                    }

                    loadingInfo.Add(spawned, loadingPerInstance);

                    // spawn the entity with the preloaded state
                    savablesToLoad.Clear();
                    spawned.GetComponentsInChildren(true, savablesToLoad);

                    /// adding states to instances
                    for (int i = 0; i < savablesToLoad.Count; i++)
                    {
                        ISavableBehaviour savableBhv = savablesToLoad[i];
                        ISavable savable = savableBhv.GetSavable();
                        LoadingInfo loading = loadingPerInstance.FirstOrDefault(ins => ins.State.GetType() == savable.SavableStateType);

                        loading.Instance = savable;

                        // this line assigns the state to the instance
                        savable.ApplyState(loading.State);

                        context.AddInstance(savable);
                    }

                    allSpawned.Add(new SpawnInstanceIdPair() { SpawnedInstance = spawned, SpawnInfo = kv.instanceProvider });
                }

                // link refs
                foreach (KeyValuePair<GameObject, List<LoadingInfo>> kv in loadingInfo)
                {
                    foreach (LoadingInfo v in kv.Value)
                    {
                        v.Loader.LinkReferences(v, context);
                    }
                }

                // after all entities are loaded
                if (withPostLoad)
                {
                    foreach (SpawnInstanceIdPair s in allSpawned)
                    {
                        postLoads.Clear();
                        s.SpawnedInstance.GetComponentsInChildren(true, postLoads);

                        foreach (IPostEntitiesLoaded p in postLoads)
                        {
                            p.PostEntitiesLoaded();
                        }
                    }
                }

                foreach (SpawnInstanceIdPair s in allSpawned)
                {
                    EntityIdentifier id = s.SpawnedInstance.GetComponent<EntityIdentifier>();
                    Assert.IsNotNull(id);

                    EntitySpawner.PostInitialize(id);
                }

                foreach (SpawnInstanceIdPair s in allSpawned)
                {
                    spawnedEntities.Add(s.SpawnedInstance);
                }
            }
        }
    }
}
