using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
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

                NodeEditor.RemoveNode(n);
            }
        }

    }
}
