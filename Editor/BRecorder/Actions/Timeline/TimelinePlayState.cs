using Bloodthirst.Runtime.BRecorder;
using System;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.BRecorder
{
    public class TimelinePlayState : BRecorderActionBase
    {

        public TimelinePlayState(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeChanged;
        }

        private void HandlePlayModeChanged(PlayModeStateChange state)
        {
            BRecorderBehaviour recorder = UnityEngine.Object.FindObjectOfType<BRecorderBehaviour>();

            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    {
                        Recorder.LoadSettings();

                        break;
                    }
                case PlayModeStateChange.EnteredPlayMode:
                    {
                        Recorder.LoadSettings();

                        if (BRecorderRuntime.RecordingState == BRecorderRuntime.RECORDER_STATE.PLAYING)
                        {
                            break;
                        }

                        if (BRecorderRuntime.RecordingState == BRecorderRuntime.RECORDER_STATE.IDLE && Recorder.StartOnGameStart)
                        {
                            BRecorderRuntime.StartRecording();
                            break;
                        }

                        break;

                    }

                case PlayModeStateChange.ExitingEditMode:
                    {
                        Recorder.SaveSettings();

                        if (Recorder.OpenSession.value != null && BRecorderRuntime.RecordingState == BRecorderRuntime.RECORDER_STATE.PLAYING)
                        {
                            if(recorder == null)
                            {
                                GameObject go = new GameObject("Added Recorder Behaviour");
                                recorder = go.AddComponent<BRecorderBehaviour>();
                                recorder.playOnAwake = true;
                            }

                            recorder.recorderAsset = (BRecorderAsset) Recorder.OpenSession.value;
                            recorder.playOnAwake = true;
                        }
                        else
                        {
                            if (recorder != null)
                            {
                                recorder.recorderAsset = null;
                                recorder.playOnAwake = false;
                            }
                        }

                        break;
                    }
                case PlayModeStateChange.ExitingPlayMode:
                    {
                        BRecorderRuntime.StopRecording();

                        Recorder.SaveSettings();

                        if (recorder != null)
                        {
                            recorder.recorderAsset = null;
                            recorder.playOnAwake = false;
                        }

                        break;
                    }
            }

            Recorder.RefreshState();
        }


        public override void Destroy()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeChanged;
        }


    }
}
