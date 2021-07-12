using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeType
    {
        [IgnoreBindable]
        int NodeID { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        List<IPortType> InputPortsConst { get; set; }
        List<IPortType> InputPortsVariable { get; set; }

        List<IPortType> OutputPortsConst { get; set; }
        List<IPortType> OutputPortsVariable { get; set; }
    }
}