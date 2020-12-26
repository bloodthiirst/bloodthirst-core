using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public abstract class QueueCommandBase<T> : CommandBase<T> where T : QueueCommandBase<T>
    {
        private Queue<ICommandBase> cached;
        private CommandBatchQueue queue;
        private bool isInterrupted;
        public QueueCommandBase() : base()
        {
            cached = new Queue<ICommandBase>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T AddToQueue(ICommandBase command)
        {
            cached.Enqueue(command);
            return (T) this;
        }

        public override void OnStart()
        {
            queue = CommandManager.AppendBatch<CommandBatchQueue>(this , true);

            // add the commands from AddToQueue
            while (cached.Count != 0)
            {
                queue.Append(cached.Dequeue());
            }

            // add the queue commands from QueueCommands
            foreach (ICommandBase cmd in QueueCommands())
            {
                if (cmd != null)
                    queue.Append(cmd);
            }

            cached = null;
        }

        /// <summary>
        /// Add commands internally to execute in the CommandQueue
        /// , note : these commands are executed AFTER the cached commands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ICommandBase> QueueCommands()
        {
            yield return null;
        }

        /// <summary>
        /// Are the children commands done ?
        /// </summary>
        public bool CommandsAreDone
        {
            get
            {
                return queue.CommandsList.Count == 0;
            }
        }

        public override void OnTick(float delta)
        {
            // the lifetime of the queue command depends on its children commands
            if(CommandsAreDone)
            {
                Success();
            }
        }

        public override void OnInterrupt()
        {
            // interrup the child queue commands first
            queue.Interrupt();
            isInterrupted = true;
            // continue the interruption
            base.OnInterrupt();
        }

        public override void OnEnd()
        {
            if (!isInterrupted && queue.BatchState != BATCH_STATE.DONE)
            {
                queue.Interrupt();
            }
        }

    }
}
