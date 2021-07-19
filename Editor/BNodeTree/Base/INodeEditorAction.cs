namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeEditorAction
    {
        INodeEditor NodeEditor { get; set; }
        void OnEnable();
        void OnDisable();
    }
}