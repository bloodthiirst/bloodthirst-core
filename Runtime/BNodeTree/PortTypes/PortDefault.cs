using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    public class PortDefault<TNode> : IPortType<TNode>, IPortType where TNode : INodeType<TNode>, INodeType
    {
        #region PortDefault<TNode> implementation
        public Type PortValueType { get; protected set; }
        public string PortName { get; set; }
        internal TNode ParentNode { get; set; }
        public PORT_DIRECTION PortDirection { get; set; }
        internal PORT_TYPE PortType { get; set; }
        public ILinkType<TNode> LinkAttached { get; internal set; }
        #endregion

        #region IPortType<TNode> implementation
        Type IPortType<TNode>.PortValueType => PortValueType;
        string IPortType<TNode>.PortName { get => PortName; set => PortName = value; }
        PORT_TYPE IPortType<TNode>.PortType { get => PortType; set => PortType = value; }
        PORT_DIRECTION IPortType<TNode>.PortDirection { get => PortDirection; set => PortDirection = value; }
        TNode IPortType<TNode>.ParentNode { get => ParentNode; set => ParentNode = value; }

        ILinkType<TNode> IPortType<TNode>.LinkAttached 
        { 
            get => LinkAttached;
            set => LinkAttached = value;
        }

        #endregion

        #region IPortType implementation
        Type IPortType.PortValueType => PortValueType;

        string IPortType.PortName { get => PortName; set => PortName = value; }
        PORT_TYPE IPortType.PortType { get => PortType; set => PortType = value; }
        PORT_DIRECTION IPortType.PortDirection { get => PortDirection; set => PortDirection = value; }
        INodeType IPortType.ParentNode { get => ParentNode; set => ParentNode = (TNode)value; }
        ILinkType IPortType.LinkAttached
        { 
            get => LinkAttached;
            set => LinkAttached = (ILinkType<TNode>)value;
        }
        #endregion

        public PortDefault()
        {
            PortValueType = typeof(string);
        }

        public virtual object GetPortValue()
        {
            return null;
        }

        public virtual bool CanLinkTo(IPortType<TNode> otherPort)
        {
            // can't connect same direction nodes
            if (PortDirection == otherPort.PortDirection)
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