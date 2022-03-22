using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineKeydown : BRecorderEventBase
    {
        public KeyDownEvent KeyDownEvent { get; }

        public OnTimelineKeydown(BRecorderEditor recorder, KeyDownEvent KeyDownEvent) : base(recorder)
        {
            this.KeyDownEvent = KeyDownEvent;
        }
    }
}
