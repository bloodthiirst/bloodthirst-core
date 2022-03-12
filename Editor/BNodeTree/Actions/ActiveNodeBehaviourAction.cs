using Bloodthirst.Runtime.BNodeTree;
using System.Linq;

namespace Bloodthirst.Editor.BNodeTree
{
    public class ActiveNodeBehaviourAction : NodeEditorActionBase
    {
        private int? LastSelectedNodeID;

        private IBNodeTreeBehaviour SelectedTree;

        public override void OnDisable()
        {
            if (SelectedTree != null)
            {
                SelectedTree.OnActiveNodeChanged -= HanleActiveNodeChanged;
            }

            NodeEditor.BEventSystem.Unlisten<OnBehaviourSelectionChanged>(HandleBehaviourChanged);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnBehaviourSelectionChanged>(HandleBehaviourChanged);
            NodeEditor.BEventSystem.Listen<OnBehaviourSelectionChanged>(HandleBehaviourChanged);
        }
        private void HanleActiveNodeChanged(INodeType currentNode)
        {
            RefreshSelected(currentNode);
        }

        private void HandleBehaviourChanged(OnBehaviourSelectionChanged onBehaviourSelectionChanged)
        {
            if (SelectedTree != null)
            {
                SelectedTree.OnActiveNodeChanged -= HanleActiveNodeChanged;
            }

            IBNodeTreeBehaviour behaviour = onBehaviourSelectionChanged.Behaviour;

            if (behaviour == null)
            {
                return;
            }

            SelectedTree = onBehaviourSelectionChanged.Behaviour;

            // when active node is changed
            SelectedTree.OnActiveNodeChanged -= HanleActiveNodeChanged;
            SelectedTree.OnActiveNodeChanged += HanleActiveNodeChanged;

            DelesectAll();

            if (behaviour.ActiveNode != null)
            {
                RefreshSelected(behaviour.ActiveNode);
            }
        }


        private void DelesectAll()
        {
            // clean up
            foreach (NodeBaseElement n in NodeEditor.AllNodes)
            {
                n.DeselectActive();
            }

            LastSelectedNodeID = null;
        }

        private void RefreshSelected(INodeType currentNode)
        {
            // if node has already been selected before
            // we deselect it
            if (LastSelectedNodeID.HasValue)
            {
                NodeBaseElement oldNode = NodeEditor.AllNodes.FirstOrDefault(n => n.NodeType.NodeID == LastSelectedNodeID);

                if (oldNode != null)
                {
                    oldNode.DeselectActive();
                }
            }

            if(currentNode == null)
            {
                LastSelectedNodeID = null;
                return;
            }

            // select current node
            LastSelectedNodeID = currentNode.NodeID;

            NodeBaseElement node = NodeEditor.AllNodes.FirstOrDefault(n => n.NodeType.NodeID == LastSelectedNodeID);

            node.SelectActive();
        }


    }
}
