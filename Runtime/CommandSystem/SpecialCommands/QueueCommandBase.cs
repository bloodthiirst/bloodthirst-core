using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    /// <summary>
    /// <para>Executes the subcommands sequentially using a queue order</para>
    /// <para>This doesn't account for the case when the sub-commands fail , whenever a subcommand fails it just gets dequeued a we got onto the next command</para>
    /// <para>For interruptable queue Look at <see cref="QueueInterruptableCommandBase{T}"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QueueCommandBase<T> : CommandBase<T> where T : QueueCommandBase<T>
    {
        private Queue<CommandSettings> cached;
        private CommandBatchQueue queue;
        private readonly CommandManager commandManager;
        private readonly bool failIfQueueInterrupted;

        public QueueCommandBase(CommandManager commandManager , bool failIfQueueInterrupted = false) : base()
        {
            this.commandManager = commandManager;
            this.failIfQueueInterrupted = failIfQueueInterrupted;
            cached = new Queue<CommandSettings>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T AddToQueue(ICommandBase command, bool interruptOnChildFail = false)
        {
            cached.Enqueue(new CommandSettings() { Command = command, InterruptBatchOnFail = interruptOnChildFail });
            return (T)this;
        }

        public override void OnStart()
        {
            queue = commandManager.AppendBatch<CommandBatchQueue>(this, true);

            // add the commands from AddToQueue
            while (cached.Count != 0)
            {
                queue.Append(cached.Dequeue());
            }

            // add the queue commands from QueueCommands
            foreach (CommandSettings cmd in QueueCommands())
            {
                if (cmd.Command != null)
                    queue.Append(cmd);
            }

            cached = null;

            queue.OnBatchEnded -= Queue_OnBatchEnded;
            queue.OnBatchEnded += Queue_OnBatchEnded;
        }

        private void Queue_OnBatchEnded(ICommandBatch arg1)
        {
            // the lifetime of the queue command depends on its children commands
            if (!CommandsAreDone)
            {
                return;
            }

            queue.OnBatchEnded -= Queue_OnBatchEnded;

            if (queue.BatchState == BATCH_STATE.INTERRUPTED)
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
        protected virtual IEnumerable<CommandSettings> QueueCommands()
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
                return queue.CommandsQueue.Count == 0;
            }
        }

        public override void OnEnd()
        {
            queue.OnBatchEnded -= Queue_OnBatchEnded;

            if (queue.BatchState == BATCH_STATE.EXECUTING)
            {
                queue.Interrupt();
            }
        }

    }
}
