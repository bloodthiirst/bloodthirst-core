using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnCanvasMouseContextClick : BNodeTreeEventBase
    {
        public ContextClickEvent ClickEvent { get; }

        public OnCanvasMouseContextClick(INodeEditor nodeEditor, ContextClickEvent ClickEvent) : base(nodeEditor)
        {
            this.ClickEvent = ClickEvent;
        }
    }
}
