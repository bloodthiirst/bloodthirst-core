using Bloodthirst.Runtime.BNodeTree;
using System;

namespace Bloodthirst.Editor.BNodeTree
{
    public class AddPortInputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeAddInput>(HandleNodeAddInput);
        }
        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeAddInput>(HandleNodeAddInput);
            NodeEditor.BEventSystem.Listen<OnNodeAddInput>(HandleNodeAddInput);
        }

        private void HandleNodeAddInput(OnNodeAddInput evt)
        {
            Type defaultPortType = typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType);

            IPortType defaultPort = (IPortType)Activator.CreateInstance(defaultPortType);
            defaultPort.PortDirection = PORT_DIRECTION.INPUT;
            defaultPort.PortType = PORT_TYPE.VARIABLE;
            defaultPort.PortName = "Input";

            evt.Node.NodeType.AddPort(defaultPort , defaultPort.PortDirection , defaultPort.PortType);
        }
    }
}
