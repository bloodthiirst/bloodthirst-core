namespace Bloodthirst.Editor.BNodeTree
{
    public abstract class BNodeTreeEventBase
    {
        public INodeEditor NodeEditor { get; private set; }

        public BNodeTreeEventBase(INodeEditor nodeEditor)
        {
            NodeEditor = nodeEditor;
        }
    }
}
