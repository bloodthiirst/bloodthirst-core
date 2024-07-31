using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MeshSplitterTest : MonoBehaviour
{
    static Vector3[] points = new Vector3[4]
        {
            new Vector3( +1, 0, +1 ),
            new Vector3( -1, 0, +1 ),
            new Vector3( -1, 0, -1 ),
            new Vector3( +1, 0, -1 ),
        };

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private Transform planeTransform;

    private Vector3[] planeTemp = new Vector3[4];

    [Header("Gizmos")]

    [Range(0 , 5)]
    [SerializeField]
    private float pushDistance;

    [SerializeField]
    private bool closeHole;

    [SerializeField]
    private Color planeColor;

    [SerializeField]
    private Color frontColor;

    [SerializeField]
    private Color behindColor;

    [SerializeField]
    private Color intersectColor;

    private List<Vector3> meshVerticies = new List<Vector3>();
    private List<int> frontTriangles = new List<int>();
    private List<int> behindTriangles = new List<int>();
    private List<int> intersectionIndicies = new List<int>();
    private List<Mesh> resultMeshes = new List<Mesh>();
    private List<MeshSplitter.TriangleIntersectionData> intersectingPoints = new List<MeshSplitter.TriangleIntersectionData>();
    private Vector3[] tempTriangle = new Vector3[3];

    private MeshSplitter splitter;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (splitter == null)
        {
            splitter = new MeshSplitter();
        }

        if (resultMeshes.Count == 0)
        {
            resultMeshes.Add(new Mesh());
            resultMeshes.Add(new Mesh());
        }

        meshVerticies.Clear();
        meshFilter.sharedMesh.GetVertices(meshVerticies);


        MeshSplitter.Input input = new MeshSplitter.Input()
        {
            mesh = meshFilter.sharedMesh,
            meshTransform = meshFilter.transform,
            planetTransform = planeTransform,
            closeHole = closeHole
        };

        MeshSplitter.Output output = new MeshSplitter.Output()
        {
            behindIndicies = behindTriangles,
            frontIndicies = frontTriangles,
            intersectionTriangles = intersectingPoints,
            intersectionIndicies = intersectionIndicies,
            resultMeshes = resultMeshes
        };

        splitter.Split(ref input, ref output);

        // Draw gizoms
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawPlane();
        }

        Gizmos.color = Color.red;

        // first
        {
            Mesh m = resultMeshes[0];
            Vector3 pos = meshFilter.transform.position + (planeTransform.up * pushDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawMesh(m, 0, pos, meshFilter.transform.rotation);

            Gizmos.color = Color.black;
            Gizmos.DrawWireMesh(m, 0, pos, meshFilter.transform.rotation);
        }

        // second
        {
            Mesh m = resultMeshes[1];
            Vector3 pos = meshFilter.transform.position - (planeTransform.up * pushDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawMesh(m, 0, pos, meshFilter.transform.rotation);
            
            Gizmos.color = Color.black;
            Gizmos.DrawWireMesh(m, 0, pos, meshFilter.transform.rotation);
        }
    }


    private void DrawPlane()
    {
        for (int i = 0; i < points.Length; i++)
        {
            planeTemp[i] = planeTransform.TransformPoint(points[i]);
        }

        Handles.color = planeColor;
        Handles.DrawAAConvexPolygon(planeTemp);
        Handles.color = Color.black;
        Handles.DrawPolyLine(planeTemp);
        Handles.DrawLine(planeTemp[0], planeTemp[planeTemp.Length - 1]);
    }
#endif
}
