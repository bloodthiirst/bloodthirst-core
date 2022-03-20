using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnNodeResized : BNodeTreeEventBase
    {
        public NodeBaseElement Node { get; }

        public OnNodeResized(INodeEditor nodeEditor, NodeBaseElement node) : base(nodeEditor)
        {
            Node = node;
        }
    }
}
