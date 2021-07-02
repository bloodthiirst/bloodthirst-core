using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class ZoomCanvasAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnCanvasScrollWheel -= HandleMouseScroll;
        }

        public override void OnEnable()
        {
            NodeEditor.OnCanvasScrollWheel -= HandleMouseScroll;
            NodeEditor.OnCanvasScrollWheel += HandleMouseScroll;
        }

        private void HandleMouseScroll(WheelEvent evt)
        {
            float zoomDelta = evt.delta.y * NodeEditor.ZoomSensitivity;
            zoomDelta *= NodeEditor.InvertZoom ? -1 : 1;
            NodeEditor.Zoom += zoomDelta;
        }


    }
}
