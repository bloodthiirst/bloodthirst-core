using System;

namespace Bloodthirst.System.Quest.Editor
{
    public abstract class PortWithType<T ,TNode> : IPortType<TNode> where TNode : INodeType<TNode>
    {
        private static Type type = typeof(T);

        public Type PortType => type;
        public string PortName { get; set; }
        public TNode ParentNode { get; set; }
        
        public NODE_DIRECTION NodeDirection { get; set; }

        public ILinkType<TNode> LinkAttached { get; set; }

        public PortWithType()
        {

        }

        public object GetPortValue()
        {
            return GetValue();
        }

        protected abstract T GetValue();

        public bool CanLinkTo(IPortType<TNode> otherPort)
        {
            // can't connect same direction nodes
            if (NodeDirection == otherPort.NodeDirection)
                return false;

            return true;
        }
    }
}