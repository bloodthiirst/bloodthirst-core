using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public interface IPoolBehaviour
    {
        GameObject Prefab { get; set; }
        int Count { get; set; }
        void Initialize();
        
    }
}
