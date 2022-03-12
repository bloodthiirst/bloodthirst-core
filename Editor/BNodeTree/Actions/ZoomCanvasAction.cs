using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class ZoomCanvasAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasScrollWheel>(HandleMouseScroll);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasScrollWheel>(HandleMouseScroll);
            NodeEditor.BEventSystem.Listen<OnCanvasScrollWheel>(HandleMouseScroll);
        }

        private void HandleMouseScroll(OnCanvasScrollWheel evt)
        {
            float zoomDelta = evt.WheelEvent.delta.y * NodeEditor.ZoomSensitivity;
            zoomDelta *= NodeEditor.InvertZoom ? -1 : 1;
            NodeEditor.Zoom += zoomDelta;
        }


    }
}
