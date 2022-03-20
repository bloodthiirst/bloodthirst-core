using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class OnWindowKeyPressed : BNodeTreeEventBase
    {
        public KeyDownEvent KeyDownEvent { get; }

        public OnWindowKeyPressed(INodeEditor nodeEditor, KeyDownEvent KeyDownEvent) : base(nodeEditor)
        {
            this.KeyDownEvent = KeyDownEvent;
        }
    }
}
