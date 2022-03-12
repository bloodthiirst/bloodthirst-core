using Bloodthirst.Runtime.BNodeTree;
using System;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class AddPortOutputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeAddOutput>(HandleNodeAddOutput);
        }
        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeAddOutput>(HandleNodeAddOutput);
            NodeEditor.BEventSystem.Listen<OnNodeAddOutput>(HandleNodeAddOutput);
        }

        private void HandleNodeAddOutput(OnNodeAddOutput evt)
        {
            Type defaultPortType = typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType);

            IPortType defaultPort = (IPortType) Activator.CreateInstance(defaultPortType);
            defaultPort.PortDirection = PORT_DIRECTION.OUTPUT;
            defaultPort.PortType = PORT_TYPE.VARIABLE;
            defaultPort.PortName = "Output";

            evt.Node.NodeType.AddPort(defaultPort, defaultPort.PortDirection, defaultPort.PortType);
        }
    }
}
