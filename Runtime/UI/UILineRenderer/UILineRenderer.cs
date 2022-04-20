using Bloodthirst.Scripts.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Core.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : MaskableGraphic
    {
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
        /// Margin used to give space between the line and the edge of the RectTransform
        /// </summary>
        public Vector2 Margin;

        /// <summary>
        /// Method used for UV smoothing
        /// </summary>
        public UVSmoothingType UVSmoothing;

        [ShowIf(nameof(UVSmoothing), Value = UVSmoothingType.LERP)]
        [Range(0f, 1f)]
        public float uvSmoothLerp;

        [SerializeField]
        /// <summary>
        /// The initial points that define the line (non smoothed)
        /// </summary>
        private Vector2[] points;

        public Vector2[] Points
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
        bool CheckPointsChanged(Vector2[] newPoints)
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
        protected override void OnPopulateMesh(VertexHelper vbo)
        {
            List<LineQuad> finaleVerts = new List<LineQuad>();

            if (points == null || points.Length < 2)
                points = new[] { new Vector2(0, 0), new Vector2(1, 1) };

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

            List<Vector2> scaledPoints = new List<Vector2>(points.Length);

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 curr = points[i];
                curr = new Vector2(curr.x * sizeX + offsetX, curr.y * sizeY + offsetY);
                scaledPoints.Add(curr);
            }

            vbo.Clear();
            float lineLength = 0;
            List<UIVertex> tris = VectorUtils.LineToCurve(scaledPoints, UVSmoothing, ref lineLength, uvSmoothLerp, cornerSmoothing, LineThikness, handlesLength, detailPerSegment, normalizeHandles, invertHandles);

            material.SetFloat("_LineLength", lineLength);

            vbo.AddUIVertexTriangleStream(tris);

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
