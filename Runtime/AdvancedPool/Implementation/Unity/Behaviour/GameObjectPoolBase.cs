﻿#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
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

        [ReadOnly]
        [ShowInInspector]
        private int poolCount;

        private Stack<GameObject> freeInstances = new Stack<GameObject>();

        private List<GameObject> usedInstances = new List<GameObject>();

        public IReadOnlyCollection<GameObject> UsedInstances => usedInstances;

        private Transform poolTransform;

        public GameObjectPoolBase(Transform transform, GameObject prefab, int poolCount)
        {
            poolTransform = transform;
            Prefab = prefab;
            this.poolCount = poolCount;
        }

        public void PopulatePool()
        {
            AddToPool(poolCount);
        }

        private void AddToPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject inst = UnityEngine.Object.Instantiate(Prefab, poolTransform);
                inst.name = GetName(Prefab, i);
                inst.gameObject.SetActive(false);
                inst.transform.SetParent(poolTransform);
                freeInstances.Push(inst);
                usedInstances.Remove(inst);
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

            t.transform.SetParent(poolTransform);
            t.gameObject.SetActive(false);
            freeInstances.Push(t);
            usedInstances.Remove(t);
        }

        private void CheckPoolSize()
        {
            // re-instantiate if no more objects are available
            if (freeInstances.Count == 0)
            {
                AddToPool(poolCount);
            }
        }

        public override GameObject Get()
        {
            CheckPoolSize();

            GameObject obj = freeInstances.Pop();
            usedInstances.Add(obj);

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

        protected override string GetName(UnityEngine.Object original, int index)
        {
            return $"{original.name} - {index}";
        }
    }
}
