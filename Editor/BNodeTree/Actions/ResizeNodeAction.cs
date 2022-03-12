namespace Bloodthirst.Editor.BNodeTree
{
    public class ResizeNodeAction : NodeEditorActionBase
    {
        private NodeBaseElement currentNode;

        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeStartResize>(HandleStartNodeResize);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseUp>(HandleMouseUp);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseMove>(HandleMouseMove);
        }


        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeStartResize>(HandleStartNodeResize);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseUp>(HandleMouseUp);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseMove>(HandleMouseMove);

            NodeEditor.BEventSystem.Listen<OnNodeStartResize>(HandleStartNodeResize);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseUp>(HandleMouseUp);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseMove>(HandleMouseMove);
        }

        private void HandleMouseMove(OnCanvasMouseMove evt)
        {
            if (currentNode == null)
                return;

            currentNode.NodeSize += evt.MouseMoveEvent.mouseDelta / NodeEditor.Zoom;
        }

        private void HandleMouseUp(OnCanvasMouseUp evt)
        {
            currentNode = null;
        }

        private void HandleStartNodeResize(OnNodeStartResize evt)
        {
            currentNode = evt.Node;
        }

    }
}
