namespace Bloodthirst.System.Quest.Editor
{
    public class PortDialogOption : PortDefault
    {
        public string Content { get; set; }
        public override object GetPortValue()
        {
            return Content;
        }

        public PortDialogOption()
        {
            PortType = typeof(string);
        }
    }
}