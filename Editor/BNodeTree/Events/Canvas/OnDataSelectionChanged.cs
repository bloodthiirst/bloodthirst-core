using Bloodthirst.Runtime.BNodeTree;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnDataSelectionChanged : BNodeTreeEventBase
    {
        public NodeTreeData NodeTreeData { get; }

        public OnDataSelectionChanged(INodeEditor nodeEditor, NodeTreeData NodeTreeData) : base(nodeEditor)
        {
            this.NodeTreeData = NodeTreeData;
        }
    }
}
