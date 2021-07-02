using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class PanCanvasAction : NodeEditorActionBase
    {
        private bool IsDragging;

        public override void OnDisable()
        {
            NodeEditor.OnCanvasMouseMove -= HandleMouseMove;
            NodeEditor.OnCanvasMouseUp -= HandleMouseUp;
            NodeEditor.OnCanvasMouseDown -= HandleMouseDown;
            NodeEditor.OnCanvasMouseLeave -= HandleMouseLeave;
        }

        public override void OnEnable()
        {
            NodeEditor.OnCanvasMouseMove -= HandleMouseMove;
            NodeEditor.OnCanvasMouseMove += HandleMouseMove;

            NodeEditor.OnCanvasMouseUp -= HandleMouseUp;
            NodeEditor.OnCanvasMouseUp += HandleMouseUp;

            NodeEditor.OnCanvasMouseDown -= HandleMouseDown;
            NodeEditor.OnCanvasMouseDown += HandleMouseDown;

            NodeEditor.OnCanvasMouseLeave -= HandleMouseLeave;
            NodeEditor.OnCanvasMouseLeave += HandleMouseLeave;
        }

        private void HandleMouseDown(MouseDownEvent evt)
        {
            if (evt.button != (int)MouseButton.MiddleMouse)
                return;

            IsDragging = true;

            // stops the click from affecting child elements
            evt.StopPropagation();
        }

        private void HandleMouseLeave(MouseLeaveEvent evt)
        {
            IsDragging = false;

            // stops the click from affecting child elements
            evt.StopPropagation();
        }


        private void HandleMouseUp(MouseUpEvent evt)
        {
            if (evt.button != (int)MouseButton.MiddleMouse)
                return;

            IsDragging = false;

            // stops the click from affecting child elements
            evt.StopPropagation();
        }

        private void HandleMouseMove(MouseMoveEvent evt)
        {
            if (!IsDragging)
                return;

            NodeEditor.PanningOffset += evt.mouseDelta;

            // stops the click from affecting child elements
            evt.StopPropagation();
        }
    }
}
