using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasMouseLeave : BNodeTreeEventBase
    {
        public MouseLeaveEvent MouseLeaveEvent { get; }

        public OnCanvasMouseLeave(INodeEditor nodeEditor, MouseLeaveEvent MouseLeaveEvent) : base(nodeEditor)
        {
            this.MouseLeaveEvent = MouseLeaveEvent;
        }
    }
}
