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
        public List<object> states;
    }

    public class SaveLoadManager
    {
        private struct EntityStatePair
        {
            public GameObject entity;
            public object state;
        }

        private static List<IGameStateLoader> loaders = new List<IGameStateLoader>();

        private static List<IGameStateSaver> savers = new List<IGameStateSaver>();
        public static IReadOnlyList<IGameStateLoader> Loaders => loaders;
        public static IReadOnlyList<IGameStateSaver> Savers => savers;

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

            loaders.Clear();
            foreach (Type l in loadTypes)
            {
                IGameStateLoader loader = (IGameStateLoader)Activator.CreateInstance(l);
                loaders.Add(loader);
            }

            savers.Clear();
            foreach (Type s in saveTypes)
            {
                IGameStateSaver saver = (IGameStateSaver)Activator.CreateInstance(s);
                savers.Add(saver);
            }
        }


#if ODIN_INSPECTOR
        [Button]
#endif

        public static void SaveRuntimeState(IReadOnlyList<GameObject> gos, List<SavedEntityEntry> results)
        {
            SavingContext context = new SavingContext();

            // get all savables
            using (ListPool<ISavableIdentifier>.Get(out List<ISavableIdentifier> ids))
            {
                GameObjectUtils.GetAllComponents(ref ids, gos, false);

                // get all the savables and organize themm into key/value pairs of instance -> states
                foreach (ISavableIdentifier id in ids)
                {
                    Component casted = (Component)id;
                    Assert.IsNotNull(casted);

                    // gameObject of the entity to save
                    GameObject saveableGo = casted.gameObject;

                    List<object> statesList = new List<object>();

                    // get the correct saver
                    foreach (IGameStateSaver saver in savers)
                    {
                        if (!saver.CanSave(saveableGo)) { continue; }

                        // create the gamesave from the state
                        object saveData = saver.GetSave(saveableGo, context);
                        Assert.IsNotNull(saveData);

                        statesList.Add(saveData);
                    }

                    // finally after all the states are saved for this entity
                    // we save them with the key being a class that contains all the necessary info to spawn or load
                    ISavableInstanceProvider instanceProvider = id.GetInstanceProvider();

                    results.Add(new SavedEntityEntry()
                    {
                        instanceProvider = instanceProvider,
                        states = statesList
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

            using (ListPool<IPostEntitiesLoaded>.Get(out List<IPostEntitiesLoaded> postLoads))
            using (ListPool<EntityIdentifier>.Get(out List<EntityIdentifier> ids))
            using (DictionaryPool<IGameStateLoader, EntityStatePair>.Get(out Dictionary<IGameStateLoader, EntityStatePair> entityStatePairs))
            {
                // for each entity
                foreach (SavedEntityEntry kv in savedEntities)
                {
                    // get the id component of the entity
                    GameObject spawned = kv.instanceProvider.GetInstanceToInject();

                    EntityIdentifier id = spawned.GetComponent<EntityIdentifier>();
                    Assert.IsNotNull(id);

                    ids.Add(id);

                    EntitySpawner.IntializeEntityIdentifier(id);

                    EntitySpawner.IntializeInstances(id);

                    context.loadedEntities.Add(id);

                    foreach (object saveState in kv.states)
                    {
                        bool foundLoader = false;

                        foreach (IGameStateLoader loader in loaders)
                        {
                            if (!loader.CanLoad(spawned, saveState)) { continue; }

                            foundLoader = true;
                            object gameState = loader.ApplyState(spawned, saveState, context);

                            entityStatePairs.Add(loader, new EntityStatePair() { entity = spawned, state = gameState });
                        }

                        Assert.IsTrue(foundLoader);
                    }

                }

                // link refs
                foreach (KeyValuePair<IGameStateLoader, EntityStatePair> kv in entityStatePairs)
                {
                    kv.Key.LinkReferences(kv.Value.entity, kv.Value.state, context);
                }

                // after all entities are loaded
                if (withPostLoad)
                {
                    foreach (EntityIdentifier id in ids)
                    {
                        postLoads.Clear();
                        id.GetComponentsInChildren(true, postLoads);

                        foreach (IPostEntitiesLoaded p in postLoads)
                        {
                            p.PostEntitiesLoaded();
                        }
                    }
                }

                foreach (EntityIdentifier id in ids)
                {
                    EntitySpawner.PostInitialize(id);
                }

                foreach (EntityIdentifier id in ids)
                {
                    spawnedEntities.Add(id.gameObject);
                }
            }
        }
    }
}
