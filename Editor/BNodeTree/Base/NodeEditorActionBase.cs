namespace Bloodthirst.Editor.BNodeTree
{
    public abstract class NodeEditorActionBase : INodeEditorAction
    {
        public INodeEditor NodeEditor { get; set; }
        public abstract void OnEnable();
        public abstract void OnDisable();
    }
}
