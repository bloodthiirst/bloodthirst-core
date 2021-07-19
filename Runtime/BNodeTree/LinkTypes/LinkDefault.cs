namespace Bloodthirst.System.Quest.Editor
{
    public class LinkDefault<TNode> :  ILinkType<TNode> , ILinkType  where TNode : INodeType<TNode>
    {
        public IPortType<TNode> From { get; set; }
        public IPortType<TNode> To { get; set; }

        IPortType ILinkType.From { get => (IPortType) From; set => From = (IPortType<TNode>)value; }

        IPortType ILinkType.To { get => (IPortType)To; set => To = (IPortType<TNode>)value; }
        
        IPortType<TNode> ILinkType<TNode>.FromTyped { get => From; set => From = value; }
        IPortType<TNode> ILinkType<TNode>.ToTyped { get => To; set => To = value; }
        
    }
}