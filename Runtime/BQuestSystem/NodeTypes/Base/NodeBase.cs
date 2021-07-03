using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public abstract class NodeBase : INodeType
    {
        public int NodeID { get; set; } = -1;
        public string Name { get; set; }
        public string Description { get; set; }
        public List<IPortType> InputPorts { get; set; }
        public List<IPortType> OutputPorts { get; set; }

        public NodeBase()
        {
            InputPorts = new List<IPortType>()
            {
                new PortDefault(){ PortName ="Input 1", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ PortName ="Input 2", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ PortName ="Input 3", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ PortName ="Input 4", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT }
            };

            OutputPorts = new List<IPortType>()
            {
                new PortDefault(){ PortName ="Output 1", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDefault(){ PortName ="Output 2", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDefault(){ PortName ="Output 3", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT }
            };

        }
    }
}
