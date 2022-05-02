using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.UI
{
    internal class ListenVertexHoverEvent : IListenEvent
    {
        public UILineRendererSceneView SceneView { get; }
        public ListenVertexHoverEvent(UILineRendererSceneView sceneView)
        {
            SceneView = sceneView;
        }

        void IListenEvent.Setup()
        {
        }

        private bool IsHoveringOnVertex(out int vertex)
        {
            Event currEvt = Event.current;

            Vector2 mouseScreenPos = HandleUtility.GUIPointToScreenPixelCoordinate(currEvt.mousePosition);

            for (int i = 0; i < SceneView.LineRenderer.spline.VertexData.Count; i++)
            {
                Vector2 curr = SceneView.LineRenderer.spline.VertexData[i];

                Vector3 pointWorld = SceneView.LineRenderer.rectTransform.TransformPoint(curr);

                Vector2 vertexScreenPos = UnityEditor.SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(pointWorld);

                float sizeMul = HandleUtility.GetHandleSize(pointWorld);

                bool isHovering = Vector2.Distance(vertexScreenPos, mouseScreenPos) <= (UILineRendererSceneView.VERTEX_SIZE * sizeMul);

                if (isHovering)
                {
                    vertex = i;
                    return true;
                }
            }

            vertex = -1;
            return false;
        }

        void IListenEvent.OnSceneGUI()
        {
            if (!IsHoveringOnVertex(out int vertex))
                return;

            Vector2 layoutPos = SceneView.LineRenderer.spline.VertexData[vertex];
            SceneView.DrawOutlineVertex(layoutPos);
        }

        void IListenEvent.Destroy()
        {
        }
    }
}
