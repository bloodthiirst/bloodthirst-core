using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public interface IPoolBehaviour
    {
        Type Type { get; }
        GameObject Prefab { get; set; }
        string PrefabPath { get; set; }
        int Count { get; set; }
        UnityPoolBase Pool { get; }
        void Initialize();
        void Populate();
    }

    public interface IPoolBehaviour<T> : IPoolBehaviour where T : Component
    {
        UnityPool<T> PoolWithType { get; }

        public T PrefabWithType { get; }

    }
}
