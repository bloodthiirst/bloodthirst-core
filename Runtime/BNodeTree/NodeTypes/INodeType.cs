using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeType<TTNode> where TTNode : INodeType<TTNode>
    {
        [IgnoreBindable]
        int NodeID { get; set; }
        event Action<IPortType<TTNode>> OnPortAddedTyped;
        event Action<IPortType<TTNode>> OnPortRemovedTyped;
        IReadOnlyList<IPortType> Ports { get; }
        void AddPort<TPort>(TPort input, PORT_DIRECTION direction, PORT_TYPE type) where TPort : IPortType, IPortType<TTNode>;
        void RemovePort<TPort>(TPort input, PORT_DIRECTION direction, PORT_TYPE type) where TPort : IPortType, IPortType<TTNode>;
    }

    public interface INodeType
    {
        [IgnoreBindable]
        int NodeID { get; set; }
        event Action<IPortType> OnPortAdded;
        event Action<IPortType> OnPortRemoved;
        IReadOnlyList<IPortType> Ports { get; }
        void AddPort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type) where TPort : IPortType;
        void RemovePort<TPort>(TPort port, PORT_DIRECTION direction, PORT_TYPE type) where TPort : IPortType;
    }
}
