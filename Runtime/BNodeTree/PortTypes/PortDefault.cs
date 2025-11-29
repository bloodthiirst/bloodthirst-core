using Bloodthirst.Runtime.BInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Runtime.BNodeTree
{
    public class PortDefault<TNode> : IPortType where TNode : INodeType
    {
        #region PortDefault<TNode> implementation
        [BInspectorIgnore]
        public Type PortValueType { get; set; }

        [OdinSerialize]
        public string PortName { get; set; }
        [BInspectorIgnore]
        public INodeType ParentNode { get; set; }
        
        [BInspectorIgnore]
        [OdinSerialize]
        public PORT_DIRECTION PortDirection { get; set; }
        
        [BInspectorIgnore]
        [OdinSerialize]
        public PORT_TYPE PortType { get; set; }

        [BInspectorIgnore]
        public List<ILinkType> LinkAttached { get; set; } = new List<ILinkType>();
        #endregion

        public PortDefault()
        {
            PortValueType = typeof(string);
        }

        public virtual object GetPortValue()
        {
            return null;
        }

        public virtual bool CanLinkTo(IPortType otherPort)
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
    }
}