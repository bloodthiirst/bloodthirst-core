using Bloodthirst.Core.Clonable;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class ParallelLayer
    {
        [SerializeField]
        private List<ICommandBase> waitableCommmands = default;

        [SerializeField]
        private List<ICommandBase> interruptableCommmands = default;

        [SerializeField]
        private List<ICommandBase> nonSyncedCached = default;

        private CommandBatchList nonSyncedCommandBatch = default;

        public List<ICommandBase> WaitableCommmands => waitableCommmands;

        public List<ICommandBase> InterruptableCommmands => interruptableCommmands;


        private bool isStarted;

        public bool IsStarted => isStarted;

        public bool IsDone => IsDoneInternal();

        public void Start()
        {
            nonSyncedCommandBatch = CommandManager.AppendBatch<CommandBatchList>(this);

            for(int i = 0; i < nonSyncedCached.Count; i++)
            {
                nonSyncedCommandBatch.Append(nonSyncedCached[i]);
            }

            isStarted = true;
        }

        public ParallelLayer()
        {
            waitableCommmands = new List<ICommandBase>();
            interruptableCommmands = new List<ICommandBase>();
            nonSyncedCached = new List<ICommandBase>();
            isStarted = false;
        }

        private bool IsDoneInternal()
        {
            for(int i = 0; i < waitableCommmands.Count; i++)
            {
                if (!waitableCommmands[i].GetExcutingCommand().IsDone)
                    return false;
            }

            return true;
        }

        public ParallelLayer AppendWaitable(ICommandBase commandBase)
        {
            waitableCommmands.Add(commandBase);

            return this;
        }

        public ParallelLayer AppendInterruptable(ICommandBase commandBase)
        {

            interruptableCommmands.Add(commandBase);

            return this;
        }

        public ParallelLayer AppendNonSync(ICommandBase commandBase)
        {
            nonSyncedCached.Add(commandBase);
            return this;
        }

        public virtual ParallelLayer GenerateNext()
        {
            return null;
        }
    }
}
