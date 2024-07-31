using System.Collections.Generic;
using System;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine;

public class MeshSplitter
{
    private struct IndexPointPair
    {
        public Vector3 point;
        public int index;
    }
    public struct Input
    {
        public Mesh mesh;
        public Transform planetTransform;
        public Transform meshTransform;
        public bool closeHole;
    }

    public struct TriangleIntersectionData
    {
        public int[] triangleIndicies;
        public List<Vector3> intersections;
    }

    public struct Output
    {
        public Mesh sourceMesh;
        public Plane planeInMeshSpace;
        public Vector3 normalInMeshSpace;
        public Vector3 pointInMeshSpace;
        public List<int> frontIndicies;
        public List<int> behindIndicies;
        public List<int> intersectionIndicies;
        public Vector3 holeCenter;
        public int holeIndex;

        public List<TriangleIntersectionData> intersectionTriangles;

        public List<Mesh> resultMeshes;
    }

    #region temporary variables
    private List<Vector3> verticies = new List<Vector3>();
    private List<int> indicies = new List<int>();
    private Vector3[] trianglePoints = new Vector3[3];
    private int[] triangleIndicies = new int[3];
    #endregion

    private static void TransformPlaneToMeshSpace(Transform plane, Transform mesh, out Vector3 normalInMeshSpace, out Vector3 pointInMeshSpace)
    {
        Vector3 normalWorld = plane.TransformDirection(Vector3.up);
        Vector3 pointWorld = plane.TransformPoint(Vector3.zero);

        normalInMeshSpace = mesh.transform.InverseTransformDirection(normalWorld);
        pointInMeshSpace = mesh.transform.InverseTransformPoint(pointWorld);
    }

    public static float DistanceToPlane(Vector3 plane, Vector3 pointOnPlane, Vector3 point)
    {
        float d = Vector3.Dot(plane, pointOnPlane);

        return Vector3.Dot(plane, point) - d;
    }

    public void Split(ref Input input, ref Output output)
    {
        verticies.Clear();
        indicies.Clear();

        Assert.IsNotNull(output.frontIndicies);
        Assert.IsNotNull(output.behindIndicies);
        Assert.IsNotNull(output.intersectionIndicies);
        Assert.IsNotNull(output.intersectionTriangles);

        output.frontIndicies.Clear();
        output.behindIndicies.Clear();
        output.intersectionIndicies.Clear();
        output.intersectionTriangles.Clear();

        input.mesh.GetVertices(verticies);
        input.mesh.GetIndices(indicies, 0);

        TransformPlaneToMeshSpace(input.planetTransform, input.meshTransform, out output.normalInMeshSpace, out output.pointInMeshSpace);

        Plane plane = new Plane(output.normalInMeshSpace, output.pointInMeshSpace);
        output.planeInMeshSpace = plane;

        // seprate the triangles
        SortTriangles(ref input, ref output);

        // calculate the the triangle intersections
        ComputeIntersectionData(ref output, ref plane);

        // create the two mesh sections
        PopulateSeparateMeshes(ref input, ref output);
    }

