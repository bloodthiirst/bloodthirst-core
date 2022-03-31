using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BRecorder;
using System;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.BRecorder
{
    public class TimelineSave: BRecorderActionBase
    {
        public TimelineSave(BRecorderEditor recorder) : base(recorder)
        {
        }

        public override void Initialize()
        {
            Recorder.EventSystem.Listen<OnTimelineSave>(HandleSave); 
        }

        private void HandleSave(OnTimelineSave evt)
        {
            string title = $"Session - {DateTime.Now.ToString(@"Da\te yyyy-MM-dd A\t HH\hmm")}";
            string savePath = EditorUtility.SaveFilePanel("Save Recorded session", "Assets", title, "asset");

            if(string.IsNullOrEmpty(savePath))
            {
                return;
            }

            BRecorderAsset newAsset = ScriptableObject.CreateInstance<BRecorderAsset>();
            newAsset.Session = BRecorderRuntime.CurrentSession;

            string relativePath = EditorUtils.AbsoluteToRelativePath(savePath);

            BRecorderAsset getExistingFile = AssetDatabase.LoadAssetAtPath<BRecorderAsset>(relativePath);

            if(getExistingFile != null)
            {
                EditorUtility.CopySerializedIfDifferent(newAsset, getExistingFile);
            }
            else
            {
                AssetDatabase.CreateAsset(newAsset, relativePath);
            }
            

        }

        public override void Destroy()
        {
            Recorder.EventSystem.Unlisten<OnTimelineSave>(HandleSave);
        }


    }
}
