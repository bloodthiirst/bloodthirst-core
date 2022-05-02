using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Utils
{
    public class BSpline
    {
        private List<Vector2> vertexData;

        public IReadOnlyList<Vector2> VertexData => vertexData;

        public BSpline(List<Vector2> points)
        {
            Assert.IsNotNull(points);
            Assert.IsTrue(points.Count > 3);

            vertexData = points;
        }

        public Vector2 GetPosition(int index , float t)
        {
            Vector2 p1 = VertexData[index];
            Vector2 p2 = VertexData[index + 1];
            Vector2 p3 = VertexData[index + 2];
            Vector2 p4 = VertexData[index + 3];

            Vector3 p =  Bezier.GetPoint(p1, p2, p3, p4, t);

            return p;
        }

        public void SetVertexData(IList<Vector2> points)
        {
            if (vertexData == points)
                return;

            vertexData.Clear();
            vertexData.AddRange(points);
        }

        public void SetPoints(IList<Vector2> points)
        {
            if (vertexData == points)
                return;

            vertexData.Clear();

            Rebuild(points);
        }

        public void UpdatePoint(Vector2 point, int index)
        {
            vertexData[index] = point;
        }

        public void AddPoint(Vector2 point)
        {
            vertexData.Add(point);
        }

        public void RemovePoint(int index)
        {
            Assert.IsTrue(vertexData.Count > 4, "You can't have less than 2 points in a line"); 

            int type = index % 3;

            Assert.IsTrue(type == 0, "You can't remove Handle vertices from the line , only point verticies are removable");

            if(index == 0)
            {
                vertexData.RemoveRange(0, 3);
                return;
            }

            if(index == vertexData.Count - 1)
            {
                vertexData.RemoveRange(vertexData.Count - 3, 3);
                return;
            }

            vertexData.RemoveRange(index - 1 , 3);
        }

        private void Rebuild(IList<Vector2> points)
        {
            BuildCurve(points);

            DefaultHandle();
        }

        public int GetPointCount()
        {
            // first we remove the first point and handle AND last point and handle
            // then we divide by the since the rest of the points are attached to 2 other handles
            //return ((vertexData.Count - 4) / 3) + 2;
            return (vertexData.Count / 3) + 1;
        }



        public int GetPointIndex(int index)
        {
            return 3 * index;
        }


        private void BuildCurve(IList<Vector2> points)
        {
            vertexData.Clear();

            // first point
            vertexData.Add(points[0]);
            // first handle
            vertexData.Add(points[0]);

            for(int i = 1; i < points.Count - 1; i++)
            {
                // last handle
                vertexData.Add(points[i]);

                // point
                vertexData.Add(points[i]);

                // next handle
                vertexData.Add(points[i]);
            }

            // last handle
            vertexData.Add(points[points.Count - 1]);

            // land point
            vertexData.Add(points[points.Count - 1]);
        }

        public int GetSegmentsCount()
        {
            return (vertexData.Count / 3);
        }

        public float GetLineLength()
        {
            float distance = 0;

            Vector2 last = vertexData[0];
            
            for(int i = 3; i < vertexData.Count; i += 3)
            {
                Vector2 curr = vertexData[i];
                distance += Vector2.Distance(curr, last);

                last = curr;
            }

            return distance;
        }

        public bool IsPoint(int vertexIndex)
        {
            return vertexIndex % 3 == 0;
        }

        private void DefaultHandle()
        {
            float lineLength = GetLineLength();
            int segCount = GetSegmentsCount();
            float handlLength = (lineLength / segCount) * 0.5f;

            Quaternion rightAngle = Quaternion.AngleAxis(-90, Vector3.forward);

            // first handle
            {
                Vector2 firstPoint = vertexData[0];
                Vector2 nextPoint = vertexData[3];

                vertexData[1] = firstPoint + ((Vector2)(rightAngle * (nextPoint - firstPoint).normalized * handlLength));
            }

            // after the first handle
            // what we do is set the previous handle based on the angle in the middle
            // and for the next handle we just mirror it
            for(int i = 1; i  < GetPointCount() - 1 ; i++)
            {
                int curretpoint = GetPointIndex(i);
                int prevPoint = GetPointIndex(i - 1);
                int nextPoint = GetPointIndex(i + 1);

                Vector2 currP = vertexData[curretpoint];
                Vector2 prevP = vertexData[prevPoint];
                Vector2 nextP = vertexData[nextPoint];

                Vector2 prevSeg = prevP - currP;
                Vector2 nextSeg = nextP - currP;

                float angleBetween =  Vector2.SignedAngle(prevSeg, nextSeg);

                Vector2 middleAngle = Quaternion.AngleAxis(angleBetween * 0.5f, Vector3.forward) * prevSeg.normalized * handlLength;

                // assign the handle
                vertexData[curretpoint - 1] = currP + middleAngle;

                // mirror the handle
                vertexData[curretpoint + 1] = currP - middleAngle;
            }

            // last handle
            {
                Vector2 lastPoint = vertexData[vertexData.Count - 1];
                Vector2 prevPoint = vertexData[vertexData.Count - 4];


                vertexData[vertexData.Count - 2] = lastPoint + ( (Vector2) (rightAngle * (prevPoint - lastPoint).normalized * handlLength) );
            }

        }
    }
}