    private void PopulateSeparateMeshes(ref Input input, ref Output output)
    {
        // create the partial triangles
        using (ListPool<int>.Get(out List<int> front))
        using (ListPool<int>.Get(out List<int> behind))
        {
            for (int i = 0; i < output.intersectionTriangles.Count; i++)
            {
                front.Clear();
                behind.Clear();

                TriangleIntersectionData curr = output.intersectionTriangles[i];

                int interIndex1 = verticies.Count;
                Vector3 interPoint1 = curr.intersections[0];
                verticies.Add(interPoint1);

                int interIndex2 = verticies.Count;
                Vector3 interPoint2 = curr.intersections[1];
                verticies.Add(interPoint2);

                output.intersectionIndicies.Add(interIndex1);
                output.intersectionIndicies.Add(interIndex2);

                Vector3 p0 = verticies[curr.triangleIndicies[0]];
                Vector3 p1 = verticies[curr.triangleIndicies[1]];
                Vector3 p2 = verticies[curr.triangleIndicies[2]];

                Vector3 v1 = p1 - p0;
                Vector3 v2 = p2 - p0;

                Vector3 normal = Vector3.Cross(v1, v2);
                Vector3 center = (p0 + p1 + p2) / 3;
                //Handles.ArrowHandleCap(-888, center, Quaternion.LookRotation(normal), 1, EventType.Repaint);

                for (int p = 0; p < 3; p++)
                {
                    int index = curr.triangleIndicies[p];
                    Vector3 vert = verticies[index];

                    if (output.planeInMeshSpace.GetSide(vert))
                    {
                        front.Add(index);
                    }
                    else
                    {
                        behind.Add(index);
                    }
                }

                bool drawBehind = true;
                bool drawFront = true;

                // front triangles
                if (drawFront)
                {
                    if (front.Count == 1)
                    {
                        if (SameDirection(front[0], interIndex1, interIndex2, normal))
                        {
                            output.frontIndicies.Add(front[0]);
                            output.frontIndicies.Add(interIndex1);
                            output.frontIndicies.Add(interIndex2);
                        }
                        else
                        {
                            output.frontIndicies.Add(front[0]);
                            output.frontIndicies.Add(interIndex2);
                            output.frontIndicies.Add(interIndex1);
                        }
                    }

                    if (front.Count == 2)
                    {
                        Vector3 inter1To2Vec = (interPoint2 - interPoint1).normalized;
                        Vector3 inter1ToP0 = (verticies[front[0]] - interPoint1).normalized;
                        Vector3 inter1ToP1 = (verticies[front[1]] - interPoint1).normalized;

                        int diagIndex = front[1];
                        int otherIndex = front[0];

                        if (Vector3.Dot(inter1To2Vec, inter1ToP0) > Vector3.Dot(inter1To2Vec, inter1ToP1))
                        {
                            diagIndex = front[0];
                            otherIndex = front[1];
                        }

                        if (SameDirection(diagIndex, interIndex1, interIndex2, normal))
                        {
                            output.frontIndicies.Add(diagIndex);
                            output.frontIndicies.Add(interIndex1);
                            output.frontIndicies.Add(interIndex2);
                        }
                        else
                        {
                            output.frontIndicies.Add(diagIndex);
                            output.frontIndicies.Add(interIndex2);
                            output.frontIndicies.Add(interIndex1);
                        }

                        if (SameDirection(diagIndex, interIndex1, otherIndex, normal))
                        {
                            output.frontIndicies.Add(diagIndex);
                            output.frontIndicies.Add(interIndex1);
                            output.frontIndicies.Add(otherIndex);
                        }
                        else
                        {
                            output.frontIndicies.Add(diagIndex);
                            output.frontIndicies.Add(otherIndex);
                            output.frontIndicies.Add(interIndex1);
                        }
                    }
                }

                // behind
                if (drawBehind)
                {
                    if (behind.Count == 1)
                    {
                        if (SameDirection(behind[0], interIndex1, interIndex2, normal))
                        {
                            output.behindIndicies.Add(behind[0]);
                            output.behindIndicies.Add(interIndex1);
                            output.behindIndicies.Add(interIndex2);
                        }
                        else
                        {
                            output.behindIndicies.Add(behind[0]);
                            output.behindIndicies.Add(interIndex2);
                            output.behindIndicies.Add(interIndex1);
                        }
                    }

                    if (behind.Count == 2)
                    {
                        Vector3 inter1To2Vec = (interPoint2 - interPoint1).normalized;
                        Vector3 inter1ToP0 = (verticies[behind[0]] - interPoint1).normalized;
                        Vector3 inter1ToP1 = (verticies[behind[1]] - interPoint1).normalized;

                        int diagIndex = behind[1];
                        int otherIndex = behind[0];

                        if (Vector3.Dot(inter1To2Vec, inter1ToP0) > Vector3.Dot(inter1To2Vec, inter1ToP1))
                        {
                            diagIndex = behind[0];
                            otherIndex = behind[1];
                        }

                        if (SameDirection(diagIndex, interIndex1, interIndex2, normal))
                        {
                            output.behindIndicies.Add(diagIndex);
                            output.behindIndicies.Add(interIndex1);
                            output.behindIndicies.Add(interIndex2);
                        }
                        else
                        {
                            output.behindIndicies.Add(diagIndex);
                            output.behindIndicies.Add(interIndex2);
                            output.behindIndicies.Add(interIndex1);
                        }

                        if (SameDirection(diagIndex, interIndex1, otherIndex, normal))
                        {
                            output.behindIndicies.Add(diagIndex);
                            output.behindIndicies.Add(interIndex1);
                            output.behindIndicies.Add(otherIndex);
                        }
                        else
                        {
                            output.behindIndicies.Add(diagIndex);
                            output.behindIndicies.Add(otherIndex);
                            output.behindIndicies.Add(interIndex1);
                        }
                    }
                }
            }
        }

        if (input.closeHole && output.intersectionIndicies.Count != 0)
        {
            using (ListPool<IndexPointPair>.Get(out List<IndexPointPair> tmp))
            {
                Vector3 center = default;

                for (int i = 0; i < output.intersectionIndicies.Count; ++i)
                {
                    Vector3 v = verticies[output.intersectionIndicies[i]];
                    center += v;

                    tmp.Add(new IndexPointPair() { index = i, point = v });
                }

                center /= output.intersectionIndicies.Count;

                output.holeCenter = center;

                Vector3 normal = output.normalInMeshSpace;
                Vector3 right = Vector3.Cross(normal, verticies[output.intersectionIndicies[0]] - center).normalized;

                // TODO : check if the sorting is even needed
                //tmp.Sort((a, b) => Vector3.SignedAngle(right, a.point, normal).CompareTo(Vector3.SignedAngle(right, b.point, normal)));

                int centerIndex = verticies.Count;
                verticies.Add(center);
                output.holeIndex = centerIndex;

                for (int i = 0; i < output.intersectionIndicies.Count; i++)
                {
                    int curr = output.intersectionIndicies[i];
                    int next = output.intersectionIndicies[(i + 1) % output.intersectionIndicies.Count];

                    if (SameDirection(centerIndex, curr, next, -output.normalInMeshSpace))
                    {
                        output.frontIndicies.Add(centerIndex);
                        output.frontIndicies.Add(curr);
                        output.frontIndicies.Add(next);

                        output.behindIndicies.Add(centerIndex);
                        output.behindIndicies.Add(next);
                        output.behindIndicies.Add(curr);
                    }
                    else
                    {
                        output.frontIndicies.Add(centerIndex);
                        output.frontIndicies.Add(next);
                        output.frontIndicies.Add(curr);

                        output.behindIndicies.Add(centerIndex);
                        output.behindIndicies.Add(curr);
                        output.behindIndicies.Add(next);
                    }
                }
            }
        }

        // front
        {
            Mesh m = output.resultMeshes[0];
            m.Clear();
            m.SetVertices(verticies);
            m.SetIndices(output.frontIndicies, MeshTopology.Triangles, 0, true);
            m.RecalculateNormals();
        }

        // behind
        {
            Mesh m = output.resultMeshes[1];

            m.Clear();
            m.SetVertices(verticies);
            m.SetIndices(output.behindIndicies, MeshTopology.Triangles, 0, true);
            m.RecalculateNormals();
        }
    }

