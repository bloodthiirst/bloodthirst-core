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
        void Initialize();

        Pool Pool { get; }

    }

    public interface IPoolBehaviour<T> : IPoolBehaviour where T : Component
    {
        Pool<T> PoolWithType { get; }

        public T PrefabWithType { get; }

    }
}
