using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineMouseDown : BRecorderEventBase
    {
        public MouseDownEvent MouseDownEvent { get; }

        public OnTimelineMouseDown(BRecorder recorder, MouseDownEvent MouseDownEvent) : base(recorder)
        {
            this.MouseDownEvent = MouseDownEvent;
        }
    }
}
