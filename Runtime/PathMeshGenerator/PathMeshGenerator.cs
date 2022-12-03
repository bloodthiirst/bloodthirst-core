using Bloodthirst.Core.UI;
using Bloodthirst.Scripts.Utils;
using Bloodthirst.Utils;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PathMeshGenerator : MonoBehaviour
    {
        /// <summary>
        /// Renderer used to show the mesh
        /// </summary>
        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private Material material;

        /// <summary>
        /// Number of sub-parts (segements) in between each point
        /// </summary>
        [SerializeField]
        [Range(3, 100)]
        private int detailPerSegment = 10;

        /// <summary>
        /// Thickness of the line
        /// </summary>
        public float LineThikness = 2;

        /// <summary>
        /// Use margins ?
        /// </summary>
        public bool UseMargins;


        /// <summary>
        /// Handle length of the spline controls
        /// </summary>
        [SerializeField]
        private float handlesLength;

        /// <summary>
        /// Normalize the spline handles ?
        /// </summary>
        [SerializeField]
        private bool normalizeHandles;

        /// <summary>
        /// Invert the spline handles ?
        /// </summary>
        [SerializeField]
        private bool invertHandles;

        /// <summary>
        /// Method used for UV smoothing
        /// </summary>
        public UVSmoothingType UVSmoothing;

#if ODIN_INSPECTOR
        [ShowIf(nameof(UVSmoothing), Value = UVSmoothingType.LERP)]
#endif
        [Range(0f, 1f)]
        public float uvSmoothLerp;

        [SerializeField]
        /// <summary>
        /// The initial points that define the line (non smoothed)
        /// </summary>
        private Vector3[] points;

        public Vector3[] Points
        {
            get => points;
            set
            {
                if (CheckPointsChanged(value))
                {
                    points = value;
                }
            }
        }

        [Range(1, 10)]

        /// <summary>
        /// detail of defining the corners
        /// </summary>
        public int cornerSmoothing;

        /// <summary>
        /// Check if array of points is equal to the current array of points
        /// </summary>
        /// <param name="newPoints"></param>
        /// <returns></returns>
        bool CheckPointsChanged(Vector3[] newPoints)
        {
            if (newPoints.Length != points.Length)
                return true;

            for (int i = 0; i < newPoints.Length; i++)
            {
                if (newPoints[i] != points[i])
                    return true;
            }

            return false;
        }


        
#if ODIN_INSPECTOR
[Button]
#endif

        public void GenerateMesh()
        {
            Mesh mesh = new Mesh();

            lineQuads = new List<LineQuad>();

            if (points == null || points.Length < 2)
                points = new[] { new Vector3(0, 0, 0), new Vector3(1, 1, 1) };

            float totalLength = 0;
            Spline<Node3D> spline = new Spline<Node3D>();
            InterpolatedPoints = new List<Vector3>();


            List<Node3D> nodes = points.Select(v => new Node3D() { point = v }).ToList();


            spline.NormalizeHandles = normalizeHandles;
            spline.HandleLength = handlesLength;
            spline.InvertHandlesLengths = invertHandles;

            spline.Initialize(nodes, null, null, true);

            totalLength = spline.GetTotalLength();


            for (int i = 0; i < spline.SegmentCount; i++)
            {
                for (int j = 0; j < detailPerSegment; j++)
                {
                    float t = j / (float)(detailPerSegment);
                    Vector3 v2 = spline[i].GetPoint(t);
                    InterpolatedPoints.Add(v2);
                }

                InterpolatedPoints.Add(spline[i].GetPoint(1f));
            }

            float prevLength = 0;
            float currentLength = 0;

            for (int i = 1; i < InterpolatedPoints.Count; i++)
            {
                Vector3 prev = InterpolatedPoints[i - 1];
                Vector3 cur = InterpolatedPoints[i];

                if (cur == prev)
                    continue;

                Vector3 dir = cur - prev;

                //float angle = Mathf.Atan2(dir.y, dir.x) * 180f / Mathf.PI;

                currentLength += (dir).magnitude;

                Vector3 up = Vector3.up;
                Vector3 right = Vector3.Cross(dir.normalized, up).normalized;
                Vector3 trueUp = Vector3.Cross(right, dir.normalized).normalized;
                Vector3 trueRight = Vector3.Cross(dir.normalized, trueUp).normalized;

                var prevBottom = prev + (trueRight * (-LineThikness / 2));
                var prevUp = prev + (trueRight * (+LineThikness / 2));
                var currentUp = cur + (trueRight * (+LineThikness / 2));
                var currentBottom = cur + (trueRight * (-LineThikness / 2));
                /*
                prevBottom = RotatePointAroundPivot(prevBottom, prev, new Vector3(0, 0, angle));
                prevUp = RotatePointAroundPivot(prevUp, prev, new Vector3(0, 0, angle));
                currentUp = RotatePointAroundPivot(currentUp, cur, new Vector3(0, 0, angle));
                currentBottom = RotatePointAroundPivot(currentBottom, cur, new Vector3(0, 0, angle));
                */
                float ratioBeforePrev = (i - 2) / (float)(InterpolatedPoints.Count - 1);
                float ratioPrev = (i - 1) / (float)(InterpolatedPoints.Count - 1);
                float ratioCurr = (i) / (float)(InterpolatedPoints.Count - 1);

                LineQuad quad = new LineQuad()
                {
                    RightDownPos = currentBottom,
                    RightUpPos = currentUp,
                    LeftDownPos = prevBottom,
                    LeftUpPos = prevUp,
                    Normal = trueUp
                };

                lineQuads.Add(quad);
                prevLength = currentLength;
            }

            material.SetFloat("_LineLength", currentLength);

            List<UIVertex> tris = new List<UIVertex>();

            List<bool> cornerIsUp = new List<bool>();
            List<Vector3> topRow = new List<Vector3>();
            List<Vector3> bottomRow = new List<Vector3>();

            LineQuad prevQuad = null;
            LineQuad currQuad = null;

#region sowing

            ///// sow the middle parts 
            // 0 : prev bottom
            // 1 : prev up
            // 2 : current up
            // 3 : current bottom


            // TODO handle opposite angle
            for (int i = 1; i < lineQuads.Count; i++)
            {
                prevQuad = lineQuads[i - 1];
                currQuad = lineQuads[i];

                Vector3 trueMiddle = Vector3.Lerp(prevQuad.RightDownPos, prevQuad.RightUpPos, 0.5f);

                Vector3 middleOfLineThrough = Vector3.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, 0.5f);

                Vector3 middleUp = Vector3.Lerp(prevQuad.Normal, currQuad.Normal, 0.5f);

                float angle = VectorUtils.AngleOffAroundAxis(middleOfLineThrough - trueMiddle, currQuad.LeftUpPos - trueMiddle, middleUp);

                if (angle > 0)
                {
                    middleOfLineThrough = Vector3.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, 0.5f);

                    angle = 180 - 90 - angle;


                    angle *= Mathf.Deg2Rad;

                    // hyp = opp / sin(angle)

                    float hyp = (LineThikness * 0.5f) / Mathf.Sin(angle);

                    Vector3 convergeancePoint = trueMiddle + (((middleOfLineThrough - trueMiddle).normalized) * hyp);

                    Vector3 inter = convergeancePoint;

                    // calculate new segment lengths
                    float newPrevLength = Vector3.Distance(prevQuad.LeftUpPos, inter);
                    float newCurrLength = Vector3.Distance(currQuad.RightUpPos, inter);

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


                    for (int j = 1; j < cornerSmoothing; j++)
                    {

                        float t = j / (float)cornerSmoothing;
                        Vector3 l = Vector3.Lerp(prevQuad.RightDownPos, currQuad.LeftDownPos, t);
                        l = inter + ((l - inter).normalized * LineThikness);

                        // add interpolation
                        bottomRow.Add(l);
                    }

                }

                else
                {
                    middleOfLineThrough = Vector3.Lerp(prevQuad.RightDownPos, currQuad.LeftDownPos, 0.5f);

                    angle = 180 - 90 - angle;


                    angle *= Mathf.Deg2Rad;

                    // hyp = opp / sin(angle)

                    float hyp = (LineThikness * 0.5f) / Mathf.Sin(angle);

                    Vector3 convergeancePoint = trueMiddle + (((middleOfLineThrough - trueMiddle).normalized) * hyp);

                    Vector3 inter = convergeancePoint;

                    // calculate new segment lengths
                    float newPrevLength = Vector3.Distance(prevQuad.LeftDownPos, inter);
                    float newCurrLength = Vector3.Distance(currQuad.RightDownPos, inter);

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


                    for (int j = 1; j < cornerSmoothing; j++)
                    {

                        float t = j / (float)cornerSmoothing;
                        Vector3 l = Vector3.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, t);
                        l = inter + ((l - inter).normalized * LineThikness);

                        // add interpolation
                        topRow.Add(l);
                    }


                }

            }

            topRow.Add(currQuad.LeftUpPos);
            topRow.Add(currQuad.RightUpPos);

            bottomRow.Add(currQuad.LeftDownPos);
            bottomRow.Add(currQuad.RightDownPos);

