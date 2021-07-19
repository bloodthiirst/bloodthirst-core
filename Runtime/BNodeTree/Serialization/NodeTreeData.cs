using Bloodthirst.BDeepCopy;
using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    [Serializable]
    public class NodeTreeData : ScriptableObject, ISerializationCallbackReceiver
    {

#if UNITY_EDITOR
        [ShowIf(nameof(showJSON))]
#endif
        [SerializeField]
        [JsonIgnore]
        [TextArea(5, 20)]
        private string jsonData;

#if UNITY_EDITOR
        [ShowInInspector]
        [JsonIgnore]
        private bool showJSON;
#endif
        [ShowInInspector]
        public Type NodeBaseType { get; set; }

        [SerializeField]
        [JsonIgnore]
        private List<UnityEngine.Object> unityObjects;

        [ShowInInspector]
        [JsonIgnore]
        private List<NodeData> nodes;

        [ShowInInspector]
        [JsonIgnore]
        private List<LinkData> links;

        public List<NodeData> Nodes { get => nodes; set => nodes = value; }
        public List<LinkData> Links { get => links; set => links = value; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            unityObjects = unityObjects.CreateOrClear();

            JsonSerializerSettings settings = BNodeTreeSettings.GetSerializerSettings();
            CustomContext ctw = new CustomContext() { UnityObjects = unityObjects, Root = this };
            settings.Context = new StreamingContext(StreamingContextStates.Other, ctw);

            jsonData = JsonConvert.SerializeObject(this, settings);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            JsonSerializerSettings settings = BNodeTreeSettings.GetSerializerSettings();
            CustomContext ctx = new CustomContext() { UnityObjects = unityObjects, Root = this };
            settings.Context = new StreamingContext(StreamingContextStates.Other, ctx);

            JsonConvert.PopulateObject(jsonData, this, settings);
        }

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

                /*
                List<IPortType<TNode>> outputTyped = (List<IPortType<TNode>>)nodeType.OutputPortsConstTyped;
                List<IPortType> output = (List<IPortType>)nodeType.OutputPortsConst;

                for(int i = 0; i< outputTyped.Count;i++)
                {
                    output[i] = (IPortType) outputTyped[i];
                }

                List<IPortType<TNode>> inputTyped = (List<IPortType<TNode>>)nodeType.InputPortsConstTyped;
                List<IPortType> input = (List<IPortType>)nodeType.InputPortsConst;

                for (int i = 0; i < inputTyped.Count; i++)
                {
                    input[i] = (IPortType) inputTyped[i];
                }
                */
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

