namespace Bloodthirst.Runtime.BNodeTree
{
    public class LinkDefault<TNode> : ILinkType
    {
        public IPortType From { get; set; }
        public IPortType To { get; set; }
    }
}