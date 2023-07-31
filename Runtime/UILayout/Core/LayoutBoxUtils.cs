#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public static class LayoutBoxUtils
    {
        public static float GetChildrenWidthSum(this ILayoutBox layoutBox)
        {
            float sum = 0f;

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                if (c.LayoutStyle.PositionType.PositionKeyword != PositionKeyword.DISPLAY_MODE)
                    continue;

                sum += c.Rect.width;
            }

            return sum;
        }

        public static float GetChildrenHeightSum(this ILayoutBox layoutBox)
        {
            float sum = 0f;

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                if (c.LayoutStyle.PositionType.PositionKeyword != PositionKeyword.DISPLAY_MODE)
                    continue;

                sum += c.Rect.height;
            }

            return sum;
        }
#if UNITY_EDITOR
        public static void DrawSingleLayout(ILayoutBox layoutBox, SceneView sv, UICanvasInfoBase canvasInfo)
        {
            DrawRect(layoutBox.Rect, sv, canvasInfo);
        }
        public static void DrawRect(Rect rect, SceneView sv, UICanvasInfoBase canvasInfo)
        {
            Vector3 p1 = canvasInfo.CanvasToWorld(rect.topLeft);
            Vector3 p2 = canvasInfo.CanvasToWorld(rect.topRight);
            Vector3 p3 = canvasInfo.CanvasToWorld(rect.bottomRight);
            Vector3 p4 = canvasInfo.CanvasToWorld(rect.bottomLeft);

            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p4);
            Handles.DrawLine(p4, p1);
        }
#endif
        public static bool IsInsideRect(Rect rect, Vector2 p)
        {
            if (p.x < rect.x)
                return false;

            if (p.x > rect.x + rect.width)
                return false;

            if (p.y < rect.y)
                return false;

            if (p.y > rect.y + rect.height)
                return false;

            return true;
        }

        public static void ClearChildren(ILayoutBox layoutBox)
        {
            for (int i = layoutBox.ChildLayouts.Count - 1; i >= 0; i--)
            {
                ILayoutBox child = layoutBox.ChildLayouts[i];

                layoutBox.RemoveChild(child);
                ClearChildren(child);
            }
        }

        public static void AddChildren(LayoutBehaviour layoutBox)
        {
            foreach (Transform t in layoutBox.transform)
            {
                if (t.TryGetComponent(out LayoutBehaviour l))
                {
                    layoutBox.AddChild(l);
                    AddChildren(l);
                }
            }
        }

        public static void ResetChildrenRects(ILayoutBox layoutBox)
        {
            for (int i = layoutBox.ChildLayouts.Count - 1; i >= 0; i--)
            {
                ILayoutBox child = layoutBox.ChildLayouts[i];
                child.Rect.x = 0;
                child.Rect.y = 0;
                child.Rect.width = 0;
                child.Rect.height = 0;

                child.PostFlow();

                ResetChildrenRects(child);
            }
        }
#if UNITY_EDITOR
        public static void DrawLayoutOutlines(ILayoutBox layoutBox, SceneView sv, UICanvasInfoBase canvasInfo)
        {
            Color elemCol = Color.white;

            DrawSingleLayout(layoutBox, sv, canvasInfo);

            int fontSize = 12;

            Vector3 worldSpace = canvasInfo.CanvasToWorld(layoutBox.Rect.center);

            GUIContent label = new GUIContent(layoutBox.Name);
            GUIStyleState txtStyle = new GUIStyleState() { textColor = elemCol };

            GUIStyle style = new GUIStyle()
            {
                normal = txtStyle,
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize
            };

            Handles.Label(worldSpace, label, style);

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                DrawLayoutOutlines(c, sv, canvasInfo);
            }
        }
#endif

    }

}
