using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public abstract class ListCommandBase<T> : CommandBase<T> where T : ListCommandBase<T>
    {
        private List<ICommandBase> cached;
        private CommandBatchList list;
        private bool isInterrupted;
        public ListCommandBase() : base()
        {
            cached = new List<ICommandBase>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T AddToList(ICommandBase command)
        {
            cached.Add(command);
            return (T) this;
        }

        public override void OnStart()
        {
            list = CommandManager.AppendBatch<CommandBatchList>(this);

            // add the commands from AddToQueue
            while (cached.Count != 0)
            {
                list.Append(cached[0]);
                cached.RemoveAt(0);
            }

            // add the queue commands from QueueCommands
            foreach (ICommandBase cmd in ListCommands())
            {
                if (cmd != null)
                    list.Append(cmd);
            }

            cached = null;
        }

        /// <summary>
        /// Add commands internally to execute in the CommandQueue
        /// , note : these commands are executed AFTER the cached commands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ICommandBase> ListCommands()
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
                return list.CommandsList.Count == 0;
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
            list.Interrupt();
            isInterrupted = true;
            // continue the interruption
            base.OnInterrupt();
        }

        public override void OnEnd()
        {
            if (!isInterrupted)
            {
                list.Interrupt();
            }
            CommandManager.RemoveBatch(list);
        }

    }
}
