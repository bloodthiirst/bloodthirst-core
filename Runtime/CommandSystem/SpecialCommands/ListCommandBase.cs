using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public abstract class ListCommandBase<T> : CommandBase<T> where T : ListCommandBase<T>
    {
        private List<CommandSettings> cached;
        private CommandBatchList list;
        private bool isInterrupted;
        public ListCommandBase() : base()
        {
            cached = new List<CommandSettings>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T AddToList(ICommandBase command , bool interruptOnFail)
        {
            var cmd = new CommandSettings() { Command = command, InterruptBatchOnFail = interruptOnFail };
            return AddToList(cmd);
        }

        internal T AddToList(CommandSettings commandSettings)
        {
            cached.Add(commandSettings);
            return (T) this;
        }

        public override void OnStart()
        {
            list = CommandManager.AppendBatch<CommandBatchList>(this , true);

            // add the commands from AddToQueue
            while (cached.Count != 0)
            {
                list.Append(cached[0]);
                cached.RemoveAt(0);
            }

            // add the queue commands from QueueCommands
            foreach (CommandSettings cmd in ListCommands())
            {
                if (cmd.Command != null)
                    list.Append(cmd);
            }

            list.OnCommandRemoved += List_OnCommandRemoved;
            list.OnCommandRemoved += List_OnCommandRemoved;

            cached = null;
        }

        private void List_OnCommandRemoved(ICommandBatch arg1, ICommandBase arg2)
        {
            // the lifetime of the queue command depends on its children commands
            if (CommandsAreDone)
            {
                list.OnCommandRemoved -= List_OnCommandRemoved;

                if (list.BatchState == BATCH_STATE.INTERRUPTED)
                {
                    Interrupt();
                }
                else
                {
                    Success();
                }
            }
        }

        /// <summary>
        /// Add commands internally to execute in the CommandQueue
        /// , note : these commands are executed AFTER the cached commands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual IEnumerable<CommandSettings> ListCommands()
        {
            yield break;
        }

        /// <summary>
        /// Are the children commands done ?
        /// </summary>
        public bool CommandsAreDone
        {
            get
            {
                return list.CommandsList.Count == 0;
            }
        }

        public override void OnInterrupt()
        {
            // interrupt the child queue incase the interrupt was called by the Commands Interrupt method
            if (list.BatchState == BATCH_STATE.EXECUTING)
            {
                list.Interrupt();
                list.OnCommandRemoved -= List_OnCommandRemoved;

            }
        }

        public override void OnEnd()
        {
            // interrupt the child queue incase the interrupt was called by the Commands Interrupt method
            if (list.BatchState == BATCH_STATE.EXECUTING)
            {
                list.Interrupt();
                list.OnCommandRemoved -= List_OnCommandRemoved;
            }
        }

    }
}
