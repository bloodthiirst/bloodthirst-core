using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnPortMouseContextClick : BNodeTreeEventBase
    {
        public PortBaseElement PortRightClicked { get; }
        public ContextClickEvent ContextClickEvent { get; }

        public OnPortMouseContextClick(INodeEditor nodeEditor, PortBaseElement PortBaseElement, ContextClickEvent ContextClickEvent) : base(nodeEditor)
        {
            this.PortRightClicked = PortBaseElement;
            this.ContextClickEvent = ContextClickEvent;
        }
    }
}
