using Assets.Scripts.Utils;
using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using UnityEditor;
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


            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fixedWidth = 30;
            labelStyle.normal.textColor = Color.white;

            Vector3 halfSize = quadrantManager.CubeSize * 0.5f;





            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            // fill
            foreach (KeyValuePair<(int, int, int), List<QuadrantEntityBehaviour>> q in quadrantManager.QuadrantColllection)
            {
                Vector3 center = new Vector3(q.Key.Item1, q.Key.Item2, q.Key.Item3);

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += halfSize;
               
                Handles.color = gizmosFillColor;
                //Gizmos.DrawCube(center, quadrantManager.CubeSize);
                EditorUtils.HandlesDrawCube(center , quadrantManager.CubeSize);

            }

            // outline
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            foreach (KeyValuePair<(int, int, int), List<QuadrantEntityBehaviour>> q in quadrantManager.QuadrantColllection)
            {
                Vector3 center = new Vector3(q.Key.Item1, q.Key.Item2, q.Key.Item3);

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += halfSize;

                Handles.color = gizmosOutlineColor;
                EditorUtils.HandlesDrawCubeOutline(center, quadrantManager.CubeSize);

            }

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            labelStyle.contentOffset = new Vector2(0, -7.5f);

            // label for count
            foreach (KeyValuePair<(int, int, int), List<QuadrantEntityBehaviour>> q in quadrantManager.QuadrantColllection)
            {
                Vector3 center = new Vector3(q.Key.Item1, q.Key.Item2, q.Key.Item3);

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += quadrantManager.CubeSize * 0.5f;


                Handles.Label(center, $"Entities : { q.Value.Count }", labelStyle);
            }

            labelStyle.contentOffset = new Vector2(0, 7.5f);

            // label for id
            foreach (KeyValuePair<(int, int, int), List<QuadrantEntityBehaviour>> q in quadrantManager.QuadrantColllection)
            {
                Vector3 center = new Vector3(q.Key.Item1, q.Key.Item2, q.Key.Item3);

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += quadrantManager.CubeSize * 0.5f;


                Handles.Label(center, $"(id : { q.Key.Item1 } , {q.Key.Item2} , {q.Key.Item3})", labelStyle);
            }

        }
    }
}
