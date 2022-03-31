using Bloodthirst.Runtime.BRecorder;
using System;
using System.Linq;

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

            if(BRecorderRuntime.CurrentSession != null)
            {
                BRecorderRuntime.CurrentSession.OnSessionChanged -= HandleSessionChanged;
                BRecorderRuntime.CurrentSession.OnSessionChanged += HandleSessionChanged;

                BRecorderRuntime.CurrentSession.OnCommandAdded -= HandleCommandAdded;
                BRecorderRuntime.CurrentSession.OnCommandAdded += HandleCommandAdded;

                BRecorderRuntime.CurrentSession.OnCommandRemoved -= HandleCommandRemoved;
                BRecorderRuntime.CurrentSession.OnCommandRemoved += HandleCommandRemoved;
            }
        }

        private void HandleSessionChanged(OnTimelineSessionChanged evt)
        {
            if (evt.OldSession != null)
            {
                evt.OldSession.OnSessionChanged -= HandleSessionChanged;
                evt.OldSession.OnCommandAdded -= HandleCommandAdded;
                evt.OldSession.OnCommandRemoved -= HandleCommandRemoved;
            }

            if (evt.NewSession != null)
            {
                evt.NewSession.OnSessionChanged -= HandleSessionChanged;
                evt.NewSession.OnSessionChanged += HandleSessionChanged;

                evt.NewSession.OnCommandAdded -= HandleCommandAdded;
                evt.NewSession.OnCommandAdded += HandleCommandAdded;

                evt.NewSession.OnCommandRemoved -= HandleCommandRemoved;
                evt.NewSession.OnCommandRemoved += HandleCommandRemoved;

            }

            Recorder.RepaintViewport();
            Recorder.RepaintScroller();
            Recorder.RepaintTimeAxis();
            Recorder.RepaintTimline();

            if (BRecorderRuntime.CurrentSession == null || BRecorderRuntime.CurrentSession.Commands.Count == 0)
            {
                Recorder.CurrentHorizontalZoom = 1;
            }
            else
            {
                Recorder.CurrentHorizontalZoom = Recorder.ZoomNeededToShowUntil(BRecorderRuntime.CurrentSession.Commands.Last().GameTime);
            }

        }

        private void HandleCommandRemoved(IBRecorderCommand cmd)
        {
            Recorder.RemoveCommand(cmd);
        }

        private void HandleCommandAdded(IBRecorderCommand cmd)
        {
            Recorder.AddCommand(cmd);
        }

        private void HandleSessionChanged(BRecorderSession session)
        {

        }

        public override void Destroy()
        {
            if (BRecorderRuntime.CurrentSession != null)
            {
                BRecorderRuntime.CurrentSession.OnSessionChanged -= HandleSessionChanged;
                BRecorderRuntime.CurrentSession.OnCommandAdded -= HandleCommandAdded;
                BRecorderRuntime.CurrentSession.OnCommandRemoved -= HandleCommandRemoved;
            }

            Recorder.EventSystem.Unlisten<OnTimelineSessionChanged>(HandleSessionChanged);
        }


    }
}
