using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeMouseClick : BNodeTreeEventBase
    {
        public ClickEvent ClickEvent { get; }
        public NodeBaseElement Node { get; }

        public OnNodeMouseClick(INodeEditor nodeEditor, NodeBaseElement node, ClickEvent ClickEvent) : base(nodeEditor)
        {
            Node = node;
            this.ClickEvent = ClickEvent;
        }
    }
}
