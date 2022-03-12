namespace Bloodthirst.Editor.BNodeTree
{
    public interface INodeEditorAction
    {
        INodeEditor NodeEditor { get; set; }
        void OnEnable();
        void OnDisable();
    }
}