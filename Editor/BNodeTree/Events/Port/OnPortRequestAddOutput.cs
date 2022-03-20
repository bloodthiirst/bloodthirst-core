namespace Bloodthirst.Editor.BNodeTree
{
    public class OnPortRequestAddOutput : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }
        public OnPortRequestAddOutput(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
        }
    }
}
