using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public abstract class NodeBase<T> : INodeType<T>, INodeType where T : NodeBase<T>
    {
        public int NodeID { get; set; } = -1;
        public List<IPortType<T>> InputsConst { get; set; }
        public List<IPortType<T>> InputsVariable { get; set; }
        public List<IPortType<T>> OutputsConst { get; set; }
        public List<IPortType<T>> OutputsVariable { get; set; }

        private List<IPortType> InputsConstTyped { get; set; }
        private List<IPortType> InputsVariableTyped { get; set; }
        private List<IPortType> OutputsConstTyped { get; set; }
        private List<IPortType> OutputsVariableTyped { get; set; }

        #region INodeType implementation
        int INodeType.NodeID { get => NodeID; set => NodeID = value; }
        IEnumerable<IPortType> INodeType.InputPortsConst => InputsConstTyped;
        IEnumerable<IPortType> INodeType.InputPortsVariable => InputsVariableTyped;
        IEnumerable<IPortType> INodeType.OutputPortsConst => OutputsConstTyped;
        IEnumerable<IPortType> INodeType.OutputPortsVariable => OutputsVariableTyped;
        #endregion

        #region INodeType<NodeBase> implementation
        int INodeType<T>.NodeID { get => NodeID; set => NodeID = value; }

        IEnumerable<IPortType<T>> INodeType<T>.InputPortsConst => InputsConst;

        IEnumerable<IPortType<T>> INodeType<T>.InputPortsVariable => InputsVariable;

        IEnumerable<IPortType<T>> INodeType<T>.OutputPortsConst => OutputsConst;

        IEnumerable<IPortType<T>> INodeType<T>.OutputPortsVariable => OutputsVariable;
        #endregion

        protected virtual void SetupPorts()
        {
            InputsConst = new List<IPortType<T>>();
            OutputsConst = new List<IPortType<T>>();

            OutputsVariable = new List<IPortType<T>>();
            InputsVariable = new List<IPortType<T>>();

            InputsConstTyped = new List<IPortType>();
            InputsVariableTyped = new List<IPortType>();
            OutputsConstTyped = new List<IPortType>();
            OutputsVariableTyped = new List<IPortType>();

            AddInputConst(new PortDefault<T>() { PortName = "Input 1", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.INPUT });
            AddInputConst(new PortDefault<T>() { PortName = "Input 2", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.INPUT });
            AddInputConst(new PortDefault<T>() { PortName = "Input 3", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.INPUT });
            AddInputConst(new PortDefault<T>() { PortName = "Input 4", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.INPUT });

            AddOutput(new PortDefault<T>() { PortName = "Output 1", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.OUTPUT });
            AddOutput(new PortDefault<T>() { PortName = "Output 2", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.OUTPUT });
            AddOutput(new PortDefault<T>() { PortName = "Output 3", ParentNode = (T)this, NodeDirection = NODE_DIRECTION.OUTPUT });
        }

        public void AddInput<TPort>(TPort input) where TPort : IPortType, IPortType<T>
        {
            InputsVariable.Add(input);
            InputsVariableTyped.Add(input);
        }

        public void AddOutput<TPort>(TPort output) where TPort : IPortType, IPortType<T>
        {
            OutputsVariable.Add(output);
            OutputsVariableTyped.Add(output);
        }
        public void AddInputConst<TPort>(TPort input) where TPort : IPortType, IPortType<T>
        {
            InputsConst.Add(input);
            InputsConstTyped.Add(input);
        }

        public void AddOutputConst<TPort>(TPort output) where TPort : IPortType, IPortType<T>
        {
            OutputsConst.Add(output);
            OutputsConstTyped.Add(output);
        }

        public NodeBase()
        {
            SetupPorts();
        }
    }
}
