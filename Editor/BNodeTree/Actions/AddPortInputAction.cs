using System;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class AddPortInputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnNodeAddInput -= HandleNodeAddInput;
        }
        public override void OnEnable()
        {
            NodeEditor.OnNodeAddInput -= HandleNodeAddInput;
            NodeEditor.OnNodeAddInput += HandleNodeAddInput;
        }

        private void HandleNodeAddInput(NodeBaseElement node)
        {
            Type defaultPortType = typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType);

            IPortType defaultPort = (IPortType)Activator.CreateInstance(defaultPortType);
            defaultPort.PortDirection = PORT_DIRECTION.INPUT;
            defaultPort.PortType = PORT_TYPE.VARIABLE;
            defaultPort.PortName = "Input";

            node.NodeType.AddPort(defaultPort , defaultPort.PortDirection , defaultPort.PortType);
        }
    }
}
