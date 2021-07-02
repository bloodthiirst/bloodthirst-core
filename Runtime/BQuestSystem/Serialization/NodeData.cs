using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    public class NodeData
    {
        [SerializeField]
        private Vector2 size;

        [SerializeField]
        private Vector2 position;

        [ShowInInspector]
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
        private int from;

        [SerializeField]
        private int fromPort;

        [SerializeField]
        private int to;

        [SerializeField]
        private int toPort;

        [SerializeField]
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
