using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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

    public class LoadingManager : MonoBehaviour
#if UNITY_EDITOR
        , IPreprocessBuildWithReport
#endif
    {
#if UNITY_EDITOR
        public int callbackOrder => 100;

        public void OnPreprocessBuild(BuildReport report)
        {
            ScenesListData.Instance.InitializeScenes();
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

        [ShowInInspector]
        private CommandBatchQueue loadingQueue;

        private List<IProgressCommand> runningCommands;

        private List<IProgressCommand> remainingCommands;

        public IProgressCommand CurrentTask()
        {
            if (remainingCommands.Count == 0)
                return null;

            return remainingCommands[0];
        }

        public void Initialize(CommandManagerBehaviour commandManager)
        {
            State = LOADDING_STATE.FREE;
            Progress = 0;

            runningCommands = new List<IProgressCommand>();
            remainingCommands = new List<IProgressCommand>();


            loadingQueue = commandManager.AppendBatch<CommandBatchQueue>(this, false);
            loadingQueue.OnCommandAdded += HandleCommandAdded;
            loadingQueue.OnCommandRemoved += HandleCommandRemoved;

        }

        public void Interrupt()
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
                IProgressCommand curr = runningCommands[i];
                /*
                progressSum += curr.CurrentProgress * curr.TotalOpsCount;
                totalOpsSum += curr.TotalOpsCount;
                */

                progressSum += curr.CurrentProgress;
                totalOpsSum++;


                notDone = notDone || (curr.CurrentProgress != 1);
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

        private void SetStateAndProgress(LOADDING_STATE state, float progress)
        {
            bool stateChanged = false;
            bool progressChanged = false;
            if (state != State)
            {
                stateChanged = true;
                State = state;
            }

            if (progress != Progress)
            {
                progressChanged = true;
                Progress = progress;
            }

            if (stateChanged)
            {
                OnLoadingStatusChanged?.Invoke(this);
            }

            if (progressChanged)
            {
                OnLoadingProgressChanged?.Invoke(this);
            }
        }

        private void HandleProgressChanged(IProgressCommand progressCommand , float oldValue , float newValue)
        {
            RefreshState();
        }

        private void HandleCommandRemoved(ICommandBatch batch, ICommandBase cmd)
        {
            IProgressCommand casted = (IProgressCommand)cmd;
            casted.OnCurrentProgressChanged -= HandleProgressChanged;


            remainingCommands.Remove(casted);

            RefreshState();
        }

        private void HandleCommandAdded(ICommandBatch batch, ICommandBase cmd)
        {
            IProgressCommand casted = (IProgressCommand)cmd;
            casted.OnCurrentProgressChanged += HandleProgressChanged;

            runningCommands.Add(casted);
            remainingCommands.Add(casted);

            RefreshState();
        }

        public void Load(IAsynOperationWrapper asyncOperations)
        {
            IProgressCommand cmd = asyncOperations.CreateOperation();
            loadingQueue.Append(cmd);
        }

    }

}