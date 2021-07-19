using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeType<TTNode> where TTNode : INodeType<TTNode>
    {
        [IgnoreBindable]
        int NodeID { get; set; }
        IEnumerable<IPortType<TTNode>> InputPortsConstTyped { get; }
        IEnumerable<IPortType<TTNode>> InputPortsVariableTyped { get; }

        IEnumerable<IPortType<TTNode>> OutputPortsConstTyped { get; }
        IEnumerable<IPortType<TTNode>> OutputPortsVariableTyped { get; }

        void AddInputConst<TPort>(TPort input) where TPort : IPortType, IPortType<TTNode>;
        void AddOutputConst<TPort>(TPort output) where TPort : IPortType, IPortType<TTNode>;
        void AddInput<TPort>(TPort input) where TPort : IPortType, IPortType<TTNode>;
        void AddOutput<TPort>(TPort input) where TPort : IPortType, IPortType<TTNode>;
    }

    public interface INodeType
    {
        [IgnoreBindable]
        int NodeID { get; set; }

        IEnumerable<IPortType> InputPortsConst { get; }
        IEnumerable<IPortType> InputPortsVariable { get; }

        IEnumerable<IPortType> OutputPortsConst { get; }
        IEnumerable<IPortType> OutputPortsVariable { get; }

        void AddInputConst<TPort>(TPort input) where TPort : IPortType;
        void AddOutputConst<TPort>(TPort output) where TPort : IPortType;
        void AddInput<TPort>(TPort input) where TPort : IPortType;
        void AddOutput<TPort>(TPort input) where TPort : IPortType;
    }
}
