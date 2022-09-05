using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.BISDSystem
{

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

            foreach (Type l in loadTypes)
            {
                IGameStateLoader loader = (IGameStateLoader)Activator.CreateInstance(l);
                Loaders.Add(loader);
            }

            foreach (Type s in saveTypes)
            {
                IGameStateSaver saver = (IGameStateSaver)Activator.CreateInstance(s);
                Savers.Add(saver);
            }
        }

        [Button]
        public static Dictionary<ISavableInstanceProvider, List<ISavableGameSave>> SaveRuntimeState()
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
                IGameStateSaver saver = Savers.FirstOrDefault(s => s.From == savable.SavableStateType);

                // create the gamesave from the state
                ISavableGameSave gameData = saver.GetSave(savable.GetSavableState(), context);
                gameStates.Add(gameData);
            }

            // finally after all the states are grouped by instance
            // we save them with the key being a class that contains all the necessary info to spawn or load
            Dictionary<ISavableInstanceProvider, List<ISavableGameSave>> finalSaveData = savableToGamesaves.ToDictionary(kv => kv.Key.GetInstanceProvider(), kv => kv.Value);

            return finalSaveData;
        }

        public static List<GameObject> Load(GameStateSaveInstance gameData, bool withPostLoad)
        {
            List<SpawnInstanceIdPair> allSpawned = new List<SpawnInstanceIdPair>();

            LoadingContext context = new LoadingContext();

            Dictionary<GameObject, List<LoadingInfo>> loadingInfo = new Dictionary<GameObject, List<LoadingInfo>>();

            // for each entity
            foreach (KeyValuePair<ISavableInstanceProvider, List<ISavableGameSave>> kv in gameData.GameDatas)
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
                ISavableBehaviour[] savablesToLoad = spawned.GetComponentsInChildren<ISavableBehaviour>();

                /// adding states to instances
                for (int i = 0; i < savablesToLoad.Length; i++)
                {
                    ISavableBehaviour savableBhv = savablesToLoad[i];
                    ISavable savable = savableBhv.GetSavable();
                    LoadingInfo loading = loadingPerInstance.FirstOrDefault(ins => ins.State.GetType() == savable.SavableStateType);

                    loading.Instance = savable;

                    savable.ApplyState(loading.State);

                    context.AddInstance(savable);
                }

                allSpawned.Add(new SpawnInstanceIdPair() { SpawnedInstance = spawned, SpawnInfo = kv.Key });
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
            foreach (SpawnInstanceIdPair s in allSpawned)
            {
                s.SpawnInfo.PostStatesApplied(s.SpawnedInstance);
            }

            // after all entities are loaded
            if (withPostLoad)
            {
                foreach (SpawnInstanceIdPair s in allSpawned)
                {
                    IPostEntitiesLoaded[] postLoads = s.SpawnedInstance.GetComponentsInChildren<IPostEntitiesLoaded>(true);

                    foreach (IPostEntitiesLoaded p in postLoads)
                    {
                        p.PostEntitiesLoaded();
                    }
                }
            }

            return allSpawned.Select(p => p.SpawnedInstance).ToList();
        }
    }
}
