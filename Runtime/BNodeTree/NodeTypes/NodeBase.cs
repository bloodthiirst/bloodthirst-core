using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Runtime.BNodeTree
{
    public abstract class NodeBase<T> : INodeType where T : NodeBase<T>
    {
        [OdinSerialize]
        private int nodeID = -1;

        [OdinSerialize]
        private List<IPortType> ports = new List<IPortType>();
        
        public event Action<IPortType> OnPortAdded;

        public event Action<IPortType> OnPortRemoved;

        public int NodeID { get => nodeID; set => nodeID = value; }
        public List<IPortType> Ports { get => ports; set => ports = value; }

        IReadOnlyList<IPortType> INodeType.Ports => Ports;

        /// <summary>
        /// Method used to add and assemble all the ports of the node
        /// </summary>
        public virtual void SetupInitialPorts() { }

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
