using Bloodthirst.System.Quest.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.System.Quest
{
    public class BNodeTreeBehaviour : MonoBehaviour
    {
        [SerializeField]
        private NodeTreeData treeData;

        public NodeTreeData TreeData => treeData;

        private List<INodeType> currentInstance;
        private List<INodeType> rootNode;
        private INodeType currentActiveNode;

        public event Action<INodeType> OnCurrentActiveNodeChanged;
        public INodeType CurrentActiveNode 
        { 
            get => currentActiveNode;
            set
            {
                if (currentActiveNode == value)
                    return;

                currentActiveNode = value;
                OnCurrentActiveNodeChanged?.Invoke(currentActiveNode);
            }
        }

        private void Start()
        {
            ReloadTree();
        }

        private void ReloadTree()
        {
            // create a copy of the nodes structure
            currentInstance = new List<INodeType>(treeData.BuildAllNodes());

            rootNode = currentInstance.Where(n => n.InputPorts.All(p => p.LinkAttached == null)).ToList();
        }

        [Button]
        public void ResetCurrentNode()
        {
            CurrentActiveNode = rootNode[0];
        }

        [Button]
        public void AdvanceFromNode()
        {
            // if no current node is selected
            if (CurrentActiveNode == null)
            {
                ResetCurrentNode();
                return;
            }

            // try to nagivate from current node to next
            for (int i = 0; i < CurrentActiveNode.OutputPorts.Count; i++)
            {
                IPortType curr = CurrentActiveNode.OutputPorts[i];

                if (curr.LinkAttached == null)
                    continue;

                CurrentActiveNode = curr.LinkAttached.To.ParentNode;
                return;
            }

            // else go back to root node
            ResetCurrentNode();

        }
    }
}
