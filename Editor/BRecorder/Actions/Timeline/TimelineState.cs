using System;

namespace Bloodthirst.Editor.BRecorder
{
    public class TimelineState : BRecorderActionBase
    {
        public TimelineState(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineStateChanged>(HandleStateChanged); 
        }

        private void HandleStateChanged(OnTimelineStateChanged evt)
        {
            Recorder.RefreshState();
        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineStateChanged>(HandleStateChanged);
        }


    }
}
