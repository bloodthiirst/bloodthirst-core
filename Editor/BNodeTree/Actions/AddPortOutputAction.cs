using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class AddPortOutputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortRequestAddOutput>(HandleNodeAddOutput);
        }
        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortRequestAddOutput>(HandleNodeAddOutput);
            NodeEditor.BEventSystem.Listen<OnPortRequestAddOutput>(HandleNodeAddOutput);
        }

        private void HandleNodeAddOutput(OnPortRequestAddOutput evt)
        {
            Type defaultPortType = typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType);
            Type customPortType = TypeUtils.AllTypes.Where(t => t.BaseType == typeof(PortDefault<>).MakeGenericType(NodeEditor.NodeBaseType)).FirstOrDefault();
            Type instanceType = defaultPortType;

            if (customPortType != null)
            {
                instanceType = customPortType;
            }

            IPortType defaultPort = (IPortType)Activator.CreateInstance(instanceType);
            defaultPort.PortDirection = PORT_DIRECTION.INPUT;
            defaultPort.PortDirection = PORT_DIRECTION.OUTPUT;
            defaultPort.PortType = PORT_TYPE.VARIABLE;
            defaultPort.PortName = "Output";

            evt.Node.NodeType.AddPort(defaultPort);
        }
    }
}
