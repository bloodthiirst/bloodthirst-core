using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.Utils
{
    public static  class Bezier
    {
        public static Vector3 GetPoint( Vector3 p1 , Vector3 h1 , Vector3 h2 , Vector3 p2 , float t)
        {
            return GetQuadraticBezier(p1, h1, h2, p2, t);
        }

        public static Vector3 GetNormal(Vector3 p1, Vector3 h1, Vector3 h2, Vector3 p2, float t)
        {
            return GetFirstDerivative(p1, h1, h2, p2, t);
        }

        public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }
        public static Vector3 GetQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var a = GetBezierPoint(p0, p1, p2, t);
            var b = GetBezierPoint(p1, p2, p3, t);

            return Vector3.Lerp(a, b, t);
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }

    }
}
