using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public abstract class QueueCommandBase<T> : CommandBase<T> where T : QueueCommandBase<T>
    {
        private Queue<ICommandBase> cached;
        private CommandBatchQueue queue;
        public QueueCommandBase() : base()
        {
            cached = new Queue<ICommandBase>();
        }

        public T AddToQueue(ICommandBase command)
        {
            cached.Enqueue(command);
            return (T) this;
        }

        public override void OnStart()
        {
            queue = CommandManager.AppendBatch<CommandBatchQueue>(this);

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

        protected virtual IEnumerable<ICommandBase> QueueCommands()
        {
            yield return null;
        }

        public bool CommandsAreDone
        {
            get
            {
                return queue.CommandsList.Count == 0;
            }
        }

        public override void OnTick(float delta)
        {
            if(CommandsAreDone)
            {
                Success();
            }
        }

        public override void OnEnd()
        {
            queue.Interrupt();
            CommandManager.RemoveBatch(queue);
        }

    }
}
