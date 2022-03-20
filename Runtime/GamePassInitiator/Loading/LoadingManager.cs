using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public enum LOADDING_STATE
    {
        FREE,
        LOADING
    }
    public class LoadingManager : MonoBehaviour, IPreGameSetup, IPreGameEnd
#if UNITY_EDITOR
        , IPreprocessBuildWithReport
#endif
    {
#if UNITY_EDITOR
        public int callbackOrder => 100;

        public void OnPreprocessBuild(BuildReport report)
        {
            ScenesListData.Instance.LoadAllScenesAvailable();
        }
#endif
        public event Action<LoadingManager> OnLoadingProgressChanged;

        public event Action<LoadingManager> OnLoadingStatusChanged;

        [SerializeField]
        private float progress;

        [SerializeField]
        private LOADDING_STATE state;

        /// <summary>
        /// Progress value between 0 -> 1
        /// </summary>
        public float Progress
        {
            get => progress;
            private set
            {
                progress = value;
            }
        }

        public LOADDING_STATE State
        {
            get => state;
            private set
            {
                state = value;
            }
        }

        private CommandBatchQueue loadingQueue;

        private List<AsyncOperationsCommand> runningCommands;

        int IPreGameSetup.Order => 1;
        void IPreGameSetup.Execute()
        {
            State = LOADDING_STATE.FREE;
            Progress = 0;

            runningCommands = new List<AsyncOperationsCommand>();

            loadingQueue = BProviderRuntime.Instance.GetSingleton<CommandManagerBehaviour>().AppendBatch<CommandBatchQueue>(this, false);
            loadingQueue.OnCommandAdded += HandleCommandAdded;
            loadingQueue.OnCommandRemoved += HandleCommandRemoved;

            BProviderRuntime.Instance.RegisterSingleton(this);
        }

        void IPreGameEnd.Execute()
        {
            loadingQueue.Interrupt();
            loadingQueue.OnCommandAdded -= HandleCommandAdded;
            loadingQueue.OnCommandRemoved -= HandleCommandRemoved;
        }

        private void RefreshState()
        {
            if (runningCommands.Count == 0)
            {
                SetStateAndProgress(LOADDING_STATE.FREE, 0);
                return;
            }
            
            float progressSum = 0f;
            float totalOpsSum = 0f;

            bool notDone = false;

            for (int i = 0; i < runningCommands.Count; i++)
            {
                AsyncOperationsCommand curr = runningCommands[i];
                progressSum += curr.Progress * curr.TotalOpsCount;
                totalOpsSum += curr.TotalOpsCount;

                notDone = notDone || (curr.Progress != 1);
            }

            float progressPerecentage = progressSum / totalOpsSum;

            if (notDone)
            {
                SetStateAndProgress(LOADDING_STATE.LOADING, progressPerecentage);
                return;

            }

            runningCommands.Clear();
            SetStateAndProgress(LOADDING_STATE.FREE, 0);
        }

        private void SetStateAndProgress(LOADDING_STATE state , float progress)
        {
            bool stateChanged = false;
            bool progressChanged = false;
            if(state != State)
            {
                stateChanged = true;
                State = state;
            }

            if (progress != Progress)
            {
                progressChanged = true;
                Progress = progress;
            }

            if(stateChanged)
            {
                OnLoadingStatusChanged?.Invoke(this);
            }

            if(progressChanged)
            {
                OnLoadingProgressChanged?.Invoke(this);
            }
        }

        private void HandleProgressChanged(AsyncOperationsCommand obj)
        {
            RefreshState();
        }

        private void HandleCommandRemoved(ICommandBatch batch, ICommandBase cmd)
        {
            AsyncOperationsCommand casted = (AsyncOperationsCommand)cmd;
            casted.OnCurrentProgressChanged -= HandleProgressChanged;

            RefreshState();
        }

        private void HandleCommandAdded(ICommandBatch batch, ICommandBase cmd)
        {
            AsyncOperationsCommand casted = (AsyncOperationsCommand)cmd;
            casted.OnCurrentProgressChanged += HandleProgressChanged;

            runningCommands.Add(casted);

            RefreshState();
        }

        public void Load(IAsynOperationWrapper asyncOperations)
        {
            AsyncOperationsCommand cmd = new AsyncOperationsCommand(asyncOperations);
            loadingQueue.Append(cmd);
        }

    }

}