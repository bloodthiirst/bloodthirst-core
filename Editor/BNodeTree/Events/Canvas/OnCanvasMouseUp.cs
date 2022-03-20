using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasMouseUp : BNodeTreeEventBase
    {
        public MouseUpEvent MouseUpEvent { get; }

        public OnCanvasMouseUp(INodeEditor nodeEditor, MouseUpEvent MouseUpEvent) : base(nodeEditor)
        {
            this.MouseUpEvent = MouseUpEvent;
        }
    }
}
