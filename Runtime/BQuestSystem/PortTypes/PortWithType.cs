using System;

namespace Bloodthirst.System.Quest.Editor
{
    public abstract class PortWithType<T> : IPortType
    {
        private static Type type = typeof(T);

        public Type PortType => type;

        public INodeType ParentNode { get; set; }
        
        public NODE_DIRECTION NodeDirection { get; set; }

        public ILinkType LinkAttached { get; set; }

        public PortWithType()
        {

        }

        public object GetPortValue()
        {
            return GetValue();
        }

        protected abstract T GetValue();

        public bool CanLinkTo(IPortType otherPort)
        {
            // can't connect same direction nodes
            if (NodeDirection == otherPort.NodeDirection)
                return false;

            return true;
        }
    }
}