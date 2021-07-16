using System;

namespace Bloodthirst.System.Quest.Editor
{
    public class PortDefault<TNode> : IPortType<TNode>, IPortType where TNode : INodeType<TNode>, INodeType
    {
        public Type PortType { get; protected set; }

        public string PortName { get; set; }

        public TNode ParentNode { get; set; }

        public NODE_DIRECTION NodeDirection { get; set; }

        public ILinkType<TNode> LinkAttached { get; set; }

        Type IPortType.PortType => PortType;

        string IPortType.PortName => PortName;

        NODE_DIRECTION IPortType.NodeDirection { get => NodeDirection; set => NodeDirection = value; }
        INodeType IPortType.ParentNode { get => ParentNode; set => ParentNode = (TNode)value; }
        ILinkType IPortType.LinkAttached { get => LinkAttached; set => LinkAttached = (ILinkType<TNode>)value; }

        public PortDefault()
        {
            PortType = typeof(string);
        }

        public virtual object GetPortValue()
        {
            return null;
        }

        public virtual bool CanLinkTo(IPortType<TNode> otherPort)
        {
            // can't connect same direction nodes
            if (NodeDirection == otherPort.NodeDirection)
                return false;

            return true;
        }

        object IPortType.GetPortValue()
        {
            return GetPortValue();
        }

        bool IPortType.CanLinkTo(IPortType otherPort)
        {
            return CanLinkTo((IPortType<TNode>)otherPort);
        }
    }
}