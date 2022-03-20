using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineScrollWheel : BRecorderEventBase
    {
        public WheelEvent WheelEvent { get; }

        public OnTimelineScrollWheel(BRecorder recorder, WheelEvent WheelEvent) : base(recorder)
        {
            this.WheelEvent = WheelEvent;
        }
    }
}
