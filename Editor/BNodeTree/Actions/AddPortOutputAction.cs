using System;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class AddPortOutputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnNodeAddOutput -= HandleNodeAddOutput;
        }
        public override void OnEnable()
        {
            NodeEditor.OnNodeAddOutput -= HandleNodeAddOutput;
            NodeEditor.OnNodeAddOutput += HandleNodeAddOutput;
        }

        private void HandleNodeAddOutput(NodeBaseElement node)
        {
            Type defaultPortType = typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType);

            IPortType defaultPort = (IPortType) Activator.CreateInstance(defaultPortType);
            defaultPort.PortDirection = PORT_DIRECTION.OUTPUT;
            defaultPort.PortType = PORT_TYPE.VARIABLE;
            defaultPort.PortName = "Output";

            node.NodeType.AddPort(defaultPort, defaultPort.PortDirection, defaultPort.PortType);
        }
    }
}
