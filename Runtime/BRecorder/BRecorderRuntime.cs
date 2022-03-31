using System;
using System.Collections;
using UnityEngine;

namespace Bloodthirst.Runtime.BRecorder
{
    public static class BRecorderRuntime
    {
        public enum RECORDER_STATE
        {
            IDLE,
            PLAYING,
            PAUSED,
            RECORDING,
        }

        public static event Action<BRecorderSession, BRecorderSession> OnSessionChanged;
        public static event Action<RECORDER_STATE, RECORDER_STATE> OnStateChanged;

        private static RECORDER_STATE recordingState;
        public static RECORDER_STATE RecordingState
        {
            get => recordingState;
            private set
            {
                if (recordingState == value)
                    return;

                RECORDER_STATE old = recordingState;
                recordingState = value;

                OnStateChanged?.Invoke(old, recordingState);
            }
        }

        private static BRecorderSession currentSession;
        public static BRecorderSession CurrentSession
        {
            get => currentSession;
            set
            {
                BRecorderSession old = currentSession;
                currentSession = value;

                OnSessionChanged?.Invoke(old, currentSession);
            }

        }

        public static void StartRecording()
        {
            CurrentSession = new BRecorderSession();

            RecordingState = RECORDER_STATE.RECORDING;
        }

        public static void SetRecording()
        {
            RecordingState = RECORDER_STATE.RECORDING;
        }

        public static void StopRecording()
        {
            RecordingState = RECORDER_STATE.IDLE;
        }

        public static void PlaySession()
        {
            RecordingState = RECORDER_STATE.PLAYING;
        }

        public static void PauseSession()
        {
            RecordingState = RECORDER_STATE.PAUSED;
        }
    }
}
