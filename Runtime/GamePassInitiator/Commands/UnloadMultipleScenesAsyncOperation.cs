using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class UnloadMultipleScenesAsyncOperation : CommandBase<UnloadMultipleScenesAsyncOperation> , IProgressCommand
    {
        private List<string> scenes;

        private List<AsyncOperation> asyncOperations;

        private float currentProgress;
        public float CurrentProgress
        {
            get => currentProgress;
            private set
            {
                if (currentProgress == value)
                    return;

                float old = currentProgress;
                currentProgress = value;

                OnCurrentProgressChanged?.Invoke(this, old, currentProgress);
            }
        }

        public event Action<IProgressCommand, float, float> OnCurrentProgressChanged;

        string IProgressCommand.TaskName => $"Unloading {scenes.Count} Scenes";

        public UnloadMultipleScenesAsyncOperation(List<string> scenes)
        {
            this.scenes = scenes;
            asyncOperations = new List<AsyncOperation>();
        }

        public override void OnStart()
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                string scene = scenes[i];
                AsyncOperation op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                asyncOperations.Add(op);
            }
        }

        public override void OnTick(float delta)
        {
            float curr = 0;
            float total = asyncOperations.Count;

            for (int i = 0; i < total; i++)
            {
                AsyncOperation op = asyncOperations[i];

                curr += op.progress;
            }

            CurrentProgress = curr / total;


            if (currentProgress == 1)
            {
                Success();
            }
        }


    }
}
