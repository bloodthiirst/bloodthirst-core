using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public class QuadrantManagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Color gizmosOutlineColor;

        [SerializeField]
        private Color gizmosFillColor;

        [SerializeField]
        private Vector3 cubeSize;

        [SerializeField]
        private Vector3Int testQuadrant;

        [SerializeField]
        [HideInInspector]
        private Vector3Int cachedTestQuadrant;

        private QuadrantManager<QuadrantEntityBehaviour> quadrantManager;

        private void Awake()
        {
            quadrantManager = new QuadrantManager<QuadrantEntityBehaviour>();
        }

        private void OnValidate()
        {
            if (quadrantManager == null)
            {
                quadrantManager = new QuadrantManager<QuadrantEntityBehaviour>();
            }

            

            if (cachedTestQuadrant == testQuadrant && quadrantManager.CubeSize == cubeSize)
                return;

            quadrantManager.CubeSize = cubeSize;
            cachedTestQuadrant = testQuadrant;

            quadrantManager.Clear();

            for (int x = 0; x < testQuadrant.x; x++)
            {
                for (int y = 0; y < testQuadrant.y; y++)
                {
                    for (int z = 0; z < testQuadrant.z; z++)
                    {
                        quadrantManager.TryAddQuadrantCube(x, y, z);
                    }
                }
            }

        }

        private void Update()
        {
            if (quadrantManager.CubeSize == cubeSize)
                return;

            quadrantManager.CubeSize = cubeSize;
        }

        private void OnDrawGizmos()
        {
            if (quadrantManager == null)
                return;


            foreach (KeyValuePair<(int, int, int), List<QuadrantEntityBehaviour>> q in quadrantManager.QuadrantColllection)
            {

                Vector3 center = new Vector3(q.Key.Item1, q.Key.Item2, q.Key.Item3);

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += quadrantManager.CubeSize * 0.5f;


                Gizmos.color = gizmosOutlineColor;
                Gizmos.DrawWireCube(center, quadrantManager.CubeSize);

                Gizmos.color = gizmosFillColor;
                Gizmos.DrawCube(center, quadrantManager.CubeSize);

            }
        }
    }
}
