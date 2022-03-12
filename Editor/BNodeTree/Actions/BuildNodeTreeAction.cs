namespace Bloodthirst.Editor.BNodeTree
{
    public class BuildNodeTreeAction : NodeEditorActionBase
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
            if (evt.KeyDownEvent.keyCode != UnityEngine.KeyCode.R)
                return;

           //NodeEditor.GetRootNodes();
        }

    }
}
