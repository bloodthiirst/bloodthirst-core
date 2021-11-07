using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    public enum PORT_DIRECTION
    {
        INPUT, OUTPUT
    }

    public enum PORT_TYPE
    {
        VARIABLE, CONST
    }

    public interface IPortType<TNode> where TNode : INodeType<TNode>
    {
        [IgnoreBindable]
        Type PortValueType { get;}

        [IgnoreBindable]
        string PortName { get; set; }

        [IgnoreBindable]
        [JsonProperty]
        PORT_DIRECTION PortDirection { get; set; }

        [IgnoreBindable]
        [JsonProperty]
        PORT_TYPE PortType { get; set; }

        object GetPortValue();

        [JsonIgnore]
        [IgnoreBindable]
        TNode ParentNode { get; set; }

        [JsonIgnore]
        [IgnoreBindable]
        ILinkType<TNode> LinkAttached { get; set; }

        bool CanLinkTo(IPortType<TNode> otherPort);
    }

    public interface IPortType
    {
        [IgnoreBindable]
        Type PortValueType { get;  }

        [IgnoreBindable]
        string PortName { get; set; }

        [IgnoreBindable]
        PORT_DIRECTION PortDirection { get; set; }

        [IgnoreBindable]
        PORT_TYPE PortType { get; set; }

        object GetPortValue();
        [JsonIgnore]
        [IgnoreBindable]
        INodeType ParentNode { get; set; }

        [JsonIgnore]
        [IgnoreBindable]
        ILinkType LinkAttached { get; set; }

        bool CanLinkTo(IPortType otherPort);
    }
}