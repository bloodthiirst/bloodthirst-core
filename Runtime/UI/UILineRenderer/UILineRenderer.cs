using Bloodthirst.Scripts.Utils;
using Bloodthirst.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Core.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : MaskableGraphic
    {
        [SerializeField]
        private CanvasRenderer canvasRenderer;

        /// <summary>
        /// Number of sub-parts (segements) in between each point
        /// </summary>
        [SerializeField]
        public int resolution = 10;

        /// <summary>
        /// Thickness of the line
        /// </summary>
        public float lineThickness = 2;

        /// <summary>
        /// Method used for UV smoothing
        /// </summary>
        public UVSmoothingType uvSmoothing;

        [ShowIf(nameof(uvSmoothing), Value = UVSmoothingType.LERP)]
        [Range(0f, 1f)]
        public float uvSmoothLerp;

        /// <summary>
        /// detail of defining the corners
        /// </summary>
        [Range(1, 10)]
        public int cornerSmoothing;

        public BSpline spline;

        public IReadOnlyList<Vector2> Points => points;

        /// <summary>
        /// The initial points that define the line (non smoothed)
        /// </summary>
        [SerializeField]
        private List<Vector2> points = new List<Vector2>();

        protected override void Awake()
        {
            base.Awake();
            InitPoints();
            spline = new BSpline(points);

            SetVerticesDirty();
            this.UpdateGeometry();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (spline == null)
            {
                spline = new BSpline(points);
            }

            SetVerticesDirty();
            this.UpdateGeometry();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if(canvasRenderer == null)
            {
                canvasRenderer = GetComponent<CanvasRenderer>();
            }

            if (!enabled)
                return;

            InitPoints();

            if (spline == null)
            {
                spline = new BSpline(points);
            }
            else
            {
                spline.SetVertexData(points);
            }
        }

        private void InitPoints()
        {
            if ( points.Count < 4)
            {
                points.Clear();
                points.Add(VectorUtils.NormalizedToLayoutSpace(rectTransform, new Vector2(0, 0)));
                points.Add(VectorUtils.NormalizedToLayoutSpace(rectTransform, new Vector2(0, 0.5f)));
                points.Add(VectorUtils.NormalizedToLayoutSpace(rectTransform, new Vector2(0.5f, 1f)));
                points.Add(VectorUtils.NormalizedToLayoutSpace(rectTransform, new Vector2(1f, 1f)));
            }
        }

        public void RemovePoint(int index)
        {
            spline.RemovePoint(index);

            SetVerticesDirty();
            this.UpdateGeometry();
        }

        public void UpdatePoint(Vector2 point, int index)
        {
            spline.UpdatePoint(point, index);

            SetVerticesDirty();
            this.UpdateGeometry();
        }


        public void AddPoint(Vector2 point)
        {
            spline.AddPoint(point);
        }


        [SerializeField]
        [HideInInspector]
        private List<Vector2> cachedInterpolatedPoints = new List<Vector2>();

        protected override void OnPopulateMesh(VertexHelper vbo)
        {
            cachedInterpolatedPoints.Clear();

            cachedInterpolatedPoints.Add(spline.VertexData[0]);

            for (int i = 0; i < spline.VertexData.Count - 3; i += 3)
            {
                for (int d = 0; d < resolution; d++)
                {
                    float t = (d + 1) / (float)(resolution);

                    Vector3 a = spline.GetPosition(i, t);

                    cachedInterpolatedPoints.Add(a);
                }
            }

            VectorUtils.CurveSettings settings = new VectorUtils.CurveSettings()
            {
                UVSmoothing = uvSmoothing,
                UVSmoothLerp = uvSmoothLerp,
                CornerSmoothing = cornerSmoothing,
                LineThikness = lineThickness,
                HandlesLength = 0,
                DetailPerSegment = resolution,
                NormalizeHandles = false,
                InvertHandles = false
            };

            List<UIVertex> tris = VectorUtils.SplineToCurve(cachedInterpolatedPoints, settings, out float lineLength);

            material.SetFloat("_LineLength", lineLength);

            vbo.Clear();
            vbo.AddUIVertexTriangleStream(tris);
        }

    }

}
