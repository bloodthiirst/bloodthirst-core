﻿using Bloodthirst.Core.SceneManager;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.Setup
{
    public class ListAsyncWrapper : IAsynOperationWrapper
    {
        private readonly IList<IAsynOperationWrapper> subOperations;

        private readonly bool showLoadingScreen;
        public bool ShowLoadingScreen => showLoadingScreen;
        public ListAsyncWrapper(IList<IAsynOperationWrapper> subOperations, bool showLoadingScreen)
        {
            this.subOperations = subOperations;
            this.showLoadingScreen = showLoadingScreen;
        }
        public bool ShouldExecute()
        {
            foreach(var subOperation in subOperations)
            {
                if (subOperation.ShouldExecute())
                    return true;
            }

            return false;
        }

        IProgressCommand IAsynOperationWrapper.CreateOperation()
        {
            return new ListsAyncOperation(subOperations);
        }
    }

    public class ListsAyncOperation : BasicListCommand , IProgressCommand
    {
        private readonly IList<IAsynOperationWrapper> subOperations;
        private List<IProgressCommand> subCommands;

        public ListsAyncOperation(IList<IAsynOperationWrapper> subOperations) : base(true)
        {
            this.subOperations = subOperations;

            subCommands = new List<IProgressCommand>();
        }

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
        string IProgressCommand.TaskName => $"Loading {subOperations.Count} scenes";
        public override void OnStart()
        {
            for (int i = 0; i < subOperations.Count; i++)
            {
                IAsynOperationWrapper op = subOperations[i];

                if (!op.ShouldExecute())
                    continue;

                var cmd = op.CreateOperation();

                cmd.OnCurrentProgressChanged += Cmd_OnCurrentProgressChanged;

                subCommands.Add(cmd);

                Add(cmd, true);
            }

            base.OnStart();
        }

        private void Cmd_OnCurrentProgressChanged(IProgressCommand arg1, float arg2, float arg3)
        {
            Refresh();
        }

        private void Refresh()
        {
            float curr = 0;
            float total = subCommands.Count;

            for (int i = 0; i < total; i++)
            {
                IProgressCommand op = subCommands[i];

                curr += op.CurrentProgress;
            }

            CurrentProgress = curr / total;


            if (currentProgress != 1)
                return;

            for (int i = 0; i < total; i++)
            {
                IProgressCommand op = subCommands[i];

                op.OnCurrentProgressChanged -= Cmd_OnCurrentProgressChanged;
            }

            subCommands.Clear();

            Success();
        }
    }
}
