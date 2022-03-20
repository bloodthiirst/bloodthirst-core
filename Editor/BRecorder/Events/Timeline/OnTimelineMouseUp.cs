using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineMouseUp: BRecorderEventBase
    {
        public MouseUpEvent MouseUpEvent { get; }

        public OnTimelineMouseUp(BRecorder recorder, MouseUpEvent MouseUpEvent) : base(recorder)
        {
            this.MouseUpEvent = MouseUpEvent;
        }
    }
}