#endregion

            // TODO : massage the UVs down
            // group all verts that belong to topline and bottomline together

            int topIndex = 0;
            int bottomIndex = 0;

            // top uv
            float topRowLength = 0;

            for (int i = 1; i < topRow.Count; i++)
            {
                topRowLength += Vector3.Distance(topRow[i], topRow[i - 1]);
            }

            List<float> uvTopRatios = new List<float>();
            uvTopRatios.Add(0);
            float currentTopUv = 0;
            for (int i = 1; i < topRow.Count; i++)
            {
                currentTopUv += Vector3.Distance(topRow[i], topRow[i - 1]);
                uvTopRatios.Add(currentTopUv / topRowLength);
            }


            // bottom uv
            float bottomRowLength = 0;

            for (int i = 1; i < bottomRow.Count; i++)
            {
                bottomRowLength += Vector3.Distance(bottomRow[i], bottomRow[i - 1]);
            }

            List<float> uvBottomRatios = new List<float>();
            uvBottomRatios.Add(0);
            float currentBottomUv = 0;
            for (int i = 1; i < bottomRow.Count; i++)
            {
                currentBottomUv += Vector3.Distance(bottomRow[i], bottomRow[i - 1]);
                uvBottomRatios.Add(currentBottomUv / bottomRowLength);
            }


            // create the triangles
            while (bottomIndex < bottomRow.Count - 1)
            {

                Vector3 uv0top = new Vector3(uvTopRatios[topIndex], 1f);
                UIVertex p1 = new UIVertex() { position = topRow[topIndex], uv0 = uv0top };

                topIndex++;
                uv0top = new Vector3(uvTopRatios[topIndex], 1f);
                UIVertex p2 = new UIVertex() { position = topRow[topIndex], uv0 = uv0top };

                Vector3 uv0bottom = new Vector3(uvBottomRatios[bottomIndex], 0f);
                UIVertex p3 = new UIVertex() { position = bottomRow[bottomIndex], uv0 = uv0bottom };

                bottomIndex++;
                uv0bottom = new Vector3(uvBottomRatios[bottomIndex], 0f);
                UIVertex p4 = new UIVertex() { position = bottomRow[bottomIndex], uv0 = uv0bottom };

                // TODO : multiple ways to calculate uv

                switch (UVSmoothing)
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
                            var tmp = Mathf.Lerp(p1.uv0.x, p3.uv0.x, uvSmoothLerp);
                            var inv = Mathf.Lerp(p1.uv0.x, p3.uv0.x, 1 - uvSmoothLerp);
                            p1.uv0.x = tmp;
                            p3.uv0.x = inv;

                            tmp = Mathf.Lerp(p2.uv0.x, p4.uv0.x, uvSmoothLerp);
                            inv = Mathf.Lerp(p2.uv0.x, p4.uv0.x, 1 - uvSmoothLerp);
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

            List<Vector3> pointsIn3d = tris.Select(v => v.position).ToList();
            List<int> trianglesIndexes = tris.Select((v, i) => i).ToList();
            List<Vector2> uvs = tris.Select(v => new Vector2(v.uv0.x, v.uv0.y)).ToList();

            Vector3 center = Vector3.zero;
            center.x = pointsIn3d.Average(v => v.x);
            center.y = pointsIn3d.Average(v => v.y);
            center.z = pointsIn3d.Average(v => v.z);

            //transform.position = center;

            /*
            for(int i = 0; i < pointsIn3d.Count;i++)
            {
                pointsIn3d[i] = transform.worldToLocalMatrix * pointsIn3d[i];
            }
            */
            mesh.SetVertices(pointsIn3d);
            mesh.SetTriangles(trianglesIndexes, 0);
            mesh.SetUVs(0, uvs);

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            mesh.Optimize();

            meshFilter.mesh = mesh;
        }

        private List<Vector3> InterpolatedPoints;

        private List<LineQuad> lineQuads;

        private void OnDrawGizmos()
        {
            if (InterpolatedPoints == null)
                return;

            for (int i = 0; i < InterpolatedPoints.Count - 1; i++)
            {
                var curr = InterpolatedPoints[i];
                var next = InterpolatedPoints[i + 1];

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(curr, 2);
                Gizmos.DrawSphere(next, 2);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(curr, next);
            }

            if (lineQuads == null)
                return;
            Gizmos.color = Color.green;

            for (int i = 0; i < lineQuads.Count; i++)
            {
                var q = lineQuads[i];

                Gizmos.DrawLine(q.LeftDownPos, q.LeftUpPos);
                Gizmos.DrawLine(q.LeftUpPos, q.RightUpPos);
                Gizmos.DrawLine(q.RightUpPos, q.RightDownPos);
                Gizmos.DrawLine(q.RightDownPos, q.LeftDownPos);
            }

        }

        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }

}
