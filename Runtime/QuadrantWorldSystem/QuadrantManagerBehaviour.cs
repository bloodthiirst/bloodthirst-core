﻿#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public class QuadrantManagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Vector3 cubeSize;

        [SerializeField]
        private Vector3Int testQuadrant;

        [SerializeField]
        [HideInInspector]
        private Vector3Int cachedTestQuadrant;

        private QuadrantManager<QuadrantEntityBehaviour, QuadLeafEquatableWorldChunk> quadrantManager;

        public QuadrantManager<QuadrantEntityBehaviour, QuadLeafEquatableWorldChunk> QuadrantManager => quadrantManager;

        private void Awake()
        {
            quadrantManager = new QuadrantManager<QuadrantEntityBehaviour, QuadLeafEquatableWorldChunk>();

            PreloadLeafs();
        }

        private void OnValidate()
        {
            if (quadrantManager == null)
            {
                quadrantManager = new QuadrantManager<QuadrantEntityBehaviour, QuadLeafEquatableWorldChunk>();
            }

            if (cachedTestQuadrant == testQuadrant && quadrantManager.CubeSize == cubeSize)
                return;

            quadrantManager.CubeSize = cubeSize;
            cachedTestQuadrant = testQuadrant;

            PreloadLeafs();

        }

        #if ODIN_INSPECTOR[Button]#endif
        private void Initialize()
        {
            quadrantManager.Clear();
            PreloadLeafs();
        }

        private void PreloadLeafs()
        {
            for (int x = 0; x < testQuadrant.x; x++)
            {
                for (int y = 0; y < testQuadrant.y; y++)
                {
                    for (int z = 0; z < testQuadrant.z; z++)
                    {
                        quadrantManager.QuadTree.Traverse(new List<int>() { x, y, z });
                    }
                }
            }
        }

        private void Update()
        {
            quadrantManager.CubeSize = cubeSize;
        }
    }
}
