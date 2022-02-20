using Bloodthirst.BDeepCopy;
using Bloodthirst.BJson;
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
        [BJsonIgnore]
        private List<NodeData> nodes;

        [ShowInInspector]
        [BJsonIgnore]
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
                // get the 2 nodes linked
                TNode fromNode = allNodes.FirstOrDefault(n => ((INodeType<TNode>)n).NodeID == l.FromNodeIndex);
                TNode toNode = allNodes.FirstOrDefault(n => ((INodeType<TNode>)n).NodeID == l.ToNodeIndex);

                // get the ports of the nodes linked
                IPortType<TNode> fromPort = (IPortType<TNode>)((INodeType) fromNode).Ports[l.FromPortIndex];
                IPortType<TNode> toPort = (IPortType<TNode>)((INodeType)toNode).Ports[l.ToPortIndex];

                // assign nodes as parents of the ports
                fromPort.ParentNode = fromNode;
                toPort.ParentNode = toNode;

                // create the link
                LinkDefault<TNode> link = new LinkDefault<TNode>()
                { 
                    From = fromPort,
                    To = toPort 
                };

                // attach the link
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

