using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class RemovePortAction : NodeEditorActionBase
    {
        private PortBaseElement PendingLinkingPort;

        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortMouseContextClick>(HandlePortRightClick);
            NodeEditor.BEventSystem.Unlisten<OnWindowKeyPressed>(HandleKeyDown);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortMouseContextClick>(HandlePortRightClick);
            NodeEditor.BEventSystem.Unlisten<OnWindowKeyPressed>(HandleKeyDown);

            NodeEditor.BEventSystem.Listen<OnPortMouseContextClick>(HandlePortRightClick);
            NodeEditor.BEventSystem.Listen<OnWindowKeyPressed>(HandleKeyDown);
        }

        private void HandlePortRightClick(OnPortMouseContextClick evt)
        {
            PortBaseElement rightClickedPort = evt.PortRightClicked;

            if (PendingLinkingPort == null)
            {
                PendingLinkingPort = evt.PortRightClicked;
                PendingLinkingPort.Select();
                return;
            }

            PendingLinkingPort.Deselect();
            PendingLinkingPort = null;

        }

        private void HandleKeyDown(OnWindowKeyPressed pressed)
        {
            if (pressed.KeyDownEvent.keyCode != UnityEngine.KeyCode.P)
            {
                return;
            }

            if (PendingLinkingPort == null)
            {
                return;
            }

            foreach (ILinkType link in PendingLinkingPort.PortType.LinkAttached)
            {
                if (link != null)
                {
                    NodeEditor.RemoveLink(link);
                }
            }

            PendingLinkingPort.ParentNode.RemovePort(PendingLinkingPort.PortType);
            PendingLinkingPort = null;
        }
    }


    public class RemoveNodeAction : NodeEditorActionBase
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

        private void HandleKeyDown(OnWindowKeyPressed evt)
        {
            KeyDownEvent keydownEvent = evt.KeyDownEvent;

            if (keydownEvent.keyCode != UnityEngine.KeyCode.Delete)
                return;

            List<NodeBaseElement> asList = NodeEditor.SelectedNodes.ToList();

            for (int i = asList.Count - 1; i >= 0; i--)
            {
                NodeBaseElement n = asList[i];

                NodeEditor.RemoveNode(n.NodeType);
            }
        }

    }
}
