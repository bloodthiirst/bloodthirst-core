using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeType<TTNode> where TTNode : INodeType<TTNode>
    {
        [IgnoreBindable]
        int NodeID { get; set; }
        IEnumerable<IPortType<TTNode>> InputPortsConst { get; }
        IEnumerable<IPortType<TTNode>> InputPortsVariable { get; }

        IEnumerable<IPortType<TTNode>> OutputPortsConst { get; }
        IEnumerable<IPortType<TTNode>> OutputPortsVariable { get; }

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
    }
}