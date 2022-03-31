using Bloodthirst.Runtime.BRecorder;
using System;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.BRecorder
{
    [Serializable]
    internal struct BRecorderState
    {
        [SerializeField]
        internal BRecorderRuntime.RECORDER_STATE recorderState;

        [SerializeField]
        internal string openAsset;

        [SerializeField]
        internal bool updateEveyframe_Value;

        [SerializeField]
        internal bool recordOnGameStart_Value;

        [SerializeField]
        internal bool showCurrentFrame_Value;

    }
}