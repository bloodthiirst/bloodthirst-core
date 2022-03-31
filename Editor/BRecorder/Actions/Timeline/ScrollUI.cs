using System;

namespace Bloodthirst.Editor.BRecorder
{
    public class ScrollUI : BRecorderActionBase
    {
        public ScrollUI(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineHorizontalScrollChanged>(HandleHorizontalScroll);
            
        }

        private void HandleHorizontalScroll(OnTimelineHorizontalScrollChanged evt)
        {
            Recorder.HorizontalScrollValue = evt.HorizontalScroll;
        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineHorizontalScrollChanged>(HandleHorizontalScroll);
        }


    }
}
