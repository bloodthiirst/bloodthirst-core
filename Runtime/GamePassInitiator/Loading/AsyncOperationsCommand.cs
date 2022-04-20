using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public class AsyncOperationsCommand : CommandBase<AsyncOperationsCommand>
    {
        public event Action<AsyncOperationsCommand> OnCurrentProgressChanged;
        private IAsynOperationWrapper asyncOperations { get; set; }
        private List<AsyncOperation> asyncOperationsInProgress { get; set; }
        public float TotalOpsCount { get; private set; }

        private float progress;

        public float Progress
        {
            get => progress;
            private set
            {
                if (progress == value)
                    return;

                progress = value;
                OnCurrentProgressChanged?.Invoke(this);
            }
        }
        public AsyncOperationsCommand(IAsynOperationWrapper asyncOps)
        {
            asyncOperations = asyncOps;
            TotalOpsCount = asyncOperations.OperationsCount();
            Progress = 0;
        }

        public override void OnStart()
        {
            asyncOperationsInProgress = new List<AsyncOperation>(asyncOperations.StartOperations());
        }

        public override void OnTick(float delta)
        {
            float progressSum = 0;

            for (int i = 0; i < TotalOpsCount; i++)
            {
                AsyncOperation curr = asyncOperationsInProgress[i];
                progressSum += curr.progress;
            }

            Progress = progressSum / TotalOpsCount;

            if (Progress == 1)
            {
                Success();
                return;
            }
        }
    }
}