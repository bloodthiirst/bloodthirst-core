namespace Bloodthirst.System.Quest.Editor
{
    public class LinkDefault : ILinkType
    {
        public IPortType From { get; set; }
        public IPortType To { get; set; }

        IPortType ILinkType.From { get => From; set => From = value; }
        IPortType ILinkType.To { get => To; set => From = value; }
    }
}