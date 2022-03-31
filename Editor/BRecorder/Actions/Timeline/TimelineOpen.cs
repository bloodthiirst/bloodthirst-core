using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BRecorder;
using System;
using UnityEditor;

namespace Bloodthirst.Editor.BRecorder
{
    public class TimelineOpen : BRecorderActionBase
    {
        public TimelineOpen(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineOpen>(HandleOpen); 
        }

        private void HandleOpen(OnTimelineOpen evt)
        {
            string filePath = EditorUtility.OpenFilePanel("Open BRecorder Session", "Assets", "asset");

            if (string.IsNullOrEmpty(filePath))
                return;

            string relativePath = EditorUtils.AbsoluteToRelativePath(filePath);

            BRecorderAsset asset = AssetDatabase.LoadAssetAtPath<BRecorderAsset>(relativePath);

            if (asset == null)
            {
                EditorUtility.DisplayDialog("Error" , "File is not valid" , "ok");
                return;
            }

            BRecorderRuntime.CurrentSession = asset.Session;
        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineOpen>(HandleOpen);
        }


    }
}
