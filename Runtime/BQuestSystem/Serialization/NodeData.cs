using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    public class NodeData
    {
        [SerializeField]
        [JsonIgnore]
        private Vector2 size;

        [JsonIgnore]
        [SerializeField]
        private Vector2 position;

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
        [SerializeField]
        [JsonIgnore]
        private int from;

        [SerializeField]
        [JsonIgnore]
        private int fromPort;

        [SerializeField]
        [JsonIgnore]
        private int to;

        [SerializeField]
        [JsonIgnore]
        private int toPort;

        [SerializeField]
        [JsonIgnore]
        private ILinkType linkType;

        private ILinkType LinkType 
        { 
            get => linkType;
            set => linkType = value;
        }

        public int From
        {
            get => from;
            set => from = value;
        }

        public int FromPort
        {
            get => fromPort;
            set => fromPort = value;

        }

        public int To
        {
            get => to;
            set => to = value;
        }

        public int ToPort
        {
            get => toPort;
            set => toPort = value;
        }
    }
}
