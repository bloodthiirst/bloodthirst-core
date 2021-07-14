using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    [NodeMenuPath("Dialog")]
    [NodeName("Dialog Node")]
    [NodeDescription("Default node that represents dialog structure")]
    public class DialogNode : NodeBase
    {
        public string Description { get; set; }

        protected override void SetupPorts()
        {
            InputPortsConst = new List<IPortType>()
            {
                new PortDialogOption(){ PortName ="Dialog 1", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDialogOption(){ PortName ="Dialog 2", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDialogOption(){ PortName ="Dialog 3", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT },
                new PortDialogOption(){ PortName ="Dialog 4", ParentNode = this, NodeDirection = NODE_DIRECTION.INPUT }
            };

            OutputPortsConst = new List<IPortType>()
            {
                new PortDialogOption(){ PortName ="Dialog 1", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDialogOption(){ PortName ="Dialog 2", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT },
                new PortDialogOption(){ PortName ="Dialog 3", ParentNode = this, NodeDirection = NODE_DIRECTION.OUTPUT }
            };

            OutputPortsVariable = new List<IPortType>();
            InputPortsVariable = new List<IPortType>();
        }
    }
}
