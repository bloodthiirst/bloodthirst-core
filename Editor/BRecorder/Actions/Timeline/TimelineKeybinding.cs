using System;

namespace Bloodthirst.Editor.BRecorder
{
    public class TimelineKeybinding : BRecorderActionBase
    {
        public TimelineKeybinding(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineKeydown>(HandleKeydown); 
        }

        private void HandleKeydown(OnTimelineKeydown evt)
        {
            if(evt.KeyDownEvent.keyCode == UnityEngine.KeyCode.R)
            {
                Recorder.CurrentHorizontalZoom = 1;
                Recorder.HorizontalScrollValue = 0;
            }
        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineKeydown>(HandleKeydown);
        }


    }
}
