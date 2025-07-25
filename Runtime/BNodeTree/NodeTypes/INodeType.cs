﻿using Bloodthirst.Runtime.BInspector;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Runtime.BNodeTree
{
    public interface INodeType
    {
        [BInspectorIgnore]
        int NodeID { get; set; }
        event Action<IPortType> OnPortAdded;
        event Action<IPortType> OnPortRemoved;
        [BInspectorIgnore]
        IReadOnlyList<IPortType> Ports { get; }
        void AddPort<TPort>(TPort port) where TPort : IPortType;
        void RemovePort<TPort>(TPort port) where TPort : IPortType;
    }
}
