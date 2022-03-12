using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class ResetCanvasViewAction : NodeEditorActionBase , INodeEditorCommand
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnWindowKeyPressed>(HandleKeyDown);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnWindowKeyPressed>(HandleKeyDown);
            NodeEditor.BEventSystem.Listen<OnWindowKeyPressed>(HandleKeyDown);
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
            float appropriateZoom = Mathf.Min(wRatio, yRatio) * 0.8f;
            NodeEditor.Zoom = Mathf.Min(1, appropriateZoom);
        }

        private void HandleKeyDown(OnWindowKeyPressed evt)
        {
            if (evt.KeyDownEvent.keyCode != KeyCode.R)
                return;

            ((INodeEditorCommand) this ).Execute();
        }

    }
}
