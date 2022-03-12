namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeAddOutput : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }

        public OnNodeAddOutput(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
        }
    }
}
