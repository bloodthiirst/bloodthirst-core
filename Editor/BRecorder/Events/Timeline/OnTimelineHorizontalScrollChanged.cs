using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineHorizontalScrollChanged: BRecorderEventBase
    {
        public float HorizontalScroll { get; }

        public OnTimelineHorizontalScrollChanged(BRecorderEditor recorder, float HorizontalScroll) : base(recorder)
        {
            this.HorizontalScroll = HorizontalScroll;
        }
    }
}
