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

        #region IPortType<TNode> implementation
        Type IPortType<TNode>.PortType => PortType;
        string IPortType<TNode>.PortName { get => PortName; set => PortName = value; }

        NODE_DIRECTION IPortType<TNode>.NodeDirection { get => NodeDirection; set => NodeDirection = value; }
        TNode IPortType<TNode>.ParentNode { get => ParentNode; set => ParentNode = value; }

        ILinkType<TNode> IPortType<TNode>.LinkAttached 
        { 
            get => LinkAttached;
            set => LinkAttached = value;
        }

        #endregion

        #region IPortType implementation
        Type IPortType.PortType => PortType;

        string IPortType.PortName { get => PortName; set => PortName = value; }

        NODE_DIRECTION IPortType.NodeDirection { get => NodeDirection; set => NodeDirection = value; }
        INodeType IPortType.ParentNode { get => ParentNode; set => ParentNode = (TNode)value; }
        ILinkType IPortType.LinkAttached
        { 
            get => LinkAttached;
            set => LinkAttached = (ILinkType<TNode>)value;
        }
        #endregion

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