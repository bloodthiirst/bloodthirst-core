using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class UnlinkNodeAction : NodeEditorActionBase
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
            if (pressed.KeyDownEvent.keyCode != UnityEngine.KeyCode.U)
            {
                return;
            }

            if (PendingLinkingPort == null)
            {
                return;
            }

            for (int i = PendingLinkingPort.PortType.LinkAttached.Count - 1; i >= 0; i--)
            {
                ILinkType link = PendingLinkingPort.PortType.LinkAttached[i];
                if (link != null)
                {
                    NodeEditor.RemoveLink(link);
                }
            }

            PendingLinkingPort = null;
        }
    }
}
