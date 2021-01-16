using Bloodthirst.Core.Clonable;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class ParallelLayer
    {
        [SerializeField]
        private List<CommandSettings> waitableCommmands = default;

        [SerializeField]
        private List<CommandSettings> interruptableCommmands = default;

        [SerializeField]
        private List<CommandSettings> nonSyncedCommands = default;

        /// <summary>
        /// The essential layer that defines if the layer is done
        /// </summary>
        public List<CommandSettings> WaitableCommmands => waitableCommmands;

        /// <summary>
        /// Commands that can be interrupted when the layer is done
        /// </summary>
        public List<CommandSettings> InterruptableCommmands => interruptableCommmands;

        /// <summary>
        /// Commands that will run until they are done regardless
        /// </summary>
        public List<CommandSettings> NonSyncedCommands => nonSyncedCommands;
        public ParallelLayer()
        {
            waitableCommmands = new List<CommandSettings>();
            interruptableCommmands = new List<CommandSettings>();
            nonSyncedCommands = new List<CommandSettings>();
        }

        /// <summary>
        /// Add waitable command that defines the lifespan of the layer
        /// </summary>
        /// <param name="commandBase"></param>
        /// <returns></returns>
        public ParallelLayer AppendWaitable(ICommandBase commandBase , bool interruptOnFail = false)
        {
            waitableCommmands.Add( new CommandSettings() { Command = commandBase, InterruptBatchOnFail = interruptOnFail });

            return this;
        }

        /// <summary>
        /// Add interruptable command that get interrupted as soon as the layer (waitable commands) is done
        /// </summary>
        /// <param name="commandBase"></param>
        /// <returns></returns>
        public ParallelLayer AppendInterruptable(ICommandBase commandBase, bool interruptOnFail = false)
        {
            interruptableCommmands.Add(new CommandSettings() { Command = commandBase, InterruptBatchOnFail = interruptOnFail });

            return this;
        }

        /// <summary>
        /// Add a command that will start with the layer and end on its own terms
        /// </summary>
        /// <param name="commandBase"></param>
        /// <returns></returns>
        public ParallelLayer AppendNonSync(ICommandBase commandBase)
        {
            nonSyncedCommands.Add(new CommandSettings() { Command = commandBase, InterruptBatchOnFail = false });
            return this;
        }

        public virtual ParallelLayer GenerateNext()
        {
            return null;
        }
    }
}
