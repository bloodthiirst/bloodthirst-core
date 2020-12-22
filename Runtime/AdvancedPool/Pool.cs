using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class Pool
    {
        /// <summary>
        /// Type of pooled object
        /// </summary>
        public abstract Type PooledType { get; }
        protected abstract string GetName(MonoBehaviour original , int index);
    }
    public class Pool<TObject>: Pool where TObject : MonoBehaviour
    {
        private static readonly Type Type = typeof(TObject);
        public override Type PooledType => Type;

        [SerializeField]
        private TObject Prefab;

        [SerializeField]
        private int PoolCount { get; set; }

        private List<TObject> _pool;

        private Transform PoolTransform { get; set; }

        private List<TObject> _Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new List<TObject>();
                }

                return _pool;
            }
        }


        public Pool(Transform transform, TObject prefab, int poolCount)
        {
            PoolTransform = transform;
            Prefab = prefab;
            PoolCount = poolCount;
        }

        public void InitializePool()
        {
            AddToPool(PoolCount);
        }

        private void AddToPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                TObject inst = UnityEngine.Object.Instantiate(Prefab, PoolTransform);
                inst.name = GetName(Prefab, i);
                Return(inst);
                _Pool.Add(inst);
            }
        }

        public TObject Get(Predicate<TObject> filter)
        {
            CheckPoolSize();

            for (int i = 0; i < _Pool.Count; i++)
            {
                TObject ins = _Pool[i];

                if (!filter(ins))
                    continue;

                _Pool.RemoveAt(i);
                ins.gameObject.SetActive(true);

                IOnSpawn[] array = ins.GetComponentsInChildren<IOnSpawn>(true);
                for (int j = 0; j < array.Length; j++)
                {
                    IOnSpawn onSpawn = array[j];
                    onSpawn.OnSpawn();
                }

                return ins;
            }

            return null;

        }

        private void CheckPoolSize()
        {
            // re-instantiate if no more objects are available
            if (_Pool.Count == 0)
            {
                AddToPool(PoolCount);
            }
        }

        public TObject Get()
        {
            CheckPoolSize();

            TObject obj = _Pool[0];
            _Pool.RemoveAt(0);

            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);

            IOnSpawn[] array = obj.GetComponentsInChildren<IOnSpawn>(true);
            for (int i = 0; i < array.Length; i++)
            {
                IOnSpawn onSpawn = array[i];
                onSpawn.OnSpawn();
            }

            return obj;
        }

        public void Return(TObject t)
        {
            if (t == null)
            {
                Debug.Assert(t == null);
                return;
            }

            IOnDespawn[] array = t.GetComponentsInChildren<IOnDespawn>(true);
            for (int i = 0; i < array.Length; i++)
            {
                IOnDespawn onSpawn = array[i];
                onSpawn.OnDespawn();
            }

            t.transform.SetParent(PoolTransform);
            t.gameObject.SetActive(false);
            _Pool.Add(t);
        }

        protected override string GetName(MonoBehaviour original , int index)
        {
            return $"{original.name} - {index}";
        }
    }
}