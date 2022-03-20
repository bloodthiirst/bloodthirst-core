using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasMouseClick : BNodeTreeEventBase
    {
        public ClickEvent ClickEvent { get; }

        public OnCanvasMouseClick(INodeEditor nodeEditor, ClickEvent ClickEvent) : base(nodeEditor)
        {
            this.ClickEvent = ClickEvent;
        }
    }
}
