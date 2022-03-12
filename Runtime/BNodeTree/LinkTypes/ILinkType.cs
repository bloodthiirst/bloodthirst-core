using Bloodthirst.BJson;

namespace Bloodthirst.Runtime.BNodeTree
{
    public interface ILinkType<TNode> : ILinkType where TNode : INodeType<TNode>
    {
        [BJsonIgnore]
        IPortType<TNode> FromTyped { get; set; }

        [BJsonIgnore]
        IPortType<TNode> ToTyped { get; set; }
    }

    public interface ILinkType
    {
        [BJsonIgnore]
        IPortType From { get; set; }

        [BJsonIgnore]
        IPortType To { get; set; }
    }
}