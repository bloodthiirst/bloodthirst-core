using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        public static void DrawRect(Rect rect)
        {
            Gizmos.DrawLine(ScreenToWorld(rect.topLeft), ScreenToWorld(rect.topRight));
            Gizmos.DrawLine(ScreenToWorld(rect.topRight), ScreenToWorld(rect.bottomRight));
            Gizmos.DrawLine(ScreenToWorld(rect.bottomRight), ScreenToWorld(rect.bottomLeft));
            Gizmos.DrawLine(ScreenToWorld(rect.bottomLeft), ScreenToWorld(rect.topLeft));
        }

        public static void DrawSingleLayout(ILayoutBox layoutBox)
        {
            Gizmos.DrawLine(ScreenToWorld(layoutBox.Rect.topLeft), ScreenToWorld(layoutBox.Rect.topRight));
            Gizmos.DrawLine(ScreenToWorld(layoutBox.Rect.topRight), ScreenToWorld(layoutBox.Rect.bottomRight));
            Gizmos.DrawLine(ScreenToWorld(layoutBox.Rect.bottomRight), ScreenToWorld(layoutBox.Rect.bottomLeft));
            Gizmos.DrawLine(ScreenToWorld(layoutBox.Rect.bottomLeft), ScreenToWorld(layoutBox.Rect.topLeft));
        }

        static bool IsInsideRect(Rect rect, Vector2 p)
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

        public static void DrawLayoutOutlines(ILayoutBox layoutBox)
        {
            Color elemCol = Color.white;

            if (IsInsideRect(layoutBox.Rect, Input.mousePosition))
            {
                elemCol = Color.red;
            }

            DrawSingleLayout(layoutBox);

            int fontSize = 12;
            int halftTextWidth = layoutBox.Name.Length * fontSize / 2;
            int halftTextHeight = fontSize / 2;
            Vector2 offset = new Vector2(halftTextWidth, halftTextHeight);
            Vector3 worldSpace = ScreenToWorld(layoutBox.Rect.center - offset);
            GUIContent label = new GUIContent(layoutBox.Name);
            GUIStyleState txtStyle = new GUIStyleState() { textColor = elemCol };

            GUIStyle style = new GUIStyle()
            {
                normal = txtStyle,
                fontSize = 12
            };

            Handles.Label(worldSpace, label, style);

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                DrawLayoutOutlines(c);
            }
        }

        private static Vector3 ScreenToWorld(Vector3 uiSpace)
        {
            Vector3 screenSpace = uiSpace;
            screenSpace.z = Camera.main.nearClipPlane;
            screenSpace.y = Camera.main.pixelHeight - screenSpace.y;
            Vector3 worldSpace = Camera.main.ScreenToWorldPoint(screenSpace);
            return worldSpace;
        }
    }

}
