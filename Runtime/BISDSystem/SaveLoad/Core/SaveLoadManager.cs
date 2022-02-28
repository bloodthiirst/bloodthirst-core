using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.BISDSystem
{
    [InitializeOnLoad]
    public class SaveLoadManager
    {
        private static List<IEntityLoader> Loaders { get; set; } = new List<IEntityLoader>();

        private static List<IEntitySaver> Savers { get; set; } = new List<IEntitySaver>();

        static SaveLoadManager()
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
        public static Dictionary<ISavableSpawnInfo, List<ISavableGameSave>> SaveRuntimeState()
        {
            Dictionary<ISavableIdentifier, List<ISavableGameSave>> savableToGamesaves = new Dictionary<ISavableIdentifier, List<ISavableGameSave>>();

            SavingContext context = new SavingContext();

            // get all savables
            List<ISavableBehaviour> savables = new List<ISavableBehaviour>();
            GameObjectUtils.GetAllComponents(ref savables, false);

            // get all the savables and organize themm into key/value pairs of instance -> states
            foreach (ISavableBehaviour saveBhv in savables)
            {
                ISavable savable = saveBhv.GetSavable();
                ISavableIdentifier id = savable.GetIdentifierInfo();

                Assert.IsNotNull(id);

                if (!savableToGamesaves.TryGetValue(id, out List<ISavableGameSave> gameStates))
                {
                    gameStates = new List<ISavableGameSave>();
                    savableToGamesaves.Add(id, gameStates);
                }

                // get the correct saver
                IEntitySaver saver = Savers.FirstOrDefault(s => s.From == savable.SavableStateType);

                // create the gamesave from the state
                ISavableGameSave gameData = saver.GetSave(savable.GetSavableState(), context);
                gameStates.Add(gameData);
            }

            // finally after all the states are grouped by instance
            // we save them with the key being a class that contains all the necessary info to spawn or load
            Dictionary<ISavableSpawnInfo, List<ISavableGameSave>> finalSaveData = savableToGamesaves.ToDictionary(kv => kv.Key.GetSpawnInfo(), kv => kv.Value);

            return finalSaveData;
        }

        // TODO : work on loading
        [Button]
        public static List<GameObject> Load(GameStateSaveInstance gameData, bool withPostLoad)
        {
            EntitySpawner spawner = UnityEngine.Object.FindObjectOfType<EntitySpawner>();

            return Load(gameData, spawner, withPostLoad);
        }
        
        private struct SpawnInfoInstancePair
        {
            public GameObject SpawnedInstance { get; set; }
            public ISavableSpawnInfo SpawnInfo { get; set; }
        }

        public static List<GameObject> Load(GameStateSaveInstance gameData, EntitySpawner spawner, bool withPostLoad)
        {
            List<SpawnInfoInstancePair> allSpawned = new List<SpawnInfoInstancePair>();

            LoadingContext context = new LoadingContext();

            Dictionary<GameObject, List<LoadingInfo>> loadingInfo = new Dictionary<GameObject, List<LoadingInfo>>();

            // for each entity
            foreach (KeyValuePair<ISavableSpawnInfo, List<ISavableGameSave>> kv in gameData.GameDatas)
            {

                List<LoadingInfo> loadingPerInstance = new List<LoadingInfo>();

                // get the id component of the entity
                GameObject spawned = kv.Key.GetInstanceToInject();

                // copy the saved gameStates
                List<ISavableGameSave> gameSavesForEntity = kv.Value.ToList();

                // states to inject into the instance
                List<ISavableState> statesForInstance = new List<ISavableState>();

                // generate the runtime state from the saves
                // get the loader
                // get the gameData
                // get the state
                foreach (ISavableGameSave g in gameSavesForEntity)
                {
                    LoadingInfo loading = new LoadingInfo();

                    IEntityLoader stateLoader = Loaders.FirstOrDefault(l => l.From == g.GetType());

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
                //EntityIdentifier loadedEntity = spawner.SpawnEntity<EntityIdentifier>(e => e.EntityType == prefabId.EntityType, statesForInstance);

                ISavableBehaviour[] savablesToLoad = spawned.GetComponentsInChildren<ISavableBehaviour>();

                /// adding states to instances
                foreach (ISavableBehaviour savableBhv in savablesToLoad)
                {
                    ISavable savable = savableBhv.GetSavable();
                    LoadingInfo loading = loadingPerInstance.FirstOrDefault(i => i.State.GetType() == savable.SavableStateType);

                    loading.Instance = savable;

                    savable.ApplyState(loading.State);

                    context.AddInstance(savable);
                }

                allSpawned.Add( new SpawnInfoInstancePair() { SpawnedInstance = spawned, SpawnInfo = kv.Key });
            }

            // link refs
            foreach (KeyValuePair<GameObject, List<LoadingInfo>> kv in loadingInfo)
            {
                foreach (LoadingInfo v in kv.Value)
                {
                    v.Loader.LinkReferences(v.GameData, v.State, context);
                }
            }

            // post instance applied
            // for example : do the whole instance linking/refresh state for BISD
            foreach (SpawnInfoInstancePair s in allSpawned)
            {
                s.SpawnInfo.PostStatesApplied(s.SpawnedInstance);
            }

            // after all entities are loaded
            if (withPostLoad)
            {
                foreach (SpawnInfoInstancePair s in allSpawned)
                {
                    IPostEntitiesLoaded[] postLoads = s.SpawnedInstance.GetComponentsInChildren<IPostEntitiesLoaded>(true);

                    foreach (IPostEntitiesLoaded p in postLoads)
                    {
                        p.PostEntitiesLoaded();
                    }
                }
            }

            return allSpawned.Select( p => p.SpawnedInstance).ToList();
        }
    }
}
