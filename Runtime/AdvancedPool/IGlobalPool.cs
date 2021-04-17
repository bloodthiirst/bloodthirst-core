using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool.Pools
{
    public interface IGlobalPool
    {
        T Get<T>() where T : Component;
        T Get<T>(Predicate<T> filter) where T : Component; 
        void Return<T>(T t) where T : Component;


    }
}