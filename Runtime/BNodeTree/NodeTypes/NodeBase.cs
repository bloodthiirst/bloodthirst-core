using System;
using System.Collections.Generic;

namespace Bloodthirst.Runtime.BNodeTree
{
    public abstract class NodeBase<T> : INodeType<T>, INodeType where T : NodeBase<T>
    {
        public event Action<IPortType<T>> OnPortAddedTyped;

        public event Action<IPortType<T>> OnPortRemovedTyped;

        public event Action<IPortType> OnPortAdded;

        public event Action<IPortType> OnPortRemoved;

        public int NodeID { get; set; } = -1;
        public List<IPortType> Ports { get; set; }

        #region INodeType implementation
        int INodeType.NodeID { get => NodeID; set => NodeID = value; }
        IReadOnlyList<IPortType> INodeType.Ports => Ports;

        void INodeType.AddPort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type)
        {
            AddPortInternal(port, direction, type);
        }

        void INodeType.RemovePort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type)
        {
            RemovePortInternal(port, direction, type);
        }

        #endregion

        #region INodeType<NodeBase> implementation
        int INodeType<T>.NodeID { get => NodeID; set => NodeID = value; }

        IReadOnlyList<IPortType> INodeType<T>.Ports => Ports;

        void INodeType<T>.AddPort<TPort>(TPort input, PORT_DIRECTION direction, PORT_TYPE type)
        {
            AddPort(input, direction, type);
        }

        void INodeType<T>.RemovePort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type)
        {
            RemovePort(port, direction, type);
        }

        #endregion

        public NodeBase()
        {
            Ports = new List<IPortType>();

            SetupPorts();
        }

        /// <summary>
        /// Method used to add and assemble all the ports of the node
        /// </summary>
        protected virtual void SetupPorts() { }

        public void AddPort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type) where TPort : IPortType, IPortType<T>
        {
            AddPortInternal(port, direction, type);
        }

        public void RemovePort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type) where TPort : IPortType, IPortType<T>
        {
            RemovePortInternal(port, direction, type);
        }

        private void AddPortInternal(IPortType port, PORT_DIRECTION direction, PORT_TYPE type)
        {
            port.ParentNode = this;
            port.PortDirection = direction;
            port.PortType = type;

            Ports.Add(port);

            OnPortAdded?.Invoke(port);
            OnPortAddedTyped?.Invoke(port as IPortType<T>);
        }

        private void RemovePortInternal(IPortType port, PORT_DIRECTION direction, PORT_TYPE type)
        {
            if (!Ports.Remove(port))
                return;

            OnPortRemoved?.Invoke(port);
            OnPortRemovedTyped?.Invoke(port as IPortType<T>);
        }
    }
}
