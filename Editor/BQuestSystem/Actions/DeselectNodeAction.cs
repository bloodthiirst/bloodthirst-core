using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class DeselectNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnCanvasMouseClick -= HandleMouseClick;
        }

        public override void OnEnable()
        {
            NodeEditor.OnCanvasMouseClick -= HandleMouseClick;
            NodeEditor.OnCanvasMouseClick += HandleMouseClick;
        }

        private void HandleMouseClick(ClickEvent evt)
        {
            TryDeselect(evt);
        }

        private void TryDeselect(ClickEvent evt)
        {
            foreach (NodeBaseElement n in NodeEditor.SelectedNodes)
            {
                n.DeselectInCanvas();
            }

            NodeEditor.SelectedNodes.Clear();
        }
    }
}
