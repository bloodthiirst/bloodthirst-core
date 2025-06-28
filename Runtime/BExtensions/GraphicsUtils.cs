using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for types / reflection
    /// </summary>
    public static class GraphicsUtils
    {
        public static Rect ComputeScreenSize(GameObject gameObject, Camera cam, Vector3 worldPos )
        {
            Bounds bounds = GraphicsUtils.GetBounds(gameObject);

            using (ListPool<Vector3>.Get(out List<Vector3> cornersWorld))
            using (ListPool<Vector3>.Get(out List<Vector3> cornersScreen))
            {
                CollectionsUtils.ExpandeSize(cornersWorld, 8);
                GraphicsUtils.GetCorners(bounds, cornersWorld);

                for (int i = 0; i < cornersWorld.Count; i++)
                {
                    Vector3 c = cornersWorld[i];
                    c += worldPos;
                    Vector3 screenPos = cam.WorldToScreenPoint(c);
                    cornersScreen.Add(screenPos);
                }

                Vector2 xMinMax = new Vector2(cornersScreen[0].x, float.NegativeInfinity);
                Vector2 yMinMax = new Vector2(float.PositiveInfinity, float.NegativeInfinity);

                for (int i = 0; i < cornersScreen.Count; i++)
                {
                    Vector3 curr = cornersScreen[i];
                    xMinMax.x = Mathf.Min(curr.x, xMinMax.x);
                    xMinMax.y = Mathf.Max(curr.x, xMinMax.y);

                    yMinMax.x = Mathf.Min(curr.y, yMinMax.x);
                    yMinMax.y = Mathf.Max(curr.y, yMinMax.y);
                }

                Vector2 size = new Vector2(xMinMax.y - xMinMax.x, yMinMax.y - yMinMax.x);
                Rect rect = new Rect(xMinMax.x , yMinMax.x , size.x , size.y);

                return rect;
            }
        }

        public static Bounds GetBounds(GameObject obj)
        {
            Bounds bounds = new Bounds();
            using (ListPool<Renderer>.Get(out List<Renderer> renderers))
            {
                obj.GetComponentsInChildren<Renderer>(renderers);

                //Encapsulate for all renderers
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.enabled)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }

                return bounds;
            }
        }

        public static void GetCorners(this Bounds bounds, IList<Vector3> points)
        {
            Assert.IsTrue(points.Count == 8);
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