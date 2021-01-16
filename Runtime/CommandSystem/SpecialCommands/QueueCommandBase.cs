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
        public QueueCommandBase() : base()
        {
            cached = new Queue<CommandSettings>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T AddToQueue(ICommandBase command, bool interruptOnFail = false)
        {
            cached.Enqueue(new CommandSettings() { Command = command, InterruptBatchOnFail = interruptOnFail });
            return (T)this;
        }

        public override void OnStart()
        {
            queue = CommandManager.AppendBatch<CommandBatchQueue>(this, true);

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

            queue.OnCommandRemoved -= Queue_OnCommandRemoved;
            queue.OnCommandRemoved += Queue_OnCommandRemoved;
        }

        private void Queue_OnCommandRemoved(ICommandBatch arg1, ICommandBase arg2)
        {
            // the lifetime of the queue command depends on its children commands
            if (CommandsAreDone)
            {
                queue.OnCommandRemoved -= Queue_OnCommandRemoved;

                if (queue.BatchState == BATCH_STATE.INTERRUPTED)
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


        public override void OnInterrupt()
        {
            // interrupt the child queue incase the interrupt was called by the Commands Interrupt method
            if (queue.BatchState == BATCH_STATE.EXECUTING)
            {
                queue.Interrupt();
                queue.OnCommandRemoved -= Queue_OnCommandRemoved;

            }
        }

        public override void OnEnd()
        {
            if (queue.BatchState == BATCH_STATE.EXECUTING)
            {
                queue.Interrupt();
                queue.OnCommandRemoved -= Queue_OnCommandRemoved;
            }
        }

    }
}
