using Bloodthirst.BJson;
using Bloodthirst.Editor.BInspector;

namespace Bloodthirst.Runtime.BNodeTree
{
    public interface ILinkType<TNode> : ILinkType where TNode : INodeType<TNode>
    {
        [BJsonIgnore]
        [BInspectorIgnore]
        IPortType<TNode> FromTyped { get; set; }

        [BJsonIgnore]
        [BInspectorIgnore]
        IPortType<TNode> ToTyped { get; set; }
    }

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