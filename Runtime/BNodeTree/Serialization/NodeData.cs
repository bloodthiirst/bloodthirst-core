using Bloodthirst.Runtime.BNodeTree;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Runtime.BNodeTree
{
    public class NodeData
    {
        [Title("Canvas UI info")]
        [SerializeField]
        [JsonIgnore]
        private Vector2 size;

        [JsonIgnore]
        [SerializeField]
        private Vector2 position;

        [Title("Node Data")]
        [ShowInInspector]
        [JsonIgnore]
        private INodeType nodeType;

        public Vector2 Size
        {
            get => size;
            set => size = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public INodeType NodeType
        {
            get => nodeType;
            set => nodeType = value;
        }
    }

    public class LinkData
    {
#if UNITY_EDITOR && ODIN_INSPECTOR

        const string H_GROUP = "H_GROUP";
        const string V_FROM_GROUP = "H_GROUP/V_FROM_GROUP";
        const string V_TO_GROUP = "H_GROUP/V_TO_GROUP";
#endif

        #region from
        [Title("From")]
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private int fromNodeIndex;

        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private int fromPortIndex;

        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private PORT_DIRECTION fromPortDirection;

        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private PORT_TYPE fromPortType;

        #endregion

        #region to
        [Title("To")]
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private int toNodeIndex;

        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private int toPortIndex;

        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private PORT_DIRECTION toPortDirection;

        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
        [SerializeField]
        [JsonIgnore]
        private PORT_TYPE toPortType;

        #endregion

        [Title("Link Data")]
        [SerializeField]
        [JsonIgnore]
        private ILinkType linkType;

        #region accessors

        private ILinkType LinkType 
        { 
            get => linkType;
            set => linkType = value;
        }

        public int FromNodeIndex
        {
            get => fromNodeIndex;
            set => fromNodeIndex = value;
        }

        public int FromPortIndex
        {
            get => fromPortIndex;
            set => fromPortIndex = value;
        }

        public int ToNodeIndex
        {
            get => toNodeIndex;
            set => toNodeIndex = value;
        }

        public int ToPortIndex
        {
            get => toPortIndex;
            set => toPortIndex = value;
        }
        public PORT_DIRECTION FromPortDirection
        { 
            get => fromPortDirection;
            set => fromPortDirection = value;
        }
        
        public PORT_TYPE FromPortType
        { 
            get => fromPortType;
            set => fromPortType = value;
        }
        
        public PORT_DIRECTION ToPortDirection
        {
            get => toPortDirection;
            set => toPortDirection = value;
        }

        public PORT_TYPE ToPortType
        { 
            get => toPortType;
            set => toPortType = value;
        }

        #endregion
    }
}
