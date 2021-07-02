using System;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class LinkNodesAction : NodeEditorActionBase
    {
        private PortBaseElement PendingLinkingPort { get; set; }

        public override void OnDisable()
        {
            NodeEditor.OnPortMouseContextClick -= HandleNodeRightClick;
        }

        public override void OnEnable()
        {
            NodeEditor.OnPortMouseContextClick -= HandleNodeRightClick;
            NodeEditor.OnPortMouseContextClick += HandleNodeRightClick;
        }

        private void HandleNodeRightClick(PortBaseElement rightClickedPort , ContextClickEvent evt)
        {
            if (PendingLinkingPort == null)
            {
                PendingLinkingPort = rightClickedPort;
                PendingLinkingPort.Select();
                return;
            }

            PendingLinkingPort.Deselect();

            if (PendingLinkingPort == rightClickedPort)
                return;
            


            if (!rightClickedPort.PortType.CanLinkTo(PendingLinkingPort.PortType))
                return;

            if (!PendingLinkingPort.PortType.CanLinkTo(rightClickedPort.PortType))
                return;

            if (rightClickedPort.PortType.NodeDirection != NODE_DIRECTION.INPUT)
                return;

            if (PendingLinkingPort.PortType.NodeDirection != NODE_DIRECTION.OUTPUT)
                return;

            NodeEditor.AddLink(PendingLinkingPort, rightClickedPort);

            PendingLinkingPort = null;
        }
    }
}
