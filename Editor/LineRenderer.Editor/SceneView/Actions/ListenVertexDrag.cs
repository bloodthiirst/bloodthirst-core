using Bloodthirst.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.UI
{
    internal class ListenVertexDragEvent : IListenEvent
    {
        public UILineRendererSceneView SceneView { get; }

        private bool isDragging = false;

        private int vertexIndex = -1;

        public ListenVertexDragEvent(UILineRendererSceneView sceneView)
        {
            SceneView = sceneView;
        }

        void IListenEvent.Setup()
        {
        }

        private bool IsDownOnVertex(out int vertex)
        {
            Event currEvt = Event.current;

            Vector2 mouseScreenPos = HandleUtility.GUIPointToScreenPixelCoordinate(currEvt.mousePosition);

            for (int i = 0; i < SceneView.LineRenderer.spline.VertexData.Count; i++)
            {
                Vector2 curr = SceneView.LineRenderer.spline.VertexData[i];

                Vector3 pointWorld = SceneView.LineRenderer.rectTransform.TransformPoint(curr);

                Vector2 vertexScreenPos = UnityEditor.SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(pointWorld);

                float sizeMul = HandleUtility.GetHandleSize(pointWorld);

                bool isHovering = Vector2.Distance(vertexScreenPos, mouseScreenPos) <= (UILineRendererSceneView.VERTEX_SIZE * sizeMul * 2);

                if (isHovering)
                {
                    vertex = i;
                    return true;
                }
            }

            vertex = -1;
            return false;
        }

        private void PerformDrag()
        {
            if (!isDragging)
                return;

            Event currEvt = Event.current;

            if (currEvt.button != 1)
                return;

            Vector2 screenPos = HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition);
            Vector3 worldPos = UnityEditor.SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(screenPos);
            Vector3 localPos = SceneView.LineRenderer.rectTransform.InverseTransformPoint(worldPos);

            int pointIndex = vertexIndex;
            SceneView.LineRenderer.UpdatePoint(localPos, pointIndex);
            SceneView.Editor.Inspector.RefreshPoint(pointIndex);


            SceneView.LineRenderer.SetVerticesDirty();

            currEvt.Use();
        }

        void IListenEvent.OnSceneGUI()
        {
            Event currEvt = Event.current;

            if (!currEvt.isMouse)
                return;

            switch (currEvt.type)
            {
                case EventType.MouseDown:
                    {
                        isDragging = IsDownOnVertex(out vertexIndex);
                        break;
                    }
                case EventType.MouseUp:
                    {
                        isDragging = false;
                        vertexIndex = -1;
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        PerformDrag();
                        break;
                    }
                default:
                    break;
            }
        }

        void IListenEvent.Destroy()
        {
        }
    }
}
