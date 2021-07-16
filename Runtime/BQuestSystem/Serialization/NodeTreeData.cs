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

            JsonSerializerSettings settings = BQuestSystemSettings.GetSerializerSettings();
            CustomContext ctw = new CustomContext() { UnityObjects = unityObjects, Root = this };
            settings.Context = new StreamingContext(StreamingContextStates.Other, ctw);

            jsonData = JsonConvert.SerializeObject(this, settings);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            JsonSerializerSettings settings = BQuestSystemSettings.GetSerializerSettings();
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
        public IEnumerable<TNode> BuildAllNodes<TNode>() where TNode : INodeType<TNode> , INodeType
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
                INodeType fromNode = allNodes.FirstOrDefault(n => ((INodeType) n).NodeID == l.From);
                INodeType toNode = allNodes.FirstOrDefault(n => ((INodeType)n).NodeID == l.To);

                IPortType fromPort = fromNode.OutputPortsConst.ElementAt(l.FromPort);
                fromPort.ParentNode = fromNode;

                IPortType toPort = toNode.InputPortsConst.ElementAt(l.ToPort);
                toPort.ParentNode = toNode;

                LinkDefault link = new LinkDefault() { From = fromPort, To = toPort };

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

