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

        /// <summary>
        /// The essential layer that defines if the layer is done
        /// </summary>
        public List<ICommandBase> WaitableCommmands => waitableCommmands;

        /// <summary>
        /// Commands that can be interrupted when the layer is done
        /// </summary>
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

        /// <summary>
        /// Add waitable command that defines the lifespan of the layer
        /// </summary>
        /// <param name="commandBase"></param>
        /// <returns></returns>
        public ParallelLayer AppendWaitable(ICommandBase commandBase)
        {
            waitableCommmands.Add(commandBase);

            return this;
        }

        /// <summary>
        /// Add interruptable command that get interrupted as soon as the layer (waitable commands) is done
        /// </summary>
        /// <param name="commandBase"></param>
        /// <returns></returns>
        public ParallelLayer AppendInterruptable(ICommandBase commandBase)
        {

            interruptableCommmands.Add(commandBase);

            return this;
        }

        /// <summary>
        /// Add a command that will start with the layer and end on its own terms
        /// </summary>
        /// <param name="commandBase"></param>
        /// <returns></returns>
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
