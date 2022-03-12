using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class PanCanvasAction : NodeEditorActionBase
    {
        private bool IsDragging;

        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseMove>(HandleMouseMove);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseUp>(HandleMouseUp);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseDown>(HandleMouseDown);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseLeave>(HandleMouseLeave);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseMove>(HandleMouseMove);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseUp>(HandleMouseUp);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseDown>(HandleMouseDown);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseLeave>(HandleMouseLeave);

            NodeEditor.BEventSystem.Listen<OnCanvasMouseMove>(HandleMouseMove);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseUp>(HandleMouseUp);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseDown>(HandleMouseDown);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseLeave>(HandleMouseLeave);
        }

        private void HandleMouseDown(OnCanvasMouseDown evt)
        {
            MouseDownEvent mouseDown = evt.MouseDownEvent;

            if (mouseDown.button != (int)MouseButton.MiddleMouse)
                return;

            IsDragging = true;

            // stops the click from affecting child elements
            mouseDown.StopPropagation();
        }

        private void HandleMouseLeave(OnCanvasMouseLeave evt)
        {
            MouseLeaveEvent mouseLeave = evt.MouseLeaveEvent;
            IsDragging = false;

            // stops the click from affecting child elements
            mouseLeave.StopPropagation();
        }


        private void HandleMouseUp(OnCanvasMouseUp evt)
        {
            MouseUpEvent mouseUp = evt.MouseUpEvent;
            if (mouseUp.button != (int)MouseButton.MiddleMouse)
                return;

            IsDragging = false;

            // stops the click from affecting child elements
            mouseUp.StopPropagation();
        }

        private void HandleMouseMove(OnCanvasMouseMove evt)
        {
            if (!IsDragging)
                return;

            NodeEditor.PanningOffset += evt.MouseMoveEvent.mouseDelta / NodeEditor.Zoom;

            // stops the click from affecting child elements
            evt.MouseMoveEvent.StopPropagation();
        }
    }
}
