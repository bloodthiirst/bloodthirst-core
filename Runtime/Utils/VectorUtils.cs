using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class VectorUtils
    {
        /// <summary>
        /// Changes the vectors velue separatly and returns the vector with the new values
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Change( this Vector3 vec , float? x = null , float? y = null, float? z = null)
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
        public static Vector3 MulVector(this Vector3 original , float x = 1 , float y = 1 , float z = 1)
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
        public static Color MulColor(this Color original, float r = 1, float g = 1, float b = 1 , float a = 1)
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
