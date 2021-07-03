using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    public enum NODE_DIRECTION
    {
        INPUT, OUTPUT
    }

    public interface IPortType
    {
        Type PortType { get;}

        string PortName { get; }

        object GetPortValue();

        NODE_DIRECTION NodeDirection { get; set; }

        [JsonIgnore]
        INodeType ParentNode { get; set; }

        [JsonIgnore]
        ILinkType LinkAttached { get; set; }

        bool CanLinkTo(IPortType otherPort);
    }
}