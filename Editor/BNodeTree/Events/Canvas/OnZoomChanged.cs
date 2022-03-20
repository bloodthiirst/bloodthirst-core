namespace Bloodthirst.Editor.BNodeTree
{
    public class OnZoomChanged : BNodeTreeEventBase
    {
        public float PreviousZoom { get; }
        public float CurrentZoom { get; }
        public OnZoomChanged(INodeEditor nodeEditor, float PreviousZoom, float CurrentZoom) : base(nodeEditor)
        {
            this.PreviousZoom = PreviousZoom;
            this.CurrentZoom = CurrentZoom;
        }

    }
}
