using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Core.Utils
{
    public class DelauneyTriangulation
    {
        public struct Vector2Indexed
        {
            public Vector2 vec;
            public int index;
        }

        public static Vector3 GetCircumcircle(Vector2 a, Vector2 b, Vector2 c)
        {
            // Midpoints of AB and AC
            Vector2 midAB = (a + b) * 0.5f;
            Vector2 midAC = (a + c) * 0.5f;

            // Direction vectors of AB and AC
            Vector2 dirAB = b - a;
            Vector2 dirAC = c - a;

            // Perpendicular vectors (rotated 90°)
            Vector2 perpAB = new Vector2(-dirAB.y, dirAB.x);
            Vector2 perpAC = new Vector2(-dirAC.y, dirAC.x);

            // Solve intersection: midAB + t*perpAB = midAC + s*perpAC
            float denom = perpAB.x * perpAC.y - perpAB.y * perpAC.x;
            if (Mathf.Abs(denom) < 1e-12f)
            {
                // Lines are parallel → points are collinear
                return new Vector3(0, 0, -1f);
            }

            Vector2 diff = midAC - midAB;
            float t = (diff.x * perpAC.y - diff.y * perpAC.x) / denom;
            Vector2 center = midAB + t * perpAB;

            float rSq = (a - center).sqrMagnitude;
            return new Vector3(center.x, center.y, rSq);
        }

        public static void Triangulate(IReadOnlyList<Vector2> points, List<TriangleIndicies> resultTriangles, out Vector2[] superTri)
        {
            // create super tri
            {
                Vector2 middle = default;

                foreach (Vector2 p in points)
                {
                    middle += p;
                }

                middle /= points.Count;

                float radiusSqr = points.Max(p => Vector2.SqrMagnitude(middle - p));
                float radius = Mathf.Sqrt(radiusSqr);
                float padding = 0.5f;

                float len = (radius + padding) / Mathf.Cos(60 * Mathf.Deg2Rad);

                Vector2 v1 = middle + (Vector2.up * (len + padding));
                Vector2 v2 = middle + (Vector2)(Quaternion.AngleAxis(120, Vector3.forward) * (Vector2.up * (len + padding)));
                Vector2 v3 = middle + (Vector2)(Quaternion.AngleAxis(-120, Vector3.forward) * (Vector2.up * (len + padding)));

                superTri = new Vector2[] { v1, v2, v3 };
            }

            using (ListPool<Vector2>.Get(out List<Vector2> allPoints))
            using (ListPool<Vector2Indexed>.Get(out List<Vector2Indexed> pointsIndexed))
            using (DictionaryPool<EdgeIndicies, int>.Get(out Dictionary<EdgeIndicies, int> edgeCount))
            using (ListPool<TriangleIndicies>.Get(out List<TriangleIndicies> goodTriangles))
            using (ListPool<TriangleIndicies>.Get(out List<TriangleIndicies> badTriangles))
            using (ListPool<int>.Get(out List<int> idxSort))
            {
                allPoints.AddRange(points);
                allPoints.Add(superTri[0]);
                allPoints.Add(superTri[1]);
                allPoints.Add(superTri[2]);

                for (int i = 0; i < allPoints.Count; i++)
                {
                    Vector2 p = allPoints[i];
                    pointsIndexed.Add(new Vector2Indexed() { vec = p, index = i });
                }

                TriangleIndicies initTri = new TriangleIndicies() { v1 = pointsIndexed.Count - 3, v2 = pointsIndexed.Count - 2, v3 = pointsIndexed.Count - 1 };
                goodTriangles.Add(initTri);

                for (int i = 0; i < pointsIndexed.Count; i++)
                {
                    badTriangles.Clear();

                    Vector2Indexed p = pointsIndexed[i];

                    for (int i1 = 0; i1 < goodTriangles.Count; i1++)
                    {
                        TriangleIndicies goodTri = goodTriangles[i1];

                        Vector2Indexed p1 = pointsIndexed[goodTri.v1];
                        Vector2Indexed p2 = pointsIndexed[goodTri.v2];
                        Vector2Indexed p3 = pointsIndexed[goodTri.v3];

                        Vector3 circle = DelauneyTriangulation.GetCircumcircle(p1.vec, p2.vec, p3.vec);

                        if (circle.z > 0)
                        {
                            Vector2 center = new Vector2(circle.x, circle.y);

                            float circumRadiusSqr = circle.z;
                            float distanceToCircumSqr = (p.vec - center).sqrMagnitude;

                            if (distanceToCircumSqr <= circumRadiusSqr)
                            {
                                badTriangles.Add(goodTri);
                            }
                        }
                    }

                    edgeCount.Clear();

                    foreach (TriangleIndicies b in badTriangles)
                    {
                        EdgeIndicies e1 = new EdgeIndicies() { e1 = Mathf.Min(b.v1, b.v2), e2 = Mathf.Max(b.v1, b.v2) };
                        EdgeIndicies e2 = new EdgeIndicies() { e1 = Mathf.Min(b.v2, b.v3), e2 = Mathf.Max(b.v2, b.v3) };
                        EdgeIndicies e3 = new EdgeIndicies() { e1 = Mathf.Min(b.v3, b.v1), e2 = Mathf.Max(b.v3, b.v1) };

                        if (!edgeCount.TryGetValue(e1, out _)) { edgeCount.Add(e1, 0); }
                        if (!edgeCount.TryGetValue(e2, out _)) { edgeCount.Add(e2, 0); }
                        if (!edgeCount.TryGetValue(e3, out _)) { edgeCount.Add(e3, 0); }

                        edgeCount[e1]++;
                        edgeCount[e2]++;
                        edgeCount[e3]++;
                    }

                    foreach (TriangleIndicies b in badTriangles)
                    {
                        goodTriangles.Remove(b);
                    }

                    foreach (KeyValuePair<EdgeIndicies, int> kv in edgeCount)
                    {
                        if (kv.Value != 1)
                        {
                            continue;
                        }

                        idxSort.Clear();
                        idxSort.Add(p.index);
                        idxSort.Add(kv.Key.e1);
                        idxSort.Add(kv.Key.e2);
                        idxSort.Sort();

                        TriangleIndicies newT = new TriangleIndicies() { v1 = idxSort[0], v2 = idxSort[1], v3 = idxSort[2] };
                        goodTriangles.Add(newT);
                    }
                }

                int validPointsCount = pointsIndexed.Count - 3;

                for (int i = goodTriangles.Count - 1; i >= 0; i--)
                {
                    TriangleIndicies t = goodTriangles[i];

                    bool isValid = (t.v1 < validPointsCount) && (t.v2 < validPointsCount) && (t.v3 < validPointsCount);

                    if (!isValid)
                    {
                        goodTriangles.RemoveAt(i);
                    }
                }

                for (int i = 0; i < goodTriangles.Count; i++)
                {
                    TriangleIndicies t = goodTriangles[i];           
                    t = GraphicsUtils.AlignTriangleNormal(points, t, Vector3.forward);
                    goodTriangles[i] = t;
                }

                resultTriangles.AddRange(goodTriangles);

            }
        }
    }
}