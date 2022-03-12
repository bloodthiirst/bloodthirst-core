using Bloodthirst.Runtime.BNodeTree;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnBehaviourSelectionChanged : BNodeTreeEventBase
    {
        public IBNodeTreeBehaviour Behaviour { get; }

        public OnBehaviourSelectionChanged(INodeEditor nodeEditor, IBNodeTreeBehaviour IBNodeTreeBehaviour) : base(nodeEditor)
        {
            this.Behaviour = IBNodeTreeBehaviour;
        }
    }
}
