using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class ActiveNodeBehaviourAction : NodeEditorActionBase
    {
        private int? LastSelectedNodeID;

        private BNodeTreeBehaviourBase SelectedTree;

        public override void OnDisable()
        {
            if (SelectedTree != null)
            {
                SelectedTree.OnActiveNodeChanged -= HanleActiveNodeChanged;
            }

            NodeEditor.OnBehaviourSelectionChanged -= HandleBehaviourChanged;
        }

        public override void OnEnable()
        {
            NodeEditor.OnBehaviourSelectionChanged -= HandleBehaviourChanged;
            NodeEditor.OnBehaviourSelectionChanged += HandleBehaviourChanged;
        }
        private void HanleActiveNodeChanged(INodeType currentNode)
        {
            RefreshSelected(currentNode);
        }

        private void HandleBehaviourChanged(BNodeTreeBehaviourBase behaviour)
        {
            if (SelectedTree != null)
            {
                SelectedTree.OnActiveNodeChanged -= HanleActiveNodeChanged;
            }

            if (behaviour == null)
            {
                return;
            }

            SelectedTree = behaviour;

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

            // select current node
            LastSelectedNodeID = currentNode.NodeID;

            NodeBaseElement node = NodeEditor.AllNodes.FirstOrDefault(n => n.NodeType.NodeID == LastSelectedNodeID);

            node.SelectActive();
        }


    }
}
