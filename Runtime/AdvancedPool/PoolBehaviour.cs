using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class PoolBehaviour<TObejct> : MonoBehaviour, IPoolBehaviour , IPostSceneLoadedPass where TObejct : Component
    {

        private static readonly Type type = typeof(TObejct);

        [SerializeField]
        protected Transform poolContainer = null;

        [SerializeField]
        protected TObejct prefab;
        
        [SerializeField]
        protected int poolCount;

        public TObejct Prefab
        {
            get => prefab;
            set => prefab = value;
        }

        GameObject IPoolBehaviour.Prefab
        {
            get => Prefab.gameObject;
            set => Prefab = value.GetComponent<TObejct>();
        }

        
        int IPoolBehaviour.Count { get => poolCount; set => poolCount = value; }

        private Pool<TObejct> _Pool;
        public Pool<TObejct> Pool
        {
            get
            {
                if (_Pool == null)
                {
                    _Pool = new Pool<TObejct>(poolContainer, prefab, poolCount);
                }
                return _Pool;
            }
        }

        Type IPoolBehaviour.Type => type;

        [SerializeField]
        [ReadOnly]
        private string prefabPath;
        public string PrefabPath { get => prefabPath; set => prefabPath = value; }

        void IPoolBehaviour.Initialize()
        {
            poolContainer = transform;
            _Pool = new Pool<TObejct>(poolContainer, prefab, poolCount);
        }

        void IPostSceneLoadedPass.DoScenePass()
        {
            Pool.InitializePool();
        }
    }
}
