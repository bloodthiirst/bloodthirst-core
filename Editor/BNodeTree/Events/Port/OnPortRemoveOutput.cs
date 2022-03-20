namespace Bloodthirst.Editor.BNodeTree
{
    public class OnPortRemoveOutput : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }
        public PortBaseElement Port { get; }

        public OnPortRemoveOutput(INodeEditor nodeEditor, NodeBaseElement NodeBaseElement , PortBaseElement Port) : base(nodeEditor)
        {
            this.Node = NodeBaseElement;
            this.Port = Port;
        }
    }
}
