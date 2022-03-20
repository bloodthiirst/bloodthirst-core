using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasMouseDown : BNodeTreeEventBase
    {
        public MouseDownEvent MouseDownEvent { get; }

        public OnCanvasMouseDown(INodeEditor nodeEditor, MouseDownEvent MouseDownEvent) : base(nodeEditor)
        {
            this.MouseDownEvent = MouseDownEvent;
        }
    }
}
