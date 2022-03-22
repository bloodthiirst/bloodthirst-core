using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineMouseLeave: BRecorderEventBase
    {
        public MouseLeaveEvent MouseLeaveEvent { get; }

        public OnTimelineMouseLeave(BRecorderEditor recorder, MouseLeaveEvent MouseLeaveEvent) : base(recorder)
        {
            this.MouseLeaveEvent = MouseLeaveEvent;
        }
    }
}
