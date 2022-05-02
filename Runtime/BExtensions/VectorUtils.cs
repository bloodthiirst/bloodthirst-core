using Bloodthirst.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Utils
{
    public static class VectorUtils
    {
       

        private class LineQuad
        {
            public Vector3 LeftUpPos { get; set; }
            public Vector3 LeftDownPos { get; set; }
            public Vector3 RightUpPos { get; set; }
            public Vector3 RightDownPos { get; set; }
            public Vector3 Normal { get; set; }
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }


        public static Vector2 NormalizedToLayoutSpace(RectTransform rectTransform, Vector2 normalizedInLayout)
        {
            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

            return new Vector2(normalizedInLayout.x * sizeX + offsetX, normalizedInLayout.y * sizeY + offsetY);
        }

        public static Vector2 LayoutToNormalizedSpace(RectTransform rectTransform, Vector2 layout)
        {
            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

            return new Vector2((layout.x - offsetX) / sizeX, (layout.y - offsetY) / sizeY);
        }

        public struct CurveSettings
        {
            [SerializeField]
            private UVSmoothingType uVSmoothing;

            [SerializeField]
            private float uVSmoothLerp;

            [SerializeField]
            private int cornerSmoothing;

            [SerializeField]
            private float lineThikness;

            [SerializeField]
            private float handlesLength;

            [SerializeField]
            private float detailPerSegment;

            [SerializeField]
            private bool normalizeHandles;

            [SerializeField]
            private bool invertHandles;

            public UVSmoothingType UVSmoothing { get => uVSmoothing; set => uVSmoothing = value; }
            public float UVSmoothLerp { get => uVSmoothLerp; set => uVSmoothLerp = value; }
            public int CornerSmoothing { get => cornerSmoothing; set => cornerSmoothing = value; }
            public float LineThikness { get => lineThikness; set => lineThikness = value; }
            public float HandlesLength { get => handlesLength; set => handlesLength = value; }
            public float DetailPerSegment { get => detailPerSegment; set => detailPerSegment = value; }
            public bool NormalizeHandles { get => normalizeHandles; set => normalizeHandles = value; }
            public bool InvertHandles { get => invertHandles; set => invertHandles = value; }
        }

        public static List<UIVertex> SplineToCurve(IReadOnlyList<Vector2> InterpolatedPoints, CurveSettings curveSettings, out float lineLength)
        {
            List<LineQuad> finaleVerts = new List<LineQuad>();

            lineLength = 0;

            float haffThicnessPlus = curveSettings.LineThikness / 2;
            float haffThicnessMinus = -curveSettings.LineThikness / 2;

            // create the quads that define the line width
            for (int i = 1; i < InterpolatedPoints.Count; i++)
            {
                Vector2 prev = InterpolatedPoints[i - 1];
                Vector2 cur = InterpolatedPoints[i];

                if (cur == prev)
                    continue;


                float angle = Mathf.Atan2(cur.y - prev.y, cur.x - prev.x) * Mathf.Rad2Deg;

                float currentLength = (cur - prev).magnitude;
                
                // form the thickness vertices
                // at this point they are not yet rotated to have the same direction as the segmenet
                Vector2 prevBottom = prev + new Vector2(0, haffThicnessMinus);
                Vector2 prevUp = prev + new Vector2(0, haffThicnessPlus);

                Vector2 currentBottom = cur + new Vector2(0, haffThicnessMinus);
                Vector2 currentUp = cur + new Vector2(0, haffThicnessPlus);

                // here we rotate them
                Vector3 rotateBy = new Vector3(0, 0, angle);
                prevBottom = RotatePointAroundPivot(prevBottom, prev, rotateBy);
                prevUp = RotatePointAroundPivot(prevUp, prev, rotateBy);
                currentUp = RotatePointAroundPivot(currentUp, cur, rotateBy);
                currentBottom = RotatePointAroundPivot(currentBottom, cur, rotateBy);

                LineQuad quad = new LineQuad()
                {
                    RightDownPos = currentBottom,
                    RightUpPos = currentUp,
                    LeftDownPos = prevBottom,
                    LeftUpPos = prevUp
                };

                finaleVerts.Add(quad);
                lineLength += currentLength;
            }


            ///// sow the middle parts 
            // 0 : prev bottom
            // 1 : prev up
            // 2 : current up
            // 3 : current bottom

            List<UIVertex> tris = new List<UIVertex>();

            List<bool> cornerIsUp = new List<bool>();
            List<Vector2> topRow = new List<Vector2>();
            List<Vector2> bottomRow = new List<Vector2>();

            LineQuad prevQuad = null;
            LineQuad currQuad = null;
            
            // here we treat the overlap between the quads
            // by pushing them enough to create a gap where we can insert a clean triangle
            for (int i = 1; i < finaleVerts.Count; i++)
            {
                prevQuad = finaleVerts[i - 1];
                currQuad = finaleVerts[i];

                Vector3 middleIntersectopBothQuads = Vector2.Lerp(prevQuad.RightDownPos, prevQuad.RightUpPos, 0.5f);

                Vector3 middleOfFreeSpace = Vector2.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, 0.5f);

                float angle = Vector2.SignedAngle(middleOfFreeSpace - middleIntersectopBothQuads, currQuad.LeftUpPos - middleIntersectopBothQuads);

                if (angle > 0)
                {
                    middleOfFreeSpace = Vector2.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, 0.5f);

                    angle = 180 - 90 - angle;


                    angle *= Mathf.Deg2Rad;

                    // hyp = opp / sin(angle)

                    float hyp = (curveSettings.LineThikness * 0.5f) / Mathf.Sin(angle);

                    Vector3 convergeancePoint = middleIntersectopBothQuads + (((middleOfFreeSpace - middleIntersectopBothQuads).normalized) * hyp);

                    Vector2 inter = convergeancePoint;

                    // calculate new segment lengths
                    float newPrevLength = Vector2.Distance(prevQuad.LeftUpPos, inter);
                    float newCurrLength = Vector2.Distance(currQuad.RightUpPos, inter);

                    // replace with new intersection point
                    prevQuad.RightUpPos = inter;
                    currQuad.LeftUpPos = inter;

                    // push back the other row to align it with the new modified one
                    // prev segment 
                    Vector3 directionPrev = (prevQuad.RightDownPos - prevQuad.LeftDownPos).normalized;
                    prevQuad.RightDownPos = prevQuad.LeftDownPos + (directionPrev * newPrevLength);

                    // current segment
                    Vector3 directionCurr = (currQuad.RightDownPos - currQuad.LeftDownPos).normalized;
                    currQuad.LeftDownPos = currQuad.RightDownPos - (directionCurr * newCurrLength);


                    // TODO : use circle to define edges
                    // Add the triangle

                    // add rows and corners

                    cornerIsUp.Add(false);

                    topRow.Add(prevQuad.LeftUpPos);
                    topRow.Add(prevQuad.RightUpPos);

                    bottomRow.Add(prevQuad.LeftDownPos);
                    bottomRow.Add(prevQuad.RightDownPos);


                    for (int j = 1; j < curveSettings.CornerSmoothing; j++)
                    {

                        float t = j / (float)curveSettings.CornerSmoothing;
                        Vector2 l = Vector2.Lerp(prevQuad.RightDownPos, currQuad.LeftDownPos, t);
                        l = inter + ((l - inter).normalized * curveSettings.LineThikness);

                        // add interpolation
                        bottomRow.Add(l);
                    }

                }

                else
                {
                    middleOfFreeSpace = Vector2.Lerp(prevQuad.RightDownPos, currQuad.LeftDownPos, 0.5f);

                    angle = 180 - 90 - angle;


                    angle *= Mathf.Deg2Rad;

                    // hyp = opp / sin(angle)

                    float hyp = (curveSettings.LineThikness * 0.5f) / Mathf.Sin(angle);

                    Vector3 convergeancePoint = middleIntersectopBothQuads + (((middleOfFreeSpace - middleIntersectopBothQuads).normalized) * hyp);

                    Vector2 inter = convergeancePoint;

                    // calculate new segment lengths
                    float newPrevLength = Vector2.Distance(prevQuad.LeftDownPos, inter);
                    float newCurrLength = Vector2.Distance(currQuad.RightDownPos, inter);

                    // replace with new intersection point
                    prevQuad.RightDownPos = inter;
                    currQuad.LeftDownPos = inter;

                    // push back the other row to align it with the new modified one
                    // prev segment 
                    Vector3 directionPrev = (prevQuad.RightUpPos - prevQuad.LeftUpPos).normalized;
                    prevQuad.RightUpPos = prevQuad.LeftUpPos + (directionPrev * newPrevLength);

                    // current segment
                    Vector3 directionCurr = (currQuad.RightUpPos - currQuad.LeftUpPos).normalized;
                    currQuad.LeftUpPos = currQuad.RightUpPos - (directionCurr * newCurrLength);


                    // TODO : use circle to define edges
                    // Add the triangle

                    // add rows and corners

                    cornerIsUp.Add(true);

                    topRow.Add(prevQuad.LeftUpPos);
                    topRow.Add(prevQuad.RightUpPos);

                    bottomRow.Add(prevQuad.LeftDownPos);
                    bottomRow.Add(prevQuad.RightDownPos);


                    for (int j = 1; j < curveSettings.CornerSmoothing; j++)
                    {

                        float t = j / (float)curveSettings.CornerSmoothing;
                        Vector2 l = Vector2.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, t);
                        l = inter + ((l - inter).normalized * curveSettings.LineThikness);

                        // add interpolation
                        topRow.Add(l);
                    }


                }

            }

            topRow.Add(currQuad.LeftUpPos);
            topRow.Add(currQuad.RightUpPos);

            bottomRow.Add(currQuad.LeftDownPos);
            bottomRow.Add(currQuad.RightDownPos);

            // group all verts that belong to topline and bottomline together
            int topIndex = 0;
            int bottomIndex = 0;

            // top uv
            float topRowLength = 0;

            for (int i = 1; i < topRow.Count; i++)
            {
                topRowLength += Vector2.Distance(topRow[i], topRow[i - 1]);
            }

            List<float> uvTopRatios = new List<float>();
            uvTopRatios.Add(0);
            float currentTopUv = 0;
            for (int i = 1; i < topRow.Count; i++)
            {
                currentTopUv += Vector2.Distance(topRow[i], topRow[i - 1]);
                uvTopRatios.Add(currentTopUv / topRowLength);
            }


            // bottom uv
            float bottomRowLength = 0;

            for (int i = 1; i < bottomRow.Count; i++)
            {
                bottomRowLength += Vector2.Distance(bottomRow[i], bottomRow[i - 1]);
            }

            List<float> uvBottomRatios = new List<float>();
            uvBottomRatios.Add(0);
            float currentBottomUv = 0;
            for (int i = 1; i < bottomRow.Count; i++)
            {
                currentBottomUv += Vector2.Distance(bottomRow[i], bottomRow[i - 1]);
                uvBottomRatios.Add(currentBottomUv / bottomRowLength);
            }


            // create the triangles
            while (bottomIndex < bottomRow.Count - 1)
            {

                Vector2 uv0top = new Vector2(uvTopRatios[topIndex], 1f);
                UIVertex p1 = new UIVertex() { position = topRow[topIndex], uv0 = uv0top };

                topIndex++;
                uv0top = new Vector2(uvTopRatios[topIndex], 1f);
                UIVertex p2 = new UIVertex() { position = topRow[topIndex], uv0 = uv0top };

                Vector2 uv0bottom = new Vector2(uvBottomRatios[bottomIndex], 0f);
                UIVertex p3 = new UIVertex() { position = bottomRow[bottomIndex], uv0 = uv0bottom };

                bottomIndex++;
                uv0bottom = new Vector2(uvBottomRatios[bottomIndex], 0f);
                UIVertex p4 = new UIVertex() { position = bottomRow[bottomIndex], uv0 = uv0bottom };

                // TODO : multiple ways to calculate uv

                switch (curveSettings.UVSmoothing)
                {
                    case UVSmoothingType.NONE:
                        break;
                    case UVSmoothingType.INVERT:
                        {
                            // swap
                            float tmp = p1.uv0.x;
                            p1.uv0.x = p3.uv0.x;
                            p3.uv0.x = tmp;

                            tmp = p2.uv0.x;
                            p2.uv0.x = p4.uv0.x;
                            p4.uv0.x = tmp;
                        }
                        break;
                    case UVSmoothingType.LERP:
                        {
                            // swap
                            var tmp = Mathf.Lerp(p1.uv0.x, p3.uv0.x, curveSettings.UVSmoothLerp);
                            var inv = Mathf.Lerp(p1.uv0.x, p3.uv0.x, 1 - curveSettings.UVSmoothLerp);
                            p1.uv0.x = tmp;
                            p3.uv0.x = inv;

                            tmp = Mathf.Lerp(p2.uv0.x, p4.uv0.x, curveSettings.UVSmoothLerp);
                            inv = Mathf.Lerp(p2.uv0.x, p4.uv0.x, 1 - curveSettings.UVSmoothLerp);
                            p2.uv0.x = tmp;
                            p4.uv0.x = inv;
                        }
                        break;
                    default:
                        break;
                }


                //vbo.AddUIVertexQuad(quad);

                tris.Add(p1);
                tris.Add(p2);
                tris.Add(p3);


                tris.Add(p4);
                tris.Add(p3);
                tris.Add(p2);
            }


            return tris;
        }

        public static List<UIVertex> LineToCurve(List<Vector2> points, CurveSettings curveSettings, out float lineLength)
        {
            List<UIVertex> curve = SplineToCurve(points, curveSettings, out lineLength);

            return curve;
        }

        /// <summary>
        /// Find some projected angle measure off some forward around some axis.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="forward"></param>
        /// <param name="axis"></param>
        /// <returns>Angle in degrees</returns>
        public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis, bool clockwise = false)
        {
            Vector3 right;
            if (clockwise)
            {
                right = Vector3.Cross(forward, axis);
                forward = Vector3.Cross(axis, right);
            }
            else
            {
                right = Vector3.Cross(axis, forward);
                forward = Vector3.Cross(right, axis);
            }
            return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * Mathf.Rad2Deg;
        }

        /// Determines whether point P is inside the triangle ABC
        public static bool PointInTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            double s1 = C.y - A.y;
            double s2 = C.x - A.x;
            double s3 = B.y - A.y;
            double s4 = P.y - A.y;

            double w1 = (A.x * s1 + s4 * s2 - P.x * s1) / (s3 * s2 - (B.x - A.x) * s1);
            double w2 = (s4 - w1 * s3) / s1;
            return w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
        }

        /// <summary>
        /// Changes the vectors velue separatly and returns the vector with the new values
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Change(this Vector3 vec, float? x = null, float? y = null, float? z = null)
        {
            Vector3 res = vec;

            if (x != null)
                res.x = x.Value;

            if (y != null)
                res.y = y.Value;

            if (z != null)
                res.z = z.Value;

            return res;
        }

        /// <summary>
        /// Returns the angle of the vector in degrees
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static float ToAngleDeg(this Vector2 vec)
        {
            return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns the angle of the vector in radians
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static float ToAngleRad(this Vector2 vec)
        {
            return Mathf.Atan2(vec.y, vec.x);
        }

        /// <summary>
        /// Get a mask version of the vector
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Vector3 MulVector(this Vector3 original, float x = 1, float y = 1, float z = 1)
        {
            Vector3 res = original;
            res.x *= x;
            res.y *= y;
            res.z *= z;

            return res;
        }

        /// <summary>
        /// Get a mask version of the vector
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Color MulColor(this Color original, float r = 1, float g = 1, float b = 1, float a = 1)
        {
            Color res = original;
            res.r *= r;
            res.g *= g;
            res.b *= b;
            res.a *= a;

            return res;
        }

        /// <summary>
        /// return if 2 line intersect with the intersection point
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
        {

            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;

            float x1lo, x1hi, y1lo, y1hi;



            Ax = p2.x - p1.x;

            Bx = p3.x - p4.x;



            // X bound box test/

            if (Ax < 0)
            {

                x1lo = p2.x; x1hi = p1.x;

            }
            else
            {

                x1hi = p2.x; x1lo = p1.x;

            }



            if (Bx > 0)
            {

                if (x1hi < p4.x || p3.x < x1lo) return false;

            }
            else
            {

                if (x1hi < p3.x || p4.x < x1lo) return false;

            }



            Ay = p2.y - p1.y;

            By = p3.y - p4.y;



            // Y bound box test//

            if (Ay < 0)
            {

                y1lo = p2.y; y1hi = p1.y;

            }
            else
            {

                y1hi = p2.y; y1lo = p1.y;

            }



            if (By > 0)
            {

                if (y1hi < p4.y || p3.y < y1lo) return false;

            }
            else
            {

                if (y1hi < p3.y || p4.y < y1lo) return false;

            }



            Cx = p1.x - p3.x;

            Cy = p1.y - p3.y;

            d = By * Cx - Bx * Cy;  // alpha numerator//

            f = Ay * Bx - Ax * By;  // both denominator//



            // alpha tests//

            if (f > 0)
            {

                if (d < 0 || d > f) return false;

            }
            else
            {

                if (d > 0 || d < f) return false;

            }



            e = Ax * Cy - Ay * Cx;  // beta numerator//



            // beta tests //

            if (f > 0)
            {

                if (e < 0 || e > f) return false;

            }
            else
            {

                if (e > 0 || e < f) return false;

            }



            // check if they are parallel

            if (f == 0) return false;

            // compute intersection coordinates //

            num = d * Ax; // numerator //

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

            //    intersection.x = p1.x + (num+offset) / f;
            intersection.x = p1.x + num / f;



            num = d * Ay;

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;

            //    intersection.y = p1.y + (num+offset) / f;
            intersection.y = p1.y + num / f;



            return true;

        }
    }
}
