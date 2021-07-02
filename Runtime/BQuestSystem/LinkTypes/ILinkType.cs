using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    public interface ILinkType
    {
        [JsonIgnore]
        IPortType From { get; set; }

        [JsonIgnore]
        IPortType To { get; set; }
    }
}