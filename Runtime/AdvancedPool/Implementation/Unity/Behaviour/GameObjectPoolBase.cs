#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public class GameObjectPoolBase: UnityPoolBase
    {
        private static readonly Type Type = typeof(GameObject);
        public override Type PooledType => Type;

        [SerializeField]
        private GameObject Prefab;

        [SerializeField]
        private int PoolCount { get; set; }

        private List<GameObject> _pool;

        private List<GameObject> _usedInstances;

        public List<GameObject> UsedInstances => _UsedInstances;

        private List<GameObject> _UsedInstances
        {
            get
            {
                if (_usedInstances == null)
                {
                    _usedInstances = new List<GameObject>();
                }

                return _usedInstances;
            }
        }

        private Transform PoolTransform { get; set; }

        private List<GameObject> _Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new List<GameObject>();
                }

                return _pool;
            }
        }


        public GameObjectPoolBase(Transform transform, GameObject prefab, int poolCount)
        {
            PoolTransform = transform;
            Prefab = prefab;
            PoolCount = poolCount;
        }

        public void PopulatePool()
        {
            AddToPool(PoolCount);
        }

        private void AddToPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject inst = UnityEngine.Object.Instantiate(Prefab, PoolTransform);
                inst.name = GetName(Prefab, i);
                inst.gameObject.SetActive(false);
                ReturnWithoutCallbacks(inst);
            }
        }

        public override void Return(GameObject t)
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
            _UsedInstances.Remove(t);
        }



        public GameObject Get<T>(Predicate<GameObject> filter)
        {
            CheckPoolSize();

            for (int i = 0; i < _Pool.Count; i++)
            {
                GameObject ins = _Pool[i];

                if (!filter(ins))
                    continue;

                _Pool.RemoveAt(i);
                _UsedInstances.Add(ins);

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

        public override GameObject Get()
        {
            CheckPoolSize();

            GameObject obj = _Pool[0];

            _Pool.RemoveAt(0);
            _UsedInstances.Add(obj);

            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);

            IOnSpawn[] array = obj.GetComponentsInChildren<IOnSpawn>(true);

            for (int i = 0; i < array.Length; i++)
            {
                IOnSpawn onSpawn = array[i];
                onSpawn.OnSpawn();
            }

            return obj.gameObject;
        }

        public GameObject GetTyped()
        {
            CheckPoolSize();

            GameObject obj = _Pool[0];

            _Pool.RemoveAt(0);
            _UsedInstances.Add(obj);

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

        private void ReturnWithoutCallbacks(GameObject t)
        {
            t.transform.SetParent(PoolTransform);
            t.gameObject.SetActive(false);
            _Pool.Add(t);
            _UsedInstances.Remove(t);
        }

        protected override string GetName(UnityEngine.Object original, int index)
        {
            return $"{original.name} - {index}";
        }
    }
}
