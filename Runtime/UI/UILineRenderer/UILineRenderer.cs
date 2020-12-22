using Assets.Scripts.Utils;
using Bloodthirst.Utils;
using Packages.com.bloodthirst.bloodthirst_core.Runtime.UI.UILineRenderer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : MaskableGraphic
    {
        [SerializeField]
        private int SPLINE_DEFINITION = 10;
        public float LineThikness = 2;
        public bool UseMargins;

        [SerializeField]
        private bool useSpline;

        [SerializeField]
        private float handlesLength;

        [SerializeField]
        private bool normalizeHandles;

        [SerializeField]
        private bool invertHandles;

        public Vector2 Margin;
        public Vector2[] Points;

        [Range(1, 10)]
        public int cornerSmoothing;
        protected override void OnPopulateMesh(VertexHelper vbo)
        {
            List<LineQuad> finaleVerts = new List<LineQuad>();

            if (Points == null || Points.Length < 2)
                Points = new[] { new Vector2(0, 0), new Vector2(1, 1) };

            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

            if (UseMargins)
            {
                sizeX -= Margin.x;
                sizeY -= Margin.y;
                offsetX += Margin.x / 2f;
                offsetY += Margin.y / 2f;
            }

            vbo.Clear();

            Vector2 prevVUp = Vector2.zero;
            Vector2 prevVBottom = Vector2.zero;

            float totalLength = 0;
            Spline<Node2D> spline = new Spline<Node2D>();
            List<Vector2> InterpolatedPoints = new List<Vector2>();

            if (useSpline)
            {
                List<Node2D> nodes = Points.Select(v => new Node2D() { point = v }).ToList();

                
                spline.NormalizeHandles = normalizeHandles;
                spline.HandleLength = handlesLength;
                spline.InvertHandlesLengths = invertHandles;

                spline.Initialize(nodes, null, null, true);

                totalLength = spline.GetTotalLength();

                material.SetFloat("_LineLength", totalLength);

                for (int i = 0; i < spline.SegmentCount; i++)
                {
                    for (int j = 0; j < SPLINE_DEFINITION; j++)
                    {
                        float t = j / (float)(SPLINE_DEFINITION);
                        Vector2 v2 = spline[i].GetPoint(t);
                        InterpolatedPoints.Add(v2);
                    }

                    InterpolatedPoints.Add(spline[i].GetPoint(1f));
                }
            }
            else
            {
                InterpolatedPoints = Points.ToList();
            }

            float prevLength = 0;
            float currentLength = 0;

            for (int i = 1; i < InterpolatedPoints.Count; i++)
            {
                Vector2 prev = InterpolatedPoints[i - 1];
                Vector2 cur = InterpolatedPoints[i];

                if (cur == prev)
                    continue;


                currentLength += (cur - prev).magnitude;

                prev = new Vector2(prev.x * sizeX + offsetX, prev.y * sizeY + offsetY);
                cur = new Vector2(cur.x * sizeX + offsetX, cur.y * sizeY + offsetY);

                float angle = Mathf.Atan2(cur.y - prev.y, cur.x - prev.x) * 180f / Mathf.PI;

                var prevBottom = prev + new Vector2(0, -LineThikness / 2);
                var prevUp = prev + new Vector2(0, +LineThikness / 2);
                var currentUp = cur + new Vector2(0, +LineThikness / 2);
                var currentBottom = cur + new Vector2(0, -LineThikness / 2);

                prevBottom = RotatePointAroundPivot(prevBottom, prev, new Vector3(0, 0, angle));
                prevUp = RotatePointAroundPivot(prevUp, prev, new Vector3(0, 0, angle));
                currentUp = RotatePointAroundPivot(currentUp, cur, new Vector3(0, 0, angle));
                currentBottom = RotatePointAroundPivot(currentBottom, cur, new Vector3(0, 0, angle));

                float ratioBeforePrev = (i - 2) / (float)(InterpolatedPoints.Count - 1);
                float ratioPrev = (i - 1) / (float)(InterpolatedPoints.Count - 1);
                float ratioCurr = (i) / (float)(InterpolatedPoints.Count - 1);

                LineQuad quad = new LineQuad()
                {
                    RightDownPos = currentBottom,
                    RightUpPos = currentUp,
                    LeftDownPos = prevBottom,
                    LeftUpPos = prevUp,

                    LeftDownUV = new Vector2(prevLength / totalLength, 0),
                    LeftUpUV = new Vector2(prevLength / totalLength, 1),
                    RightDownUV = new Vector2(currentLength / totalLength, 0),
                    RightUpUV = new Vector2(currentLength / totalLength, 1)
                };

                finaleVerts.Add(quad);
                prevLength = currentLength;
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
            // TODO handle opposite angle
            for (int i = 1; i < finaleVerts.Count; i++)
            {
                prevQuad = finaleVerts[i - 1];
                currQuad = finaleVerts[i];

                Vector2 trueMiddle = Vector2.Lerp(prevQuad.RightDownPos, prevQuad.RightUpPos, 0.5f);

                Vector2 middleOfLineThrough = Vector2.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, 0.5f);

                float angle = Vector2.SignedAngle(middleOfLineThrough - trueMiddle, currQuad.LeftUpPos - trueMiddle);

                Debug.Log($"angle {angle}");

                if (angle > 0)
                {
                    middleOfLineThrough = Vector2.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, 0.5f);

                    angle = 180 - 90 - angle;


                    angle *= Mathf.Deg2Rad;

                    // hyp = opp / sin(angle)

                    float hyp = (LineThikness * 0.5f) / Mathf.Sin(angle);

                    Vector3 convergeancePoint = trueMiddle + (((middleOfLineThrough - trueMiddle).normalized) * hyp);

                    Vector2 inter = convergeancePoint;

                    // calculate new segment lengths
                    float newPrevLength = Vector2.Distance(prevQuad.LeftUpPos, inter);
                    float newCurrLength = Vector2.Distance(currQuad.RightUpPos, inter);

                    // replace with new intersection point
                    prevQuad.RightUpPos = inter;
                    currQuad.LeftUpPos = inter;

                    // push back the other row to align it with the new modified one
                    // prev segment 
                    Vector2 directionPrev = (prevQuad.RightDownPos - prevQuad.LeftDownPos).normalized;
                    prevQuad.RightDownPos = prevQuad.LeftDownPos + (directionPrev * newPrevLength);

                    // current segment
                    Vector2 directionCurr = (currQuad.RightDownPos - currQuad.LeftDownPos).normalized;
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
                        Vector2 l = Vector2.Lerp(prevQuad.RightDownPos, currQuad.LeftDownPos, t);
                        l = inter + ((l - inter).normalized * LineThikness);

                        // add interpolation
                        bottomRow.Add(l);
                    }
                    
                }

                else
                {
                    middleOfLineThrough = Vector2.Lerp(prevQuad.RightDownPos, currQuad.LeftDownPos, 0.5f);

                    angle = 180 - 90 - angle;


                    angle *= Mathf.Deg2Rad;

                    // hyp = opp / sin(angle)

                    float hyp = (LineThikness * 0.5f) / Mathf.Sin(angle);

                    Vector3 convergeancePoint = trueMiddle + (((middleOfLineThrough - trueMiddle).normalized) * hyp);

                    Vector2 inter = convergeancePoint;

                    // calculate new segment lengths
                    float newPrevLength = Vector2.Distance(prevQuad.LeftDownPos, inter);
                    float newCurrLength = Vector2.Distance(currQuad.RightDownPos, inter);

                    // replace with new intersection point
                    prevQuad.RightDownPos = inter;
                    currQuad.LeftDownPos = inter;

                    // push back the other row to align it with the new modified one
                    // prev segment 
                    Vector2 directionPrev = (prevQuad.RightUpPos - prevQuad.LeftUpPos).normalized;
                    prevQuad.RightUpPos = prevQuad.LeftUpPos + (directionPrev * newPrevLength);

                    // current segment
                    Vector2 directionCurr = (currQuad.RightUpPos - currQuad.LeftUpPos).normalized;
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
                        Vector2 l = Vector2.Lerp(prevQuad.RightUpPos, currQuad.LeftUpPos, t);
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
            
            // TODO : massage the UVs down
            // group all verts that belong to topline and bottomline together

            Debug.Log($"top row cnt : {topRow.Count} , bottom row cnt : {bottomRow.Count} , corner { cornerIsUp.Count}");

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


            // TODO : calculate uv based on length ratio both BOTH top row AND bottom row

            // create the triangles
            while (bottomIndex < bottomRow.Count -1 )
            {

                Vector2 uv0top = new Vector2( uvTopRatios[topIndex] , 1f);
                UIVertex p1 = new UIVertex() { position = topRow[topIndex] , uv0 = uv0top };

                topIndex++;
                uv0top = new Vector2(uvTopRatios[topIndex], 1f);
                UIVertex p2 = new UIVertex() { position = topRow[topIndex], uv0 = uv0top };

                Vector2 uv0bottom = new Vector2(uvBottomRatios[bottomIndex] , 0f);
                UIVertex p3 = new UIVertex() { position = bottomRow[bottomIndex] , uv0 = uv0bottom };

                bottomIndex++;
                uv0bottom = new Vector2(uvBottomRatios[bottomIndex], 0f);
                UIVertex p4 = new UIVertex() { position = bottomRow[bottomIndex], uv0 = uv0bottom };

                //vbo.AddUIVertexQuad(quad);

                tris.Add(p1);
                tris.Add(p2);
                tris.Add(p3);


                tris.Add(p4);
                tris.Add(p3);
                tris.Add(p2);
            }


            vbo.AddUIVertexTriangleStream(tris);

            /*
            // send the vertices to the mesh
            for (int i = 0; i < finaleVerts.Count; i++)
            {
                UIVertex[] quad = new UIVertex[4];

                LineQuad lineQuad = finaleVerts[i];

                quad[0] = new UIVertex() { position = lineQuad.LeftDownPos, color = color, uv0 = lineQuad.LeftDownUV };
                quad[1] = new UIVertex() { position = lineQuad.LeftUpPos, color = color, uv0 = lineQuad.LeftUpUV };
                quad[2] = new UIVertex() { position = lineQuad.RightUpPos, color = color, uv0 = lineQuad.RightUpUV };
                quad[3] = new UIVertex() { position = lineQuad.RightDownPos, color = color, uv0 = lineQuad.RightDownUV };

                vbo.AddUIVertexQuad(quad);
            }
            */
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }

}
