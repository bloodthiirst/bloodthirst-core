﻿using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Singleton;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Scripts.Core.UnityPool
{

    /// <summary>
    /// Generic pooling system singleton used to retrieve and reuse gameobjects
    /// </summary>
    public class GenericUnityPool : UnitySingleton<GenericUnityPool>
    {
        [SerializeField]
        [Required]
        private GenericUnityPoolContainer unityPoolContainer = default;

        [SerializeField]
        [Required]
        private Transform poolParent = default;

        private Dictionary<Type, List<MonoBehaviour>> pooledObjects;

        private Dictionary<Type, List<MonoBehaviour>> PooledObjects
        {
            get
            {
                if (pooledObjects == null)
                {
                    pooledObjects = new Dictionary<Type, List<MonoBehaviour>>();
                }

                return pooledObjects;
            }
        }

        /// <summary>
        /// Initialize the pool and populate it with the instances
        /// </summary>
        [Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
        public void InitializePool()
        {
            PooledObjects.Clear();

#if UNITY_EDITOR
            poolParent.ClearTransformEditor();
#else
            poolParent.ClearTransform();
#endif

            foreach (MonoBehaviour prefab in unityPoolContainer.prefabsPoolDescription.Keys.ToList())
            {
                Type prefabType = prefab.GetType();

                PooledObjects.AddKeyValue(prefabType);

                for (int i = 0; i < unityPoolContainer.prefabsPoolDescription[prefab]; i++)
                {
                    MonoBehaviour go = Instantiate(prefab, poolParent);

                    PooledObjects[prefabType].Add(go);

                    go.gameObject.SetActive(false);
                }
            }

        }

        protected override void OnSetupSingletonPass()
        {
            InitializePool();
        }

        /// <summary>
        /// Get any instance of type T
        /// </summary>
        /// <typeparam name="T">Type of the behaviour requested</typeparam>
        /// <returns>pooled instance of type T</returns>
        public T Get<T>(Predicate<T> filter) where T : MonoBehaviour
        {
            Type type = typeof(T);

            List<MonoBehaviour> res = new List<MonoBehaviour>();

            if (!PooledObjects.TryGetValue(type, out res))
                return null;

            T instance = null;

            foreach (MonoBehaviour inst in res)
            {
                instance = inst.GetComponent<T>();

                if (instance == null)
                    continue;

                if (!filter(instance))
                    continue;

                res.Remove(inst);

                instance.gameObject.SetActive(true);

                instance.gameObject.transform.SetParent(null);

                return instance;

            }

            return null;
        }

        /// <summary>
        /// Get any instance of type T
        /// </summary>
        /// <typeparam name="T">Type of the behaviour requested</typeparam>
        /// <returns>pooled instance of type T</returns>
        public IEnumerable<T> Get<T>(int count, Predicate<T> filter) where T : MonoBehaviour
        {
            Type type = typeof(T);

            List<MonoBehaviour> res = new List<MonoBehaviour>();

            if (!PooledObjects.TryGetValue(type, out res))
                yield break;

            T instance = null;

            int number = res.Count < count ? res.Count : count;

            for (int i = 0; i < number; i++)
            {
                MonoBehaviour inst = res[i];
                instance = inst.GetComponent<T>();

                if (instance == null)
                    continue;

                if (!filter(instance))
                    continue;

                res.Remove(inst);

                instance.gameObject.SetActive(true);

                instance.gameObject.transform.SetParent(null);

                yield return instance;

            }

            yield break;
        }
        /// <summary>
        /// Get any instance of type T
        /// </summary>
        /// <typeparam name="T">Type of the behaviour requested</typeparam>
        /// <returns>pooled instance of type T</returns>
        public T Get<T>() where T : MonoBehaviour
        {
            Type type = typeof(T);

            List<MonoBehaviour> res = null;

            if (!PooledObjects.TryGetValue(type, out res))
                return null;

            T instance = res[0].GetComponent<T>();

            res.Remove(instance);

            instance.gameObject.SetActive(true);

            instance.gameObject.transform.SetParent(null);

            return instance;
        }

        /// <summary>
        /// Get multiple instances of type T
        /// </summary>
        /// <typeparam name="T">Type of the behaviour requested</typeparam>
        /// <returns>pooled instance of type T</returns>
        public IEnumerable<T> Get<T>(int count) where T : MonoBehaviour
        {
            Type type = typeof(T);

            List<MonoBehaviour> res = null;

            if (!PooledObjects.TryGetValue(type, out res))
                yield break;

            int number = res.Count < count ? res.Count : count;

            for (int i = 0; i < number; i++)
            {
                T instance = res[0].GetComponent<T>();

                res.Remove(instance);

                instance.gameObject.SetActive(true);

                instance.gameObject.transform.SetParent(null);

                yield return instance;
            }
        }

        /// <summary>
        /// Return the instance to the pool
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <param name="t">instance to return</param>
        public void Return<T>(T t) where T : MonoBehaviour
        {
            Type type = typeof(T);

            var res = new List<MonoBehaviour>();

            t.gameObject.transform.SetParent(poolParent);

            t.gameObject.SetActive(false);

            if (!PooledObjects.TryGetValue(type, out res))
            {
                PooledObjects.Add(type, new List<MonoBehaviour>() { t });
                return;
            }

            res.Add(t);

        }
    }
}