    private bool SameDirection(int i0, int i1, int i2, Vector3 normal)
    {
        Vector3 p0 = verticies[i0];
        Vector3 p1 = verticies[i1];
        Vector3 p2 = verticies[i2];

        Vector3 v1 = p1 - p0;
        Vector3 v2 = p2 - p0;

        Vector3 normalTest = Vector3.Cross(v1, v2);

        return Vector3.Dot(normalTest, normal) > 0;
    }

    /// <summary>
    /// Compute the data related to the intersecting trianglesa
    /// </summary>
    /// <param name="output"></param>
    /// <param name="plane"></param>
    private void ComputeIntersectionData(ref Output output, ref Plane plane)
    {
        for (int i = 0; i < output.intersectionTriangles.Count; i++)
        {
            Gizmos.color = Color.black;

            TriangleIntersectionData currTriangle = output.intersectionTriangles[i];

            for (int j = 0; j < 3; j++)
            {
                int curr = currTriangle.triangleIndicies[j];
                int next = currTriangle.triangleIndicies[(j + 1) % 3];

                Vector3 currVertex = verticies[curr];
                Vector3 nextVertex = verticies[next];

                trianglePoints[j] = currVertex;

                // if both points are on the same side , skip
                // we can deduce this by the sign of their distance
                if (plane.SameSide(currVertex, nextVertex))
                    continue;

                Vector3 currToNextVec = nextVertex - currVertex;
                Vector3 currToNextDir = currToNextVec.normalized;
                Ray ray = new Ray(currVertex, currToNextDir);
                bool exists = plane.Raycast(ray, out float dist);

                if (!exists)
                    continue;

                Vector3 onPlaneMeshSpace = ray.GetPoint(dist);

                currTriangle.intersections.Add(onPlaneMeshSpace);
            }
        }
    }

    /// <summary>
    /// <para>Sort the triangles into 3 categories</para>
    /// <para>Triangles completely in front of the plane </para>
    /// <para>Triangles completely in behind the plane </para>
    /// <para>Triangles intersecting with the plane the plane </para>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    private void SortTriangles(ref Input input, ref Output output)
    {
        for (int i = 0; i < indicies.Count; i += 3)
        {
            int frontOfPlane = 0;
            int behindOfPlane = 0;

            for (int j = 0; j < 3; j++)
            {
                int index = triangleIndicies[j] = indicies[i + j];
                Vector3 v = verticies[index];

                float distToPlane = DistanceToPlane(output.normalInMeshSpace, output.pointInMeshSpace, v);

                trianglePoints[j] = input.meshTransform.TransformPoint(v);

                if (distToPlane < 0)
                {
                    behindOfPlane++;
                }
                else
                {
                    frontOfPlane++;
                }
            }

            if (frontOfPlane == 3)
            {
                output.frontIndicies.AddRange(triangleIndicies);
            }
            else if (behindOfPlane == 3)
            {
                output.behindIndicies.AddRange(triangleIndicies);
            }
            else
            {
                int[] copy = new int[3];
                Array.Copy(triangleIndicies, copy, triangleIndicies.Length);

                TriangleIntersectionData data = new TriangleIntersectionData()
                {
                    triangleIndicies = copy,
                    intersections = new List<Vector3>()
                };

                output.intersectionTriangles.Add(data);
            }
        }
    }
}