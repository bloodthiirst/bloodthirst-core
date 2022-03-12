using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasMouseMove : BNodeTreeEventBase
    {
        public MouseMoveEvent MouseMoveEvent { get; }

        public OnCanvasMouseMove(INodeEditor nodeEditor, MouseMoveEvent MouseMoveEvent) : base(nodeEditor)
        {
            this.MouseMoveEvent = MouseMoveEvent;
        }
    }
}
