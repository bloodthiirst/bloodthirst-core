using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class RemoveNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnWindowKeyPressed -= HandleKeyDown;
        }

        public override void OnEnable()
        {
            NodeEditor.OnWindowKeyPressed -= HandleKeyDown;
            NodeEditor.OnWindowKeyPressed += HandleKeyDown;
        }

        private void HandleKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != UnityEngine.KeyCode.Delete)
                return;

            List<NodeBaseElement> asList = NodeEditor.SelectedNodes.ToList();

            for (int i = asList.Count - 1; i >= 0; i--)
            {
                NodeBaseElement n = asList[i];

                NodeEditor.RemoveNode(n);
            }
        }

    }
}
