using Bloodthirst.Runtime.BNodeTree;
using System;

namespace Bloodthirst.Editor.BNodeTree
{
    public class AddPortInputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortRequestAddInput>(HandleNodeAddInput);
        }
        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortRequestAddInput>(HandleNodeAddInput);
            NodeEditor.BEventSystem.Listen<OnPortRequestAddInput>(HandleNodeAddInput);
        }

        private void HandleNodeAddInput(OnPortRequestAddInput evt)
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
