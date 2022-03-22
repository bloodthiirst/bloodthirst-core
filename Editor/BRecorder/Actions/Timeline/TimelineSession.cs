using Bloodthirst.Runtime.BRecorder;

namespace Bloodthirst.Editor.BRecorder
{
    public class TimelineSession : BRecorderActionBase
    {
        public TimelineSession(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineSessionChanged>(HandleSessionChanged); 
        }

        private void HandleSessionChanged(OnTimelineSessionChanged evt)
        {
            if (evt.OldSession != null)
            {
                evt.OldSession.OnSessionChanged -= HandleSessionChanged;
            }

            if (evt.NewSession != null)
            {
                evt.NewSession.OnSessionChanged -= HandleSessionChanged;
                evt.NewSession.OnSessionChanged += HandleSessionChanged;
            }

            Recorder.RepaintViewport();
            Recorder.RepaintTimeAxis();
            Recorder.RepaintTimline();

        }

        private void HandleSessionChanged(Runtime.BRecorder.BRecorderSession obj)
        {
            Recorder.RepaintViewport();
            Recorder.RepaintTimeAxis();
            Recorder.RepaintTimline();
        }

        public override void Destroy()
        {
            if(BRecorderRuntime.CurrentSession != null)
            {
                BRecorderRuntime.CurrentSession.OnSessionChanged -= HandleSessionChanged;
            }

            Recorder.EventSystem.Unlisten<OnTimelineSessionChanged>(HandleSessionChanged);
        }


    }
}
