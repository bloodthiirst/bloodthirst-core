using System;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class LinkNodesAction : NodeEditorActionBase
    {
        private PortBaseElement PendingLinkingPort { get; set; }

        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortMouseContextClick>(HandleNodeRightClick);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortMouseContextClick>(HandleNodeRightClick);
            NodeEditor.BEventSystem.Listen<OnPortMouseContextClick>(HandleNodeRightClick);
        }

        private void HandleNodeRightClick(OnPortMouseContextClick evt)
        {
            PortBaseElement rightClickedPort = evt.PortRightClicked;

            if (PendingLinkingPort == null)
            {
                PendingLinkingPort = evt.PortRightClicked;
                PendingLinkingPort.Select();
                return;
            }

            PendingLinkingPort.Deselect();

            if (PendingLinkingPort == rightClickedPort)
            {
                PendingLinkingPort = null;
                return;
            }

            if (rightClickedPort.ParentNode == PendingLinkingPort.ParentNode)
                return;

            if (!rightClickedPort.PortType.CanLinkTo(PendingLinkingPort.PortType))
                return;

            if (!PendingLinkingPort.PortType.CanLinkTo(rightClickedPort.PortType))
                return;

            if (rightClickedPort.PortType.PortDirection == PendingLinkingPort.PortType.PortDirection)
                return;

            NodeEditor.AddLink(PendingLinkingPort, rightClickedPort);

            PendingLinkingPort = null;
        }
    }
}
