using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool.Pools
{
    public interface IGlobalPool
    {
        List<IPoolBehaviour> AllPools { get; }
        IPoolBehaviour GetByPrefab(GameObject prefab);
        void SetupPools();
    }
}