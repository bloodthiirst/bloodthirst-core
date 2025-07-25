﻿using Bloodthirst.BJson;
using Bloodthirst.Runtime.BInspector;
using System;

namespace Bloodthirst.Runtime.BNodeTree
{
    public enum PORT_DIRECTION
    {
        INPUT, OUTPUT
    }

    public enum PORT_TYPE
    {
        VARIABLE, CONST
    }

    public interface IPortType
    {
        [BInspectorIgnore]
        Type PortValueType { get; }

        [BInspectorIgnore]
        string PortName { get; set; }

        [BInspectorIgnore]
        PORT_DIRECTION PortDirection { get; set; }

        [BInspectorIgnore]
        PORT_TYPE PortType { get; set; }

        object GetPortValue();

        [BJsonIgnore]
        [BInspectorIgnore]
        INodeType ParentNode { get; set; }

        [BJsonIgnore]
        [BInspectorIgnore]
        ILinkType LinkAttached { get; set; }

        bool CanLinkTo(IPortType otherPort);
    }
}