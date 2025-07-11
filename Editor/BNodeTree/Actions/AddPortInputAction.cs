using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Linq;

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
            Type customPortType = TypeUtils.AllTypes.Where(t => t.BaseType == typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType)).FirstOrDefault();
            Type instanceType = defaultPortType;

            if(customPortType != null)
            {
                instanceType = customPortType;
            }

            IPortType defaultPort = (IPortType)Activator.CreateInstance(instanceType);
            defaultPort.PortDirection = PORT_DIRECTION.INPUT;
            defaultPort.PortType = PORT_TYPE.VARIABLE;
            defaultPort.PortName = "Input";

            evt.Node.NodeType.AddPort(defaultPort);
        }
    }
}
