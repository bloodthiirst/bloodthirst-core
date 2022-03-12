using System;

namespace Bloodthirst.Runtime.BNodeTree
{
    public abstract class PortWithType<T,TNode> : IPortType<TNode> where TNode : INodeType<TNode>
    {
        private static Type type = typeof(T);

        public Type PortValueType => type;
        public string PortName { get; set; }
        public TNode ParentNode { get; set; }
        public PORT_DIRECTION PortDirection { get; set; }
        public PORT_TYPE PortType { get; set; }
        public ILinkType<TNode> LinkAttached { get; set; }

        public PortWithType() { }

        public object GetPortValue()
        {
            return GetValue();
        }

        protected abstract T GetValue();

        public bool CanLinkTo(IPortType<TNode> otherPort)
        {
            // can't connect same direction nodes
            if (PortDirection == otherPort.PortDirection)
                return false;

            return true;
        }
    }
}