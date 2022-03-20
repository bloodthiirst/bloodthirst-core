using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class LinkNodesAction : NodeEditorActionBase
    {
        private List<KeyCode> cancelKeyboardShortcut = new List<KeyCode>()
        {
            KeyCode.Escape,
            KeyCode.Delete
        };

        private PortBaseElement PendingLinkingPort { get; set; }

        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortMouseContextClick>(HandleNodeRightClick);
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseContextClick>(HandleCanvasRightClick);
            NodeEditor.BEventSystem.Unlisten<OnWindowKeyPressed>(HandleKeyboardPress);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortMouseContextClick>(HandleNodeRightClick);
            NodeEditor.BEventSystem.Listen<OnPortMouseContextClick>(HandleNodeRightClick);

            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseContextClick>(HandleCanvasRightClick);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseContextClick>(HandleCanvasRightClick);

            NodeEditor.BEventSystem.Unlisten<OnWindowKeyPressed>(HandleKeyboardPress);
            NodeEditor.BEventSystem.Listen<OnWindowKeyPressed>(HandleKeyboardPress);
        }

        private void HandleKeyboardPress(OnWindowKeyPressed evt)
        {
            if (!cancelKeyboardShortcut.Contains(evt.KeyDownEvent.keyCode))
                return;

            if (PendingLinkingPort == null)
                return;

            PendingLinkingPort.Deselect();
            PendingLinkingPort = null;
        }

        private void HandleCanvasRightClick(OnCanvasMouseContextClick evt)
        {
            if (PendingLinkingPort == null)
                return;

            PendingLinkingPort.Deselect();
            PendingLinkingPort = null;
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
