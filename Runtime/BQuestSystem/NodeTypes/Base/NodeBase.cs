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
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT }
            };

            OutputPorts = new List<IPortType>()
            {
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDefault(){ ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT }
            };

        }
    }
}
