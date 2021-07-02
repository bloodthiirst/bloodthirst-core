using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeType
    {
        int NodeID { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        List<IPortType> InputPorts { get; set; }

        List<IPortType> OutputPorts { get; set; }
    }
}