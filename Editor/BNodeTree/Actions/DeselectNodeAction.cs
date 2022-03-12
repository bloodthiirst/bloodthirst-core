using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class DeselectNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseClick>(HandleMouseClick);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseClick>(HandleMouseClick);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseClick>(HandleMouseClick);
        }

        private void HandleMouseClick(OnCanvasMouseClick evt)
        {
            TryDeselect(evt.ClickEvent);
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
