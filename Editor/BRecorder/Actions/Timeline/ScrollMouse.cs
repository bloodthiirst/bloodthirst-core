using System;

namespace Bloodthirst.Editor.BRecorder
{
    public class ScrollMouse : BRecorderActionBase
    {
        private bool canDrag;

        public ScrollMouse(BRecorder recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineScrollWheel>(HandleScrollWheel);
            Recorder.EventSystem.Listen<OnTimelineMouseDown>(HandleMouseDown);
            Recorder.EventSystem.Listen<OnTimelineMouseMove>(HandleMouseMove);
            Recorder.EventSystem.Listen<OnTimelineMouseUp>(HandleMouseUp);
            Recorder.EventSystem.Listen<OnTimelineMouseLeave>(HandleMouseLeave);        
        }

        private void HandleMouseLeave(OnTimelineMouseLeave obj)
        {
            canDrag = false;
        }

        private void HandleMouseUp(OnTimelineMouseUp evt)
        {
            canDrag = false;
        }

        private void HandleMouseDown(OnTimelineMouseDown evt)
        {
            canDrag = true;
        }

        private void HandleScrollWheel(OnTimelineScrollWheel evt)
        {
            Recorder.CurrentZoom -= evt.WheelEvent.delta.y * 0.01f;
        }
        private void HandleMouseMove(OnTimelineMouseMove evt)
        {
            if (!canDrag)
                return;

            Recorder.HorizontalScrollValue -= evt.MouseMoveEvent.mouseDelta.x / Recorder.CurrentZoom;
        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineScrollWheel>(HandleScrollWheel);
            Recorder.EventSystem.Unlisten<OnTimelineMouseDown>(HandleMouseDown);
            Recorder.EventSystem.Unlisten<OnTimelineMouseMove>(HandleMouseMove);
            Recorder.EventSystem.Unlisten<OnTimelineMouseUp>(HandleMouseUp);
            Recorder.EventSystem.Unlisten<OnTimelineMouseLeave>(HandleMouseLeave);
        }




    }
}
