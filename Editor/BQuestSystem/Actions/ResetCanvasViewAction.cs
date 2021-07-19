using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class ResetCanvasViewAction : NodeEditorActionBase , INodeEditorCommand
    {
        public override void OnDisable()
        {
            NodeEditor.OnWindowKeyPressed -= HandleKeyDown;
        }

        public override void OnEnable()
        {
            NodeEditor.OnWindowKeyPressed -= HandleKeyDown;
            NodeEditor.OnWindowKeyPressed += HandleKeyDown;
        }

        public void Execute()
        {
            List<NodeBaseElement> asList = NodeEditor.AllNodes;

            float left = asList[0].VisualElement.localBound.xMin;
            float right = asList[0].VisualElement.localBound.xMax;
            float top = asList[0].VisualElement.localBound.yMin;
            float bottom = asList[0].VisualElement.localBound.yMax;

            foreach (NodeBaseElement n in asList)
            {
                Rect rect = n.VisualElement.localBound;

                left = Mathf.Min(left, rect.xMin);
                right = Mathf.Max(right, rect.xMax);
                top = Mathf.Min(top, rect.yMin);
                bottom = Mathf.Max(bottom, rect.yMax);
            }

            Vector2 p = Vector2.zero;
            p.x = Mathf.Lerp(left, right, 0.5f);
            p.y = Mathf.Lerp(top, bottom, 0.5f);

            // fix panning
            NodeEditor.PanningOffset = -p;

            float totalWidth = right - left;
            float totalHeight = bottom - top;

            Vector2 canvasSize = NodeEditor.CanvasSize;

            float wRatio = canvasSize.x / totalWidth;
            float yRatio = canvasSize.y / totalHeight;

            // set zoom with some padding
            NodeEditor.Zoom = Mathf.Min(wRatio, yRatio) * 0.8f;
        }

        private void HandleKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.R)
                return;

            ((INodeEditorCommand) this ).Execute();
        }

    }
}
