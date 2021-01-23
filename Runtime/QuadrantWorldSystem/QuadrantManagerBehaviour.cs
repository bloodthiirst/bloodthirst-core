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

        public QuadrantManager<QuadrantEntityBehaviour> QuadrantManager => quadrantManager;

        [BoxGroup("Editor Settings")]
        [SerializeField]
        private bool showAll;

        [BoxGroup("Editor Settings")]
        [HideIf(nameof(showAll))]
        [SerializeField]
        private float viewDistance;


        [BoxGroup("Editor Settings")]
        [SerializeField]
        [Range(0, 1)]
        private float visibility;

        private void Awake()
        {
            quadrantManager = new QuadrantManager<QuadrantEntityBehaviour>();

            PreloadLeafs();
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (quadrantManager == null)
                return;


            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fixedWidth = 30;
            labelStyle.normal.textColor = Color.white.MulColor(a: visibility);

            Vector3 halfSize = quadrantManager.CubeSize * 0.5f;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            HashSet<QuadLeaf<int, QuadrantEntityBehaviour>> allLeafts = quadrantManager.QuadTree.GetFinalLeafs();
            List<QuadLeaf<int, QuadrantEntityBehaviour>> nonEmptyLeafs = allLeafts.Where(l => l.Elements.Count != 0).ToList();
            // fill
            foreach (QuadLeaf<int, QuadrantEntityBehaviour> q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += halfSize;

                Handles.color = gizmosFillColor.MulColor(a: visibility);

                //Gizmos.DrawCube(center, quadrantManager.CubeSize);
                EditorUtils.HandlesDrawCube(center, quadrantManager.CubeSize);

            }

            // outline
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            foreach (QuadLeaf<int, QuadrantEntityBehaviour> q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);


                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += halfSize;

                Handles.color = gizmosOutlineColor.MulColor(a: visibility);
                EditorUtils.HandlesDrawCubeOutline(center, quadrantManager.CubeSize);

            }

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            labelStyle.contentOffset = new Vector2(0, -7.5f);

            Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;


            // label for count
            foreach (QuadLeaf<int, QuadrantEntityBehaviour> q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);


                float distance = Vector3.Distance(camPos, center);

                if (!showAll && distance > viewDistance)
                    continue;

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += quadrantManager.CubeSize * 0.5f;

                Handles.Label(center, $"Entities : { q.Elements.Count }", labelStyle);
            }

            labelStyle.contentOffset = new Vector2(0, 7.5f);

            // label for id
            foreach (QuadLeaf<int, QuadrantEntityBehaviour> q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);

                float distance = Vector3.Distance(camPos, center);

                if (!showAll && distance > viewDistance)
                    continue;

                center.x *= quadrantManager.CubeSize.x;
                center.y *= quadrantManager.CubeSize.y;
                center.z *= quadrantManager.CubeSize.z;

                center += quadrantManager.CubeSize * 0.5f;

                Handles.Label(center, $"(id : { q.PreviousKeys[0] } , {q.PreviousKeys[1]} , {q.Key})", labelStyle);
            }

        }
#endif
    }
}
