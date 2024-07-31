using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Linq;
using System;
using UnityEngine.SceneManagement;


namespace Bloodthirst.Core.AdvancedPool.Pools
{   
    /// <summary>
    /// <para> This class is an auto-generated pools of the prefabs marked with the <see cref="GeneratePool"></see></para>
    /// <para> The pools are created in the "PoolScene" , a scene that is specifically made to contain thses auto-generated pools</para>
    /// <para> The pool generation is done by the <see cref="PoolGenerator"> class</see></para>
    /// </summary>
    public class GlobalPoolContainer : MonoBehaviour , IGlobalPool
    {
        [SerializeField]
        private GlobalPoolData poolData;
        public GlobalPoolData PoolData 
        {
            get => poolData;
            set => poolData = value;
        }
        
        private List<IPoolBehaviour> allPools = new List<IPoolBehaviour>();

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public List<IPoolBehaviour> AllPools => allPools;

        public IPoolBehaviour<T> GetByPrefab<T>(GameObject prefab) where T : Component
        {
            IPoolBehaviour pool = AllPools.Where(p => p.Type == typeof(T)).FirstOrDefault(p => p.Prefab == prefab);

            return (IPoolBehaviour<T>) pool;
        }

        public IPoolBehaviour GetByPrefab(GameObject prefab)
        {
            IPoolBehaviour pool = AllPools.FirstOrDefault(p => p.Prefab == prefab);

            return pool;
        }

        public IPoolBehaviour<T> GetByType<T>() where T : Component
        {
            IPoolBehaviour<T> pool = AllPools.FirstOrDefault(p => p.Type == typeof(T)) as IPoolBehaviour<T>;

            return pool;
        }

        public IEnumerable<IPoolBehaviour<T>> GetAllByType<T>() where T : Component
        {
            return AllPools
                .Where(p => p.Type == typeof(T))
                .Cast<IPoolBehaviour<T>>();
        }

        public void SetupPools()
        {
            foreach(GlobalPoolEntry e in poolData.PoolEntries)
            {
                GameObject go = new GameObject($"Pool => {e.Prefab}");
                GameObjectPoolBehaviour pool = go.AddComponent<GameObjectPoolBehaviour>();
                pool.Prefab = e.Prefab;
                pool.Count = e.InitialSpawnCount;
  
                allPools.Add(pool);

                SceneManager.MoveGameObjectToScene(go, this.gameObject.scene);

                pool.Initialize();
                pool.Populate();
            }
        }
    }
}
