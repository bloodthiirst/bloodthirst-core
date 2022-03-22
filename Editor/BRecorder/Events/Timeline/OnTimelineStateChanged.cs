using Bloodthirst.Runtime.BRecorder;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineStateChanged : BRecorderEventBase
    {
        public BRecorderRuntime.RECORDER_STATE OldState { get; }
        public BRecorderRuntime.RECORDER_STATE NewState { get; }

        public OnTimelineStateChanged(BRecorderEditor recorder, BRecorderRuntime.RECORDER_STATE OldState, BRecorderRuntime.RECORDER_STATE NewState) : base(recorder)
        {
            this.OldState = OldState;
            this.NewState = NewState;
        }
    }
}
