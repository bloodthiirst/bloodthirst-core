#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
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
        public event Action<ICommandBase, ICommandBase> OnCommandRemoved;
        public event Action<ICommandBase, ICommandBase> OnCommandAdded;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private Queue<CommandSettings> commandQueue;
        private readonly bool propagateFailOrInterrupt;

        public QueueCommandBase(bool propagateFailOrInterrupt = false) : base()
        {
            this.propagateFailOrInterrupt = propagateFailOrInterrupt;
            commandQueue = new Queue<CommandSettings>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T Enqueue(ICommandBase command,  bool removeOnDone = true)
        {
            command.RemoveWhenDone = removeOnDone;
            commandQueue.Enqueue(new CommandSettings() { Command = command });

            OnCommandAdded?.Invoke(this , command);

            return (T)this;
        }

        public override void OnTick(float delta)
        {
            while (commandQueue.Count != 0)
            {
                CommandSettings cmd = commandQueue.Peek();

                ICommandBase currCmd = cmd.Command.GetExcutingCommand();

                // if command is not started , execute the command start
                if (currCmd.CommandState == COMMAND_STATE.WATING)
                {
                    currCmd.Start();
                }

                // if command is done , dequeue
                if (currCmd.IsDone())
                {
                    if (currCmd.CommandState == COMMAND_STATE.INTERRUPTED && propagateFailOrInterrupt)
                    {
                        Interrupt();
                        return;
                    }

                    if (currCmd.CommandState == COMMAND_STATE.FAILED && propagateFailOrInterrupt)
                    {
                        Fail();
                        return;
                    }

                    commandQueue.Dequeue();
                    OnCommandRemoved?.Invoke(this, cmd.Command);

                    continue;
                }

                currCmd.OnTick(delta);
                return;
            }

            if (RemoveWhenDone)
            {
                Success();
            }
        }

        public override void OnEnd()
        {
            if (CommandState == COMMAND_STATE.INTERRUPTED)
            {
                while (commandQueue.Count != 0)
                {
                    CommandSettings cmd = commandQueue.Dequeue();

                    cmd.Command.GetExcutingCommand().Interrupt();

                    OnCommandRemoved?.Invoke(this, cmd.Command);
                }
            }
        }

    }
}
