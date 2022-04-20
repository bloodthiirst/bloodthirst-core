using UnityEngine;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for types / reflection
    /// </summary>
    public static class GraphicsUtils
    {
        public static void GetCorners(this Bounds bounds, Vector3[] points)
        {
            Vector3 boundPoint1 = bounds.min;
            Vector3 boundPoint2 = bounds.max;
            Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
            Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
            Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
            Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
            Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
            Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);

            points[0] = boundPoint1;
            points[1] = boundPoint2;
            points[2] = boundPoint3;
            points[3] = boundPoint4;
            points[4] = boundPoint5;
            points[5] = boundPoint6;
            points[6] = boundPoint7;
            points[7] = boundPoint8;
        }

        public static bool IsTotallyOutsideFrustum(Vector3[] boundsPoints, Camera cam)
        {
            for (int i = 0; i < 8; i++)
            {
                if (CanSeePoint(boundsPoints[i], cam))
                    return false;
            }

            return true;
        }

        public static bool CanSeePoint(Vector3 p, Camera cam)
        {
            Vector3 vp = cam.WorldToViewportPoint(p);
            if (vp.x < 0)
                return false;
            if (vp.x > 1)
                return false;
            if (vp.y < 0)
                return false;
            if (vp.y > 1)
                return false;
            if (vp.z < 0)
                return false;

            return true;
        }
    }
}