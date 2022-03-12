namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeEndResize : BNodeTreeEventBase
    {
        public NodeBaseElement NodeBaseElement { get; }

        public OnNodeEndResize(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement) : base(nodeEditor)
        {
            this.NodeBaseElement = NodeBaseElement;
        }
    }
}
