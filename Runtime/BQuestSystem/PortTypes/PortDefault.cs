using System;

namespace Bloodthirst.System.Quest.Editor
{
    public class PortDefault : IPortType
    {        
        public Type PortType { get; protected set; }

        public string PortName { get; set; }

        public INodeType ParentNode { get; set; }
        
        public NODE_DIRECTION NodeDirection { get; set; }

        public ILinkType LinkAttached { get; set; }

        public PortDefault()
        {
            PortType = typeof(string);
        }

        public virtual object GetPortValue()
        {
            return null;
        }

        public virtual bool CanLinkTo(IPortType otherPort)
        {
            // can't connect same direction nodes
            if (NodeDirection == otherPort.NodeDirection)
                return false;

            return true;
        }
    }
}