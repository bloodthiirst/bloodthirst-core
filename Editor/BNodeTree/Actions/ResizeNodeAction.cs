using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class ResizeNodeAction : NodeEditorActionBase
    {
        private NodeBaseElement currentNode;

        public override void OnDisable()
        {
            NodeEditor.OnNodeStartResize -= HandleStartNodeResize;
            NodeEditor.OnCanvasMouseUp -= HandleMouseUp;
            NodeEditor.OnCanvasMouseMove -= HandleMouseMove;
        }


        public override void OnEnable()
        {
            NodeEditor.OnNodeStartResize -= HandleStartNodeResize;
            NodeEditor.OnNodeStartResize += HandleStartNodeResize;

            NodeEditor.OnCanvasMouseUp -= HandleMouseUp;
            NodeEditor.OnCanvasMouseUp += HandleMouseUp;

            NodeEditor.OnCanvasMouseMove -= HandleMouseMove;
            NodeEditor.OnCanvasMouseMove += HandleMouseMove;
        }

        private void HandleMouseMove(MouseMoveEvent evt)
        {
            if (currentNode == null)
                return;

            currentNode.NodeSize += evt.mouseDelta / NodeEditor.Zoom;
        }

        private void HandleMouseUp(MouseUpEvent obj)
        {
            currentNode = null;
        }

        private void HandleStartNodeResize(NodeBaseElement node)
        {
            currentNode = node;
        }

    }
}
