#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections.Generic;

public class DelauneyBehaviour : MonoBehaviour
{
    [Serializable]
    public struct VornoiPoint
    {
        public Vector3 point;
        public Color color;
    }

    [SerializeField]
    private int horizontalCount;

    [SerializeField]
    private int verticalCount;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float gizmoSize;

    [SerializeField]
    private List<VornoiPoint> vornoiPoints = new List<VornoiPoint>();

    private List<Triangle> triangles = new List<Triangle>();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        List<Vector3> verticies = new List<Vector3>();
        List<int> indicies = new List<int>();
        List<Vector2> uvs0 = new List<Vector2>();
        List<Vector2> uvs1 = new List<Vector2>();

        for (int v = 0; v < verticalCount; ++v)
        {
            float vFull = (verticalCount - 1);
            float vRatio = v / (float)vFull;

            for (int h = 0; h < horizontalCount; ++h)
            {
                float hFull = (horizontalCount - 1);
                float hRatio = h / (float)hFull;

                Vector3 p = Vector3.zero;
                Vector2 uv0 = Vector2.zero;
                Vector2 uv1 = Vector2.zero;

                float x = Mathf.Sin(Mathf.PI * hRatio) * Mathf.Cos(2 * Mathf.PI * vRatio);
                float z = Mathf.Sin(Mathf.PI * hRatio) * Mathf.Sin(2 * Mathf.PI * vRatio);
                float y = Mathf.Cos(Mathf.PI * hRatio);

                p.x = x;
                p.y = y;
                p.z = z;

                p *= radius;

                uv0.x = hRatio;
                uv0.y = vRatio;

                uv1.x = h;
                uv1.y = v;

                uvs0.Add(uv0);
                uvs1.Add(uv1);
                verticies.Add(p);

            }
        }

        Dictionary<VornoiPoint, List<Vector3>> pointsToVornoi = new Dictionary<VornoiPoint, List<Vector3>>();

        for (int i = 0; i < verticies.Count; i++)
        {
            Vector3 p = verticies[i];

            VornoiPoint closest = vornoiPoints[0];
            float closestDistance = Vector3.Distance(p, closest.point);

            for (int j = 1; j < vornoiPoints.Count; j++)
            {
                float currDistance = Vector3.Distance(p, vornoiPoints[j].point);

                if (currDistance < closestDistance)
                {
                    closest = vornoiPoints[j];
                    closestDistance = currDistance;
                }
            }

            if (!pointsToVornoi.TryGetValue(closest, out List<Vector3> list))
            {
                list = new List<Vector3>();
                pointsToVornoi.Add(closest, list);
            }

            list.Add(p);
        }

        Delauney d = new Delauney();

        foreach(KeyValuePair<VornoiPoint, List<Vector3>> kv in pointsToVornoi)
        {
            triangles.Clear();

            d.Triangulate(kv.Value, triangles);

            Handles.color = kv.Key.color;
            foreach (Triangle t in triangles)
            {
                DrawTriangle(t);
            }
        }

    }

    private static void DrawTriangle(Triangle triangle)
    {
        Handles.DrawLine(triangle.p1, triangle.p2, 1);
        Handles.DrawLine(triangle.p2, triangle.p3, 1);
        Handles.DrawLine(triangle.p3, triangle.p1, 1);
    }
#endif
}
