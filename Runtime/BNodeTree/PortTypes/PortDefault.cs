using Bloodthirst.Editor.BInspector;
using Newtonsoft.Json;
using System;

namespace Bloodthirst.Runtime.BNodeTree
{
    public class PortDefault<TNode> : IPortType<TNode>, IPortType where TNode : INodeType<TNode>, INodeType
    {
        #region PortDefault<TNode> implementation
        [BInspectorIgnore]
        public Type PortValueType { get; protected set; }
        public string PortName { get; set; }
        [BInspectorIgnore]
        internal TNode ParentNode { get; set; }
        [BInspectorIgnore]
        public PORT_DIRECTION PortDirection { get; set; }
        [BInspectorIgnore]
        internal PORT_TYPE PortType { get; set; }
        [BInspectorIgnore]
        public ILinkType<TNode> LinkAttached { get; internal set; }
        #endregion

        #region IPortType<TNode> implementation

        [BInspectorIgnore]
        Type IPortType<TNode>.PortValueType => PortValueType;

        [BInspectorIgnore]
        string IPortType<TNode>.PortName { get => PortName; set => PortName = value; }

        [BInspectorIgnore]
        PORT_TYPE IPortType<TNode>.PortType { get => PortType; set => PortType = value; }

        [BInspectorIgnore]
        PORT_DIRECTION IPortType<TNode>.PortDirection { get => PortDirection; set => PortDirection = value; }

        [BInspectorIgnore]
        TNode IPortType<TNode>.ParentNode { get => ParentNode; set => ParentNode = value; }

        [BInspectorIgnore]
        ILinkType<TNode> IPortType<TNode>.LinkAttached 
        { 
            get => LinkAttached;
            set => LinkAttached = value;
        }

        #endregion

        #region IPortType implementation

        [BInspectorIgnore]
        Type IPortType.PortValueType => PortValueType;

        [BInspectorIgnore]
        string IPortType.PortName { get => PortName; set => PortName = value; }

        [BInspectorIgnore]
        PORT_TYPE IPortType.PortType { get => PortType; set => PortType = value; }

        [BInspectorIgnore]
        PORT_DIRECTION IPortType.PortDirection { get => PortDirection; set => PortDirection = value; }

        [BInspectorIgnore]
        INodeType IPortType.ParentNode { get => ParentNode; set => ParentNode = (TNode)value; }

        [BInspectorIgnore]
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