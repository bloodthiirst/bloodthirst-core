using Bloodthirst.Scripts.Utils;
using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
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

        private QuadrantManager<QuadrantEntityBehaviour , QuadLeafEquatableWorldChunk> quadrantManager;

        public QuadrantManager<QuadrantEntityBehaviour , QuadLeafEquatableWorldChunk> QuadrantManager => quadrantManager;

        private void Awake()
        {
            quadrantManager = new QuadrantManager<QuadrantEntityBehaviour , QuadLeafEquatableWorldChunk>();

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

        [Button]
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
