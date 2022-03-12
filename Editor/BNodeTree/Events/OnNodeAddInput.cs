namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeAddInput : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }

        public OnNodeAddInput(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
        }
    }
}
