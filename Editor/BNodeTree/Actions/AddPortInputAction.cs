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
            defaultPort.PortName = "New Input";
            node.AddVariableInputPort(defaultPort);
        }
    }
}
