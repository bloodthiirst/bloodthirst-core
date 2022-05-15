using Bloodthirst.Core.UI;
using Bloodthirst.Editor.UI;
using Bloodthirst.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UILineRendererSceneView
{
    private GUIStyle lableStyle;

    internal const float VERTEX_SIZE = 0.1f;

    internal const float LINE_WIDTH = 3f;
    private List<IListenEvent> UIEvents { get; set; }

    internal UILineRendererEditor Editor { get; }

    internal UILineRenderer LineRenderer;

    public UILineRendererSceneView( UILineRendererEditor editor, UILineRenderer lineRenderer)
    {
        Editor = editor;
        LineRenderer = lineRenderer;
    }

    internal void OnEnable()
    {
        lableStyle = new GUIStyle()
        {
            contentOffset = new Vector2(10f, 10f),
            fontSize = 30,
            normal = new GUIStyleState() { textColor = Color.white },
            fontStyle = FontStyle.Bold
        };

        UIEvents = new List<IListenEvent>()
        {
            new ListenVertexDragEvent(this),
            new ListenVertexHoverEvent(this)
        };

        foreach (IListenEvent l in UIEvents)
        {
            l.Setup();
        }

    }
    internal void OnDisable()
    {
        foreach (IListenEvent l in UIEvents)
        {
            l.Destroy();
        }
    }
    internal void OnSceneGUI()
    {
        if (!LineRenderer.enabled)
            return;


        foreach (IListenEvent l in UIEvents)
        {
            l.OnSceneGUI();
        }


        DrawAllHandleVertices();
        DrawAllHandleLines();

        DrawAllPointVertices();
        DrawAllPointLines();

        DrawAllSmmothLines();

        SceneView.currentDrawingSceneView.Repaint();
    }

    private void DrawAllSmmothLines()
    {
        for (int i = 0; i < LineRenderer.spline.VertexData.Count - 3; i += 3)
        {
            for (int d = 0; d < LineRenderer.resolution; d++)
            {
                float t1 = d / (float)(LineRenderer.resolution);
                float t2 = (d + 1) / (float)(LineRenderer.resolution);

                Vector3 a = LineRenderer.spline.GetPosition(i, t1);
                Vector3 b = LineRenderer.spline.GetPosition(i, t2);

                DrawPointLine(a, b, Color.blue);
            }
        }
    }
    private void DrawAllHandleVertices()
    {
        // first
        {
            Vector2 handleLayout = LineRenderer.spline.VertexData[1];

            DrawHandleVertex(1, handleLayout);
        }

        // middle
        for (int i = 1; i < LineRenderer.spline.GetPointCount() - 1; i++)
        {
            int pointIndex = LineRenderer.spline.GetPointIndex(i);

            Vector2 prevHandleLayout = LineRenderer.spline.VertexData[pointIndex - 1];
            Vector2 nextHandleLayout = LineRenderer.spline.VertexData[pointIndex + 1];

            DrawHandleVertex(2, prevHandleLayout);
            DrawHandleVertex(1, nextHandleLayout);
        }

        // last
        {
            Vector2 handleLayout = LineRenderer.spline.VertexData[LineRenderer.spline.VertexData.Count - 2];

            DrawHandleVertex(2, handleLayout);
        }
    }
    private void DrawAllPointVertices()
    {
        // first
        {
            int pointIndex = LineRenderer.spline.GetPointIndex(0);

            Vector2 pointLayout = LineRenderer.spline.VertexData[pointIndex];

            DrawPointVertex(0, pointLayout);
        }

        // middle
        for (int i = 1; i < LineRenderer.spline.GetPointCount() - 1; i++)
        {
            int pointIndex = LineRenderer.spline.GetPointIndex(i);

            Vector2 pointLayout = LineRenderer.spline.VertexData[pointIndex];

            DrawPointVertex(i, pointLayout);
        }

        // last
        {
            int lastPoint = LineRenderer.spline.GetPointCount() - 1;
            int pointIndex = LineRenderer.spline.GetPointIndex(lastPoint);

            Vector2 pointLayout = LineRenderer.spline.VertexData[pointIndex];

            DrawPointVertex(lastPoint, pointLayout);
        }
    }
    private void DrawAllHandleLines()
    {
        // first
        {
            int pointIndex = LineRenderer.spline.GetPointIndex(0);

            Vector2 pointLayout = LineRenderer.spline.VertexData[pointIndex];
            Vector2 handleLayout = LineRenderer.spline.VertexData[pointIndex + 1];

            DrawHandleLine(pointLayout, handleLayout);
        }

        // middle
        for (int i = 1; i < LineRenderer.spline.GetPointCount() - 1; i++)
        {
            int pointIndex = LineRenderer.spline.GetPointIndex(i);

            Vector2 pointLayout = LineRenderer.spline.VertexData[pointIndex];
            Vector2 prevHandleLayout = LineRenderer.spline.VertexData[pointIndex - 1];
            Vector2 nextHandleLayout = LineRenderer.spline.VertexData[pointIndex + 1];

            DrawHandleLine(pointLayout, prevHandleLayout);
            DrawHandleLine(pointLayout, nextHandleLayout);
        }

        // last
        {
            int pointIndex = LineRenderer.spline.GetPointIndex(LineRenderer.spline.GetPointCount() - 1);

            Vector2 pointLayout = LineRenderer.spline.VertexData[pointIndex];
            Vector2 handleLayout = LineRenderer.spline.VertexData[pointIndex - 1];

            DrawHandleLine(pointLayout, handleLayout);
        }
    }
    private void DrawAllPointLines()
    {
        for (int i = 0; i < LineRenderer.spline.GetPointCount() - 1; i++)
        {
            int currIndex = LineRenderer.spline.GetPointIndex(i);
            int nextIndex = LineRenderer.spline.GetPointIndex(i + 1);

            Vector2 currP = LineRenderer.spline.VertexData[currIndex];
            Vector2 nextP = LineRenderer.spline.VertexData[nextIndex];

            DrawPointLine(currP, nextP, Color.green);
        }
    }

    #region draw line
    private void DrawHandleLine(Vector2 pointVert, Vector2 handleVert)
    {
        Vector3 pointWorld = LineRenderer.rectTransform.TransformPoint(pointVert);
        Vector3 handleWorld = LineRenderer.rectTransform.TransformPoint(handleVert);

        float sizeMulPoint = HandleUtility.GetHandleSize(pointWorld);
        float sizeMulHandle = HandleUtility.GetHandleSize(pointWorld);

        float avgMul = Mathf.Lerp(sizeMulPoint, sizeMulHandle, 0.5f);

        Handles.color = Color.red;
        Handles.DrawLine(pointWorld, handleWorld, LINE_WIDTH);
    }
    private void DrawPointLine(Vector2 from, Vector2 to, Color col)
    {
        Vector3 fromWorld = LineRenderer.rectTransform.TransformPoint(from);
        Vector3 toWorld = LineRenderer.rectTransform.TransformPoint(to);

        Handles.color = col;

        Handles.DrawLine(fromWorld, toWorld, LINE_WIDTH);
    }
    #endregion

    #region draw outline
    internal void DrawOutlineVertex(Vector2 layoutSpace)
    {
        Vector3 pointWorld = LineRenderer.rectTransform.TransformPoint(layoutSpace);

        float sizeMul = HandleUtility.GetHandleSize(pointWorld);

        Handles.color = Color.yellow;

        Handles.DrawWireDisc(pointWorld, Vector3.forward, (VERTEX_SIZE * sizeMul) + 2, LINE_WIDTH);
    }
    #endregion

    #region draw vertex
    private void DrawPointVertex(int index, Vector2 layoutSpace)
    {
        Vector3 pointWorld = LineRenderer.rectTransform.TransformPoint(layoutSpace);

        float sizeMul = HandleUtility.GetHandleSize(pointWorld);

        Handles.color = Color.green;

        Handles.Label(pointWorld, index.ToString(), lableStyle);

        Handles.DrawSolidDisc(pointWorld, Vector3.forward, VERTEX_SIZE * sizeMul);

    }

    private void DrawHandleVertex(int index, Vector2 layoutSpace)
    {
        Vector3 pointWorld = LineRenderer.rectTransform.TransformPoint(layoutSpace);

        float sizeMul = HandleUtility.GetHandleSize(pointWorld);

        Handles.color = Color.red;

        Handles.Label(pointWorld, index.ToString(), lableStyle);

        Handles.DrawSolidDisc(pointWorld, Vector3.forward, VERTEX_SIZE * sizeMul);
    }
    #endregion
}
