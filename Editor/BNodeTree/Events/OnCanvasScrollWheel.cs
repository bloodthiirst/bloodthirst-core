using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasScrollWheel : BNodeTreeEventBase
    {
        public WheelEvent WheelEvent { get; }

        public OnCanvasScrollWheel(INodeEditor nodeEditor, WheelEvent WheelEvent) : base(nodeEditor)
        {
            this.WheelEvent = WheelEvent;
        }
    }
}
