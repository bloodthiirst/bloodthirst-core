using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    public enum NODE_DIRECTION
    {
        INPUT, OUTPUT
    }

    public interface IPortType<TNode> where TNode : INodeType<TNode>
    {
        [IgnoreBindable]
        Type PortType { get;}

        [IgnoreBindable]
        string PortName { get; }
        [IgnoreBindable]
        NODE_DIRECTION NodeDirection { get; set; }

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
        Type PortType { get; }

        [IgnoreBindable]
        string PortName { get; }
        [IgnoreBindable]
        NODE_DIRECTION NodeDirection { get; set; }

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