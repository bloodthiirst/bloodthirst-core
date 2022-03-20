namespace Bloodthirst.Editor.BNodeTree
{
    public class OnPortRequestAddInput : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }

        public OnPortRequestAddInput(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
        }
    }
}
