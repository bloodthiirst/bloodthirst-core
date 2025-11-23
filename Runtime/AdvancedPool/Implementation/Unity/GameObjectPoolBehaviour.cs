using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public class GameObjectPoolBehaviour : MonoBehaviour, IPoolBehaviour
    {
        public Type Type => typeof(GameObject);

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private int count;

        public GameObject Prefab { get => prefab; set => prefab = value; }
        public int Count { get => count; set => count = value; }

        private GameObjectPoolBase unityPool;
        public UnityPoolBase Pool => unityPool;

        public void Initialize()
        {
            unityPool = new GameObjectPoolBase(transform, prefab, count);
        }

        public void Populate()
        {
            unityPool.PopulatePool();
        }
    }
}