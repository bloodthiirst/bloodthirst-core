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
}
