namespace Bloodthirst.System.Quest.Editor
{
    public class LinkDefault : ILinkType
    {
        public IPortType From { get; set; }
        public IPortType To { get; set; }
    }
}