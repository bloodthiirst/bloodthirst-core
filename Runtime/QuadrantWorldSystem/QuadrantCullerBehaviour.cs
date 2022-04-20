using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    [ExecuteAlways]
    public class QuadrantCullerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;

        [SerializeField]
        private QuadrantManagerBehaviour quadrantManagerBehaviour;

        [SerializeField]
        private Color gizmosOutlineColor;

        [SerializeField]
        private Color gizmosFillColor;

        [SerializeField]
        private Color gizmosEmptyColor;

        [SerializeField]
        private Color gizmosCulledColor;

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

        private Plane[] planes = new Plane[6];

        private Vector3[] boundsCorners = new Vector3[8];

        private void Update()
        {
            if (quadrantManagerBehaviour == null)
                return;

            GeometryUtility.CalculateFrustumPlanes(cam, planes);

            HashSet<QuadLeafEquatableWorldChunk> allLeafts = quadrantManagerBehaviour.QuadrantManager.QuadTree.GetFinalLeafs();

            foreach (QuadLeafEquatableWorldChunk q in allLeafts)
            {
                q.IsCulled = IsCulled(q);
            }
        }

        private bool IsCulled(QuadLeafEquatableWorldChunk q)
        {
            List<int> keys = q.GetKeySequence();
            Vector3 center = new Vector3(keys[0], keys[1], keys[2]);

            center.x *= quadrantManagerBehaviour.QuadrantManager.CubeSize.x;
            center.y *= quadrantManagerBehaviour.QuadrantManager.CubeSize.y;
            center.z *= quadrantManagerBehaviour.QuadrantManager.CubeSize.z;

            center += quadrantManagerBehaviour.QuadrantManager.CubeSize * 0.5f;

            Bounds b = new Bounds(center, quadrantManagerBehaviour.QuadrantManager.CubeSize);

            return !GeometryUtility.TestPlanesAABB(planes, b);

        }

        private Color GetColor(List<int> keys)
        {

            QuadLeafEquatableWorldChunk q = quadrantManagerBehaviour.QuadrantManager.QuadTree.Traverse(keys);
            Vector3 center = new Vector3(keys[0], keys[1], keys[2]);

            center.x *= quadrantManagerBehaviour.QuadrantManager.CubeSize.x;
            center.y *= quadrantManagerBehaviour.QuadrantManager.CubeSize.y;
            center.z *= quadrantManagerBehaviour.QuadrantManager.CubeSize.z;

            center += quadrantManagerBehaviour.QuadrantManager.CubeSize * 0.5f;

            Bounds b = new Bounds(center, quadrantManagerBehaviour.QuadrantManager.CubeSize);

            b.GetCorners(boundsCorners);

            if (!GeometryUtility.TestPlanesAABB(planes, b))
            {
                return gizmosCulledColor;
            }

            if (q.Elements.Count == 0)
                return gizmosEmptyColor;
            else
                return gizmosFillColor;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (quadrantManagerBehaviour == null)
                return;


            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fixedWidth = 30;
            labelStyle.normal.textColor = Color.white.MulColor(a: visibility);

            Vector3 halfSize = quadrantManagerBehaviour.QuadrantManager.CubeSize * 0.5f;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            HashSet<QuadLeafEquatableWorldChunk> allLeafts = quadrantManagerBehaviour.QuadrantManager.QuadTree.GetFinalLeafs();
            List<QuadLeafEquatableWorldChunk> nonEmptyLeafs = allLeafts.Where(l => l.Elements.Count != 0).ToList();

            // fill
            foreach (QuadLeafEquatableWorldChunk q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);

                center.x *= quadrantManagerBehaviour.QuadrantManager.CubeSize.x;
                center.y *= quadrantManagerBehaviour.QuadrantManager.CubeSize.y;
                center.z *= quadrantManagerBehaviour.QuadrantManager.CubeSize.z;

                center += halfSize;

                Handles.color = GetColor(keys).MulColor(a: visibility);

                //Gizmos.DrawCube(center, quadrantManager.CubeSize);
                GizmosUtils.HandlesDrawCube(center, quadrantManagerBehaviour.QuadrantManager.CubeSize);

            }

            // outline
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            foreach (QuadLeafEquatableWorldChunk q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);


                center.x *= quadrantManagerBehaviour.QuadrantManager.CubeSize.x;
                center.y *= quadrantManagerBehaviour.QuadrantManager.CubeSize.y;
                center.z *= quadrantManagerBehaviour.QuadrantManager.CubeSize.z;

                center += halfSize;

                Handles.color = gizmosOutlineColor.MulColor(a: visibility);
                GizmosUtils.HandlesDrawCubeOutline(center, quadrantManagerBehaviour.QuadrantManager.CubeSize);

            }

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            labelStyle.contentOffset = new Vector2(0, -7.5f);

            SceneView scene = SceneView.lastActiveSceneView;

            Vector3 camPos = scene.camera.transform.position;


            // label for count
            foreach (QuadLeafEquatableWorldChunk q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);


                float distance = Vector3.Distance(camPos, center);

                if (!showAll && distance > viewDistance)
                    continue;

                center.x *= quadrantManagerBehaviour.QuadrantManager.CubeSize.x;
                center.y *= quadrantManagerBehaviour.QuadrantManager.CubeSize.y;
                center.z *= quadrantManagerBehaviour.QuadrantManager.CubeSize.z;

                center += quadrantManagerBehaviour.QuadrantManager.CubeSize * 0.5f;

                Handles.Label(center, $"Entities : { q.Elements.Count }", labelStyle);
            }

            labelStyle.contentOffset = new Vector2(0, 7.5f);

            // label for id
            foreach (QuadLeafEquatableWorldChunk q in allLeafts)
            {
                List<int> keys = q.GetKeySequence();

                if (keys.Count != 3)
                    continue;

                Vector3 center = new Vector3(keys[0], keys[1], keys[2]);

                float distance = Vector3.Distance(camPos, center);

                if (!showAll && distance > viewDistance)
                    continue;

                center.x *= quadrantManagerBehaviour.QuadrantManager.CubeSize.x;
                center.y *= quadrantManagerBehaviour.QuadrantManager.CubeSize.y;
                center.z *= quadrantManagerBehaviour.QuadrantManager.CubeSize.z;

                center += quadrantManagerBehaviour.QuadrantManager.CubeSize * 0.5f;

                Handles.Label(center, $"(id : { q.PreviousKeys[0] } , {q.PreviousKeys[1]} , {q.Key})", labelStyle);
            }

        }
#endif
    }
}
