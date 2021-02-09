using Bloodthirst.Scripts.Core.GamePassInitiator;
using System;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class PoolBehaviour<TObejct> : MonoBehaviour, IPoolBehaviour , IPostSceneLoadedPass where TObejct : MonoBehaviour
    {

        [SerializeField]
        private Transform poolContainer = null;

        [SerializeField]
        private TObejct prefab;

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

        [SerializeField]
        private int poolCount;
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
