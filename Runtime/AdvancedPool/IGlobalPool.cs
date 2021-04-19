using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool.Pools
{
    public interface IGlobalPool
    {
        T Get<T>() where T : Component;
        T Get<T>(Predicate<T> filter) where T : Component; 
        void Return<T>(T t) where T : Component;
        List<IPoolBehaviour> AllPools { get; }
        PoolBehaviour<T> GetByPrefab<T>(GameObject prefab) where T : Component;
        IPoolBehaviour GetByPrefab(GameObject prefab);
        PoolBehaviour<T> GetByType<T>() where T : Component;
        IEnumerable<PoolBehaviour<T>> GetAllByType<T>() where T : Component;

    }
}