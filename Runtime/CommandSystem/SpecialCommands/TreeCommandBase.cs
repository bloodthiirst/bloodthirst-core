using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public abstract class TreeCommandBase<T> : CommandBase<T> where T : TreeCommandBase<T>
    {
        private List<CommandSettings> cached;
        private CommandBatchList list;
        private CommandManager commandManager;
        private bool failIfQueueInterrupted;

        public TreeCommandBase(CommandManager commandManager = null, bool failIfQueueInterrupted = false) : base()
        {
            this.commandManager = commandManager;
            this.failIfQueueInterrupted = failIfQueueInterrupted;
            cached = new List<CommandSettings>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T AddToList(ICommandBase command, bool interruptOnFail = false)
        {
            var cmd = new CommandSettings() { Command = command, InterruptBatchOnFail = interruptOnFail };
            return AddToList(cmd);
        }

        internal T AddToList(CommandSettings commandSettings)
        {
            cached.Add(commandSettings);
            return (T)this;
        }

        public override void OnStart()
        {
            list = commandManager.AppendBatch<CommandBatchList>(this, true);

            // add the commands from AddToQueue
            for(int i = 0; i < cached.Count; i++)
            {
                list.Append(cached[i]);
            }

            cached.Clear();

            // add the queue commands from QueueCommands
            foreach (CommandSettings cmd in ListCommands())
            {
                if (cmd.Command != null)
                    list.Append(cmd);
            }

            cached = null;

            list.OnBatchEnded -= List_OnBatchEnded;
            list.OnBatchEnded += List_OnBatchEnded;
        }

        private void List_OnBatchEnded(ICommandBatch obj)
        {
            // the lifetime of the queue command depends on its children commands
            if (list.CommandsList.Count != 0)
                return;

            list.OnBatchEnded -= List_OnBatchEnded;

            if (list.BatchState == BATCH_STATE.INTERRUPTED)
            {
                if (failIfQueueInterrupted)
                {
                    Fail();
                }
                else
                {
                    Interrupt();
                }
                return;
            }
            else
            {
                Success();
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

        public override void OnEnd()
        {
            list.OnBatchEnded -= List_OnBatchEnded;

            if (list.BatchState == BATCH_STATE.EXECUTING)
            {
                list.Interrupt();
            }
        }

    }
}
