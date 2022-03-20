using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeMoved : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }

        public OnNodeMoved(INodeEditor nodeEditor, NodeBaseElement node) : base(nodeEditor)
        {
            Node = node;
        }
    }
}
