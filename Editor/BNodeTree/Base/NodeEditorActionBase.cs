namespace Bloodthirst.System.Quest.Editor
{
    public abstract class NodeEditorActionBase : INodeEditorAction
    {
        public INodeEditor NodeEditor { get; set; }
        public abstract void OnEnable();
        public abstract void OnDisable();
    }
}
