using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Runtime.BNodeTree
{
    /// <summary>
    /// Base class for all the typed treenode behaviours
    /// </summary>
    public abstract class BNodeTreeBehaviourBase : MonoBehaviour , IBNodeTreeBehaviour
    {
        /// <summary>
        /// the data asset of the treenode
        /// </summary>
        public abstract NodeTreeData TreeData { get; }

        /// <summary>
        /// The root nodes of the tree node
        /// </summary>
        public List<INodeType> RootNodes { get; protected set; }

        protected INodeType activeNode;

        /// <summary>
        /// The currently active node for this instance
        /// </summary>
        public INodeType ActiveNode
        {
            get => activeNode;
            protected set
            {
                if (activeNode == value)
                    return;

                activeNode = value;
                OnActiveNodeChanged?.Invoke(activeNode);
            }
        }

        /// <summary>
        /// Triggered when the current node changes
        /// </summary>
        public event Action<INodeType> OnActiveNodeChanged;
    }

    /// <summary>
    /// Wrapper class that define a treenode behaviour with a specific node base class
    /// </summary>
    /// <typeparam name="TNodeType"></typeparam>
    public abstract class BNodeTreeBehaviourBase<TNode> : BNodeTreeBehaviourBase where TNode : INodeType<TNode> , INodeType
    {
#if UNITY_EDITOR
        private const string TYPE_WARNING_MESSAGE = "The type of the node data needs to match the type of the tree behaviour";
#endif
        private static Type type => typeof(TNode);

        /// <summary>
        /// Get the base type of the nodes of the current tree
        /// </summary>
        public static Type Type => type;

        public List<TNode> Roots { get; private set; }
        
        [SerializeField]
#if UNITY_EDITOR
        [InfoBox(TYPE_WARNING_MESSAGE  , nameof(ShowTypeWarning), InfoMessageType = InfoMessageType.Error)]
#endif
        private NodeTreeData treeData;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override NodeTreeData TreeData => treeData;

        private TNode activeNodeTyped;

        /// <summary>
        /// The currently active typed node  for this instance
        /// </summary>
        public TNode ActiveNodeTyped
        {
            get => activeNodeTyped;
            set
            {      
                if (EqualityComparer<TNode>.Default.Equals(activeNodeTyped, value))
                    return;

                ActiveNode = (INodeType) value;
                activeNodeTyped = value;
                OnActiveNodeTypedChanged?.Invoke(activeNodeTyped);
            }
        }

        /// <summary>
        /// Triggered when the current node changes
        /// </summary>
        public event Action<TNode> OnActiveNodeTypedChanged;

        private void Start()
        {
            ReloadTree();
        }

#if UNITY_EDITOR
        public bool ShowTypeWarning()
        {

            if (TreeData == null)
                return false;

            if (TreeData.NodeBaseType == Type)
                return false;

            return true;

        }
#endif

        /// <summary>
        /// Get the root nodes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TNode> RootNodesTyped()
        {
            foreach(TNode n in Roots)
            {
                yield return n;
            }
        }

        /// <summary>
        /// Reload the tree structure from the data asset
        /// </summary>
        public void ReloadTree()
        {
            // create a copy of the nodes structure
            // the root is the node that nothing attached to it as input
            Roots = treeData
                .BuildAllNodes<TNode>()
                .Where(n => ((INodeType)n).GetPorts(PORT_DIRECTION.INPUT).All(p => p.LinkAttached == null))
                .ToList();
        }

        [Button]
        public void ResetCurrentNode()
        {
            ActiveNodeTyped = Roots[0];
        }

        [Button]
        public void AdvanceFromNode()
        {
            // if no current node is selected
            if (ActiveNodeTyped == null)
            {
                ResetCurrentNode();
                return;
            }

            // try to nagivate from current node to next
            IEnumerable<IPortType<TNode>> outputPortsConst = ActiveNodeTyped.GetPorts(PORT_DIRECTION.OUTPUT).Cast<IPortType<TNode>>();

            foreach (IPortType<TNode> curr in outputPortsConst)
            {
                if (curr.LinkAttached == null)
                    continue;

                ActiveNodeTyped = curr.LinkAttached.ToTyped.ParentNode;
                return;
            }

            // else go back to root node
            ResetCurrentNode();

        }
    }
}
