using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineScrollWheel : BRecorderEventBase
    {
        public WheelEvent WheelEvent { get; }

        public OnTimelineScrollWheel(BRecorderEditor recorder, WheelEvent WheelEvent) : base(recorder)
        {
            this.WheelEvent = WheelEvent;
        }
    }
}
