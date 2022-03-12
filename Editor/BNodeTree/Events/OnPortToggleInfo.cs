namespace Bloodthirst.Editor.BNodeTree
{
    public class OnPortToggleInfo : BNodeTreeEventBase
    {
        public PortBaseElement PortToggled { get; }

        public OnPortToggleInfo(INodeEditor nodeEditor, PortBaseElement PortBaseElement) : base(nodeEditor)
        {
            this.PortToggled = PortBaseElement;
        }
    }
}
