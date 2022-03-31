using System;
using UnityEditor;

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
            switch (evt.NewState)
            {
                case Runtime.BRecorder.BRecorderRuntime.RECORDER_STATE.IDLE:
                    {
                        // if was playing or recording
                        // then exit
                        if (EditorApplication.isPlaying)
                        {
                            EditorApplication.isPaused = false;
                            EditorApplication.ExitPlaymode();
                        }
                        break;
                    }
                case Runtime.BRecorder.BRecorderRuntime.RECORDER_STATE.PLAYING:
                    {
                        // if was idle
                        // then enter playmode
                        if (evt.OldState == Runtime.BRecorder.BRecorderRuntime.RECORDER_STATE.IDLE)
                        {
                            EditorApplication.EnterPlaymode();
                        }

                        // if was paused
                        // then resume playing
                        if (evt.OldState == Runtime.BRecorder.BRecorderRuntime.RECORDER_STATE.PAUSED)
                        {
                            EditorApplication.isPaused = false;
                        }

                        break;
                    }
                case Runtime.BRecorder.BRecorderRuntime.RECORDER_STATE.PAUSED:
                    {
                        EditorApplication.isPaused = true;
                        break;
                    }
                case Runtime.BRecorder.BRecorderRuntime.RECORDER_STATE.RECORDING:
                    {
                        if (!EditorApplication.isPlaying)
                        {
                            EditorApplication.EnterPlaymode();
                        }
                        break;
                    }
            }

            Recorder.RefreshState();
        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineStateChanged>(HandleStateChanged);
        }


    }
}
