using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public abstract class NodeBase : INodeType
    {
        public int NodeID { get; set; } = -1;
        public List<IPortType> InputPortsConst { get; set; }
        public List<IPortType> InputPortsVariable { get; set; }
        public List<IPortType> OutputPortsConst { get; set; }
        public List<IPortType> OutputPortsVariable { get; set; }

        protected virtual void SetupPorts()
        {
            InputPortsConst = new List<IPortType>()
            {
                new PortDefault(){ PortName ="Input 1", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ PortName ="Input 2", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ PortName ="Input 3", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ PortName ="Input 4", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT }
            };

            OutputPortsConst = new List<IPortType>()
            {
                new PortDefault(){ PortName ="Output 1", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDefault(){ PortName ="Output 2", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDefault(){ PortName ="Output 3", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT }
            };

            OutputPortsVariable = new List<IPortType>();
            InputPortsVariable = new List<IPortType>();
        }

        public NodeBase()
        {
            SetupPorts();
        }
    }
}
