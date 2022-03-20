namespace Bloodthirst.Editor.BNodeTree
{
    public class OnPortRemoveInput : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }
        public PortBaseElement Port { get; }

        public OnPortRemoveInput(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement , PortBaseElement Port) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
            this.Port = Port;
        }
    }
}
