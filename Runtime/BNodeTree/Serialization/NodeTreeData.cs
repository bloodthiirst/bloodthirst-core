using Bloodthirst.BDeepCopy;
using Bloodthirst.Core.Utils;
using Bloodthirst.JsonUnityObject;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    public class NodeTreeData : JsonScriptableObject
    {
        [ShowInInspector]
        public Type NodeBaseType { get; set; }

        [ShowInInspector]
        [JsonIgnore]
        private List<NodeData> nodes;

        [ShowInInspector]
        [JsonIgnore]
        private List<LinkData> links;

        public List<NodeData> Nodes { get => nodes; set => nodes = value; }
        public List<LinkData> Links { get => links; set => links = value; }


        public NodeTreeData()
        {
            nodes = new List<NodeData>();
            links = new List<LinkData>();
        }

        /// <summary>
        /// Create a copy of the node tree structure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TNode> BuildAllNodes<TNode>() where TNode : INodeType<TNode>, INodeType
        {
            // get the nodes
            List<TNode> allNodes = new List<TNode>();

            // copy the nodes
            foreach (NodeData n in Nodes)
            {
                TNode nodeType = BCopier<TNode>.Instance.Copy((TNode)n.NodeType);

                allNodes.Add(nodeType);


            }

            // link the port references
            foreach (LinkData l in Links)
            {
                TNode fromNode = allNodes.FirstOrDefault(n => ((INodeType<TNode>)n).NodeID == l.From);
                TNode toNode = allNodes.FirstOrDefault(n => ((INodeType<TNode>)n).NodeID == l.To);

                IPortType<TNode> fromPort = ((INodeType<TNode>)fromNode).OutputPortsConstTyped.ElementAt(l.FromPort);
                fromPort.ParentNode = fromNode;

                IPortType<TNode> toPort = ((INodeType<TNode>)toNode).InputPortsConstTyped.ElementAt(l.ToPort);
                toPort.ParentNode = toNode;

                LinkDefault<TNode> link = new LinkDefault<TNode>() { From = fromPort, To = toPort };

                fromPort.LinkAttached = link;
                toPort.LinkAttached = link;
            }

            foreach (TNode n in allNodes)
            {
                yield return n;
            }
        }
    }
}

