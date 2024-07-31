using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class UnityPoolBase
    {
        /// <summary>
        /// Type of pooled object
        /// </summary>
        public abstract Type PooledType { get; }

        public abstract GameObject Get();

        public abstract void Return(GameObject t);

        protected abstract string GetName(UnityEngine.Object original, int index);
    }
}