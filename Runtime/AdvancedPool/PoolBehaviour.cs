using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class PoolBehaviour<TObject> : MonoBehaviour, IPoolBehaviour , IBeforeAllScenesInitializationPass where TObject : Component
    {

        private static readonly Type type = typeof(TObject);

        [SerializeField]
        protected Transform poolContainer = null;

        [SerializeField]
        protected TObject prefab;

        [SerializeField]
        protected int poolCount;

        public TObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }

        GameObject IPoolBehaviour.Prefab
        {
            get => Prefab.gameObject;
            set => Prefab = value.GetComponent<TObject>();
        }


        int IPoolBehaviour.Count { get => poolCount; set => poolCount = value; }

        private Pool<TObject> _Pool;
        public Pool<TObject> Pool
        {
            get
            {
                if (_Pool == null)
                {
                    _Pool = new Pool<TObject>(poolContainer, prefab, poolCount);
                }
                return _Pool;
            }
        }

        Type IPoolBehaviour.Type => type;

        [SerializeField]
        [ReadOnly]
        private string prefabPath;
        public string PrefabPath { get => prefabPath; set => prefabPath = value; }
        string IPoolBehaviour.PrefabPath { get => prefabPath; set => prefabPath = value; }

        Pool IPoolBehaviour.Pool => Pool;

        void IPoolBehaviour.Initialize()
        {
            poolContainer = transform;
            _Pool = new Pool<TObject>(poolContainer, prefab, poolCount);
        }

        private void InitializePool()
        {
            Pool.InitializePool();
        }

        void IBeforeAllScenesInitializationPass.Execute()
        {
            InitializePool();
            Debug.Log($"[POOL LOADED] Pool initialized for prefab <{prefab.name}>");
        }
    }
}
