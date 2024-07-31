using Bloodthirst.Core.PersistantAsset;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Core.AdvancedPool
{
    public struct GlobalPoolEntry
    {
        public GameObject Prefab;
        public int InitialSpawnCount;
    }

    public class GlobalPoolData : SingletonScriptableObject<GlobalPoolData>
    {
        [SerializeField]
        private GlobalPoolEntry[] poolEntries;
        public IReadOnlyList<GlobalPoolEntry> PoolEntries => poolEntries;

        public void Add(GameObject prefab, int count)
        {
            using (ListPool<GlobalPoolEntry>.Get(out List<GlobalPoolEntry> tmp))
            {
                tmp.AddRange(poolEntries);

                bool found = false;
                for (int i = tmp.Count - 1; i >= 0; i--)
                {
                    GlobalPoolEntry e = tmp[i];

                    if (e.Prefab == prefab)
                    {
                        found = true;

                        if (e.InitialSpawnCount != count)
                        {
                            int old = e.InitialSpawnCount;
                            e.InitialSpawnCount = count;
                            Debug.Log($"Prefab {prefab.name} already found , changed initiali spawn count from {old} to {count}");

                            tmp[i] = e;
                        }

                        break;
                    }
                }

                if (!found)
                {
                    tmp.Add(new GlobalPoolEntry() { Prefab = prefab, InitialSpawnCount = count });
                }

                poolEntries = tmp.ToArray();
            }
        }

        public void Remove(GameObject prefab)
        {
            using (ListPool<GlobalPoolEntry>.Get(out List<GlobalPoolEntry> tmp))
            {
                tmp.AddRange(poolEntries);

                for (int i = tmp.Count - 1; i >= 0; i--)
                {
                    GlobalPoolEntry e = tmp[i];

                    if (e.Prefab == prefab)
                    {
                        tmp.RemoveAt(i);
                    }
                }

                poolEntries = tmp.ToArray();
            }
        }
    }
}
