using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class SelectNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeMouseClick>(HandleNodeMouseClick);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeMouseClick>(HandleNodeMouseClick);
            NodeEditor.BEventSystem.Listen<OnNodeMouseClick>(HandleNodeMouseClick);
        }

        private void HandleNodeMouseClick(OnNodeMouseClick evt)
        {
            // middle mouse
            if (evt.ClickEvent.button == (int)MouseButton.MiddleMouse)
                return;

            // deselect previous
            foreach (NodeBaseElement n in NodeEditor.SelectedNodes)
            {
                n.DeselectInCanvas();
            }

            NodeEditor.SelectedNodes.Clear();

            // select current
            NodeEditor.SelectedNodes.Add(evt.Node);
            evt.Node.SelectInCanvas();
        }
    }
}
