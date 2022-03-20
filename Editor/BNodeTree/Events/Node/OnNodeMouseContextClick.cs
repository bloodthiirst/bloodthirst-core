using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeMouseContextClick : BNodeTreeEventBase
    {
        public ContextClickEvent ClickEvent { get; }
        public NodeBaseElement Node { get; }

        public OnNodeMouseContextClick(INodeEditor nodeEditor, NodeBaseElement node, ContextClickEvent ClickEvent) : base(nodeEditor)
        {
            Node = node;
            this.ClickEvent = ClickEvent;
        }
    }
}
