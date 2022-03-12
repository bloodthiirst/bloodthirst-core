namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeStartResize : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }

        public OnNodeStartResize(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
        }
    }
}
