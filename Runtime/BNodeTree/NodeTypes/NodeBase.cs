using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Runtime.BNodeTree
{
    public abstract class NodeBase<T> : INodeType where T : NodeBase<T>
    {
        public event Action<IPortType> OnPortAdded;

        public event Action<IPortType> OnPortRemoved;

        [OdinSerialize]
        public int NodeID { get; set; } = -1;

        [OdinSerialize]
        public List<IPortType> Ports { get; set; }

        IReadOnlyList<IPortType> INodeType.Ports => Ports;


        public NodeBase()
        {
            Ports = new List<IPortType>();

            SetupPorts();
        }

        /// <summary>
        /// Method used to add and assemble all the ports of the node
        /// </summary>
        protected virtual void SetupPorts() { }

        public void AddPort<TPort>(TPort port) where TPort : IPortType
        {
            port.ParentNode = this;

            Ports.Add(port);

            OnPortAdded?.Invoke(port);
        }

        public void RemovePort<TPort>(TPort port) where TPort : IPortType
        {
            if (!Ports.Remove(port))
                return;

            OnPortRemoved?.Invoke(port);
        }
    }
}
