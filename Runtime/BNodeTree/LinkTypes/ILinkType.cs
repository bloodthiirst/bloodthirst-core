using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    public interface ILinkType<TNode> : ILinkType where TNode : INodeType<TNode>
    {
        [JsonIgnore]
        IPortType<TNode> FromTyped { get; set; }

        [JsonIgnore]
        IPortType<TNode> ToTyped { get; set; }
    }

    public interface ILinkType
    {
        [JsonIgnore]
        IPortType From { get; set; }
        [JsonIgnore]
        IPortType To { get; set; }
    }
}