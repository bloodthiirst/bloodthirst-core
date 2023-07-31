using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;
}
public class Edge
{
    public Vector2 p1;
    public Vector2 p2;
}

public class Circle
{
    public Vector2 center;
    public float radius;
}
public class Delauney
{
    private List<Triangle> badTriangles = new List<Triangle>();
    private List<Edge> edges = new List<Edge>();
    private List<Edge> uniqueEdges = new List<Edge>();
    private Triangle supraTriangle;

    private Triangle GenerateSupraTriangle(IReadOnlyList<Vector3> points)
    {
        Vector3 center = Vector3.zero;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            center += p;
        }

        center.x /= points.Count;
        center.y /= points.Count;

        float radiusSqr = Vector3.SqrMagnitude(points[0] - center);

        for (int i = 0; i < points.Count; ++i)
        {
            float currRadius = Vector3.SqrMagnitude(points[i] - center);

            radiusSqr = Mathf.Max(radiusSqr, currRadius);
        }

        float radius = Mathf.Sqrt(radiusSqr);

        // sin(x) = opp / hyp
        // hyp = opp / sin(x)

        float hyp = radius / Mathf.Sin(Mathf.Deg2Rad * 30);

        // hyp^2 = opp^2 + adj^2
        float adj = Mathf.Sqrt((hyp * hyp) - (radius * radius));

        Triangle supraTriangle = new Triangle();
        supraTriangle.p1 = center + (Quaternion.AngleAxis(60, Vector3.forward) * (Vector3.down * hyp));
        supraTriangle.p2 = center + (Quaternion.AngleAxis(180, Vector3.forward) * (Vector3.down * hyp));
        supraTriangle.p3 = center + (Quaternion.AngleAxis(-60, Vector3.forward) * (Vector3.down * hyp));

        return supraTriangle;
    }

    private bool EdgeComparer(Edge a, Edge b)
    {
        return (a.p1 == b.p1 && a.p2 == b.p2) || (a.p1 == b.p2 && a.p2 == b.p1);
    }

    bool IsInsideCircle(Circle c, Vector2 p)
    {
        return Vector2.Distance(p, c.center) < c.radius;
    }

    Circle GetCircucircle(Triangle tri)
    {
        ///Given that all verticies on a triangle must touch the outside of the CircumCircle.
        ///We can deduce that DA = DB = DC (Distances from each vertex to the center are equal).
        ///Formulae reference - https://en.wikipedia.org/wiki/Circumscribed_circle#Circumcircle_equations

        Vector2 A = tri.p1;
        Vector2 B = tri.p2;
        Vector2 C = tri.p3;
        Vector2 SqrA = new Vector2(A.x * A.x, A.y * A.y);
        Vector2 SqrB = new Vector2(B.x * B.x, B.y * B.y);
        Vector2 SqrC = new Vector2(C.x * C.x, C.y * C.y);

        float D = (A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y)) * 2f;
        float x = ((SqrA.x + SqrA.y) * (B.y - C.y) + (SqrB.x + SqrB.y) * (C.y - A.y) + (SqrC.x + SqrC.y) * (A.y - B.y)) / D;
        float y = ((SqrA.x + SqrA.y) * (C.x - B.x) + (SqrB.x + SqrB.y) * (A.x - C.x) + (SqrC.x + SqrC.y) * (B.x - A.x)) / D;
        Vector2 center = new Vector2(x, y);
        float radius = Vector2.Distance(A, center);

        return new Circle() { center = center, radius = radius };
    }

    public void Triangulate(IReadOnlyList<Vector3> points, List<Triangle> triangles)
    {
        // create the supra-triangle
        supraTriangle = GenerateSupraTriangle(points);

        // clear
        triangles.Clear();
        badTriangles.Clear();

        // algo
        triangles.Add(supraTriangle);

        for (int i1 = points.Count - 1; i1 >= 0; i1--)
        {

            Vector3 v = points[i1];

            for (int i = triangles.Count - 1; i >= 0; i--)
            {
                Triangle t = triangles[i];
                Circle circ = GetCircucircle(t);

                if (IsInsideCircle(circ, v))
                {
                    triangles.RemoveAt(i);
                    badTriangles.Add(t);
                }
            }

            edges.Clear();

            for (int i = badTriangles.Count - 1; i >= 0; i--)
            {
                Triangle t = badTriangles[i];
                badTriangles.RemoveAt(i);

                Edge e1 = new Edge() { p1 = t.p1, p2 = t.p2 };
                Edge e2 = new Edge() { p1 = t.p2, p2 = t.p3 };
                Edge e3 = new Edge() { p1 = t.p3, p2 = t.p1 };

                edges.Add(e1);
                edges.Add(e2);
                edges.Add(e3);
            }

            uniqueEdges.Clear();

            for (int i = 0; i < edges.Count; ++i)
            {
                bool isUnique = true;
                for (int j = 0; j < edges.Count; ++j)
                {
                    if (i != j && EdgeComparer(edges[i], edges[j]))
                    {
                        isUnique = false;
                        break;
                    }
                }

                if (isUnique)
                    uniqueEdges.Add(edges[i]);
            }

            for (int i = 0; i < uniqueEdges.Count; i++)
            {
                Edge e = uniqueEdges[i];

                Triangle t1 = new Triangle() { p1 = e.p1, p2 = e.p2, p3 = v };

                triangles.Add(t1);
            }
        }

        for (int i = triangles.Count - 1; i >= 0; i--)
        {
            Triangle t = triangles[i];
            int sharedPoints = 0;

            if (t.p1 == supraTriangle.p1 || t.p1 == supraTriangle.p2 || t.p1 == supraTriangle.p3)
            {
                sharedPoints++;
            }

            if (t.p2 == supraTriangle.p1 || t.p2 == supraTriangle.p2 || t.p2 == supraTriangle.p3)
            {
                sharedPoints++;
            }

            if (t.p3 == supraTriangle.p1 || t.p3 == supraTriangle.p2 || t.p3 == supraTriangle.p3)
            {
                sharedPoints++;
            }

            if (sharedPoints != 0)
            {
                triangles.RemoveAt(i);
            }
        }
    }
}
