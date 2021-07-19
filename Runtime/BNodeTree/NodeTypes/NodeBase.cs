using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quest.Editor
{
    public abstract class NodeBase<T> : INodeType<T>, INodeType where T : NodeBase<T>
    {
        public int NodeID { get; set; } = -1;

        public List<IPortType> InputsConst { get; set; }
        public List<IPortType> InputsVariable { get; set; }
        public List<IPortType> OutputsConst { get; set; }
        public List<IPortType> OutputsVariable { get; set; }

        #region INodeType implementation
        int INodeType.NodeID { get => NodeID; set => NodeID = value; }
        IEnumerable<IPortType> INodeType.InputPortsConst => InputsConst;
        IEnumerable<IPortType> INodeType.InputPortsVariable => InputsVariable;
        IEnumerable<IPortType> INodeType.OutputPortsConst => OutputsConst;
        IEnumerable<IPortType> INodeType.OutputPortsVariable => OutputsVariable;
        void INodeType.AddInput<TPort>(TPort input)
        {
            ((IPortType)input).ParentNode = this;
            ((IPortType)input).NodeDirection = NODE_DIRECTION.INPUT;

            InputsVariable.Add(input);
        }
        void INodeType.AddOutput<TPort>(TPort output)
        {
            ((IPortType)output).ParentNode = this;
            ((IPortType)output).NodeDirection = NODE_DIRECTION.OUTPUT;

            OutputsVariable.Add(output);
        }

        void INodeType.AddInputConst<TPort>(TPort input)
        {
            ((IPortType)input).ParentNode = this;
            ((IPortType)input).NodeDirection = NODE_DIRECTION.INPUT;

            InputsConst.Add(input);
        }
        void INodeType.AddOutputConst<TPort>(TPort output)
        {
            ((IPortType)output).ParentNode = this;
            ((IPortType)output).NodeDirection = NODE_DIRECTION.OUTPUT;

            OutputsConst.Add(output);
        }

        #endregion

        #region INodeType<NodeBase> implementation
        int INodeType<T>.NodeID { get => NodeID; set => NodeID = value; }

        IEnumerable<IPortType<T>> INodeType<T>.InputPortsConstTyped => InputsConst.Cast<IPortType<T>>();

        IEnumerable<IPortType<T>> INodeType<T>.InputPortsVariableTyped => InputsVariable.Cast<IPortType<T>>();

        IEnumerable<IPortType<T>> INodeType<T>.OutputPortsConstTyped => OutputsConst.Cast<IPortType<T>>();

        IEnumerable<IPortType<T>> INodeType<T>.OutputPortsVariableTyped => OutputsVariable.Cast<IPortType<T>>();

        void INodeType<T>.AddInput<TPort>(TPort input)
        {
            AddInput(input);
        }
        void INodeType<T>.AddOutput<TPort>(TPort output)
        {
            AddOutput(output);
        }

        void INodeType<T>.AddInputConst<TPort>(TPort input)
        {
            AddInputConst(input);
        }
        void INodeType<T>.AddOutputConst<TPort>(TPort output)
        {
            AddOutputConst(output);
        }

        #endregion

        protected virtual void SetupPorts() { }



        public void AddInput<TPort>(TPort input) where TPort : IPortType, IPortType<T>
        {
            ((IPortType)input).ParentNode = this;
            ((IPortType)input).NodeDirection = NODE_DIRECTION.INPUT;

            InputsVariable.Add(input);
        }

        public void AddOutput<TPort>(TPort output) where TPort : IPortType, IPortType<T>
        {
            ((IPortType)output).ParentNode = this;
            ((IPortType)output).NodeDirection = NODE_DIRECTION.OUTPUT;

            OutputsVariable.Add(output);
        }
        public void AddInputConst<TPort>(TPort input) where TPort : IPortType, IPortType<T>
        {
            ((IPortType)input).ParentNode = this;
            ((IPortType)input).NodeDirection = NODE_DIRECTION.INPUT;

            InputsConst.Add(input);
        }

        public void AddOutputConst<TPort>(TPort output) where TPort : IPortType, IPortType<T>
        {
            ((IPortType)output).ParentNode = this;
            ((IPortType)output).NodeDirection = NODE_DIRECTION.OUTPUT;

            OutputsConst.Add(output);
        }



        public NodeBase()
        {
            InputsConst = new List<IPortType>();
            OutputsConst = new List<IPortType>();
            OutputsVariable = new List<IPortType>();
            InputsVariable = new List<IPortType>();

            SetupPorts();
        }
    }
}
