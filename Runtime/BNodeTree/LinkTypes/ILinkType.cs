using Bloodthirst.BJson;
using Bloodthirst.Runtime.BInspector;

namespace Bloodthirst.Runtime.BNodeTree
{
    public interface ILinkType
    {
        [BJsonIgnore]
        [BInspectorIgnore]
        IPortType From { get; set; }

        [BJsonIgnore]
        [BInspectorIgnore]
        IPortType To { get; set; }
    }
}