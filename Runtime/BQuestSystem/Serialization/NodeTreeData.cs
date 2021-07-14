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
            CustomContext ctw = new CustomContext() { UnityObjects = unityObjects , Root = this };
            settings.Context = new StreamingContext(StreamingContextStates.Other, ctw);

            jsonData = JsonConvert.SerializeObject(this, settings);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            JsonSerializerSettings settings = BQuestSystemSettings.GetSerializerSettings();
            CustomContext ctx = new CustomContext() { UnityObjects = unityObjects , Root = this };
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
        public IEnumerable<INodeType> BuildAllNodes()
        {
            // get the nodes
            List<INodeType> allNodes = new List<INodeType>();

            // copy the nodes
            foreach (NodeData n in Nodes)
            {

                    INodeType nodeType = BCopier<INodeType>.Instance.Copy(n.NodeType);
                    allNodes.Add(nodeType);

                    
            }

            // link the port references
            foreach (LinkData l in Links)
            {
                INodeType fromNode = allNodes.FirstOrDefault(n => n.NodeID == l.From);
                INodeType toNode = allNodes.FirstOrDefault(n => n.NodeID == l.To);

                IPortType fromPort = fromNode.OutputPortsConst[l.FromPort];
                fromPort.ParentNode = fromNode;

                IPortType toPort = toNode.InputPortsConst[l.ToPort];
                toPort.ParentNode = toNode;

                LinkDefault link = new LinkDefault() { From = fromPort, To = toPort };

                fromPort.LinkAttached = link;
                toPort.LinkAttached = link;
            }

            foreach (INodeType n in allNodes)
            {
                yield return n;
            }
        }
    }
}

