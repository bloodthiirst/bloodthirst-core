using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class SelectNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnNodeMouseClick -= HandleNodeMouseClick;
        }

        public override void OnEnable()
        {
            NodeEditor.OnNodeMouseClick -= HandleNodeMouseClick;
            NodeEditor.OnNodeMouseClick += HandleNodeMouseClick;
        }

        private void HandleNodeMouseClick(NodeBaseElement node , ClickEvent evt)
        {
            // middle mouse
            if (evt.button == (int)MouseButton.MiddleMouse)
                return;

            // deselect previous
            foreach (NodeBaseElement n in NodeEditor.SelectedNodes)
            {
                n.DeselectInCanvas();
            }

            NodeEditor.SelectedNodes.Clear();

            // select current
            NodeEditor.SelectedNodes.Add(node);
            node.SelectInCanvas();
        }
    }
}
