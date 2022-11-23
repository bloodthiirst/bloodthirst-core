using System;
using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public abstract class ListCommandBase<T> : CommandBase<T> where T : ListCommandBase<T>
    {
        public event Action<ICommandBase, ICommandBase> OnCommandRemoved;
        public event Action<ICommandBase, ICommandBase> OnCommandAdded;

        private List<CommandSettings> commandsList;
        private bool propagateFailOrInterrupt;

        public ListCommandBase(bool propagateFailOrInterrupt) : base()
        {
            this.propagateFailOrInterrupt = propagateFailOrInterrupt;
            commandsList = new List<CommandSettings>();
        }

        /// <summary>
        /// Add commands externally to execute in the CommandQueue
        /// , note : these commands are executed BEFORE the QueueCommands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public T Add(ICommandBase command, bool removeOnDone)
        {
            CommandSettings commandSettings = new CommandSettings() { Command = command };
            return Add(commandSettings , removeOnDone);
        }

        internal T Add(CommandSettings commandSettings, bool removeOnDone)
        {
            commandSettings.Command.RemoveWhenDone = removeOnDone;
            commandsList.Add(commandSettings);
            OnCommandAdded?.Invoke(this, commandSettings.Command);
            return (T)this;
        }

        public override void OnTick(float delta)
        {
            for (int i = commandsList.Count - 1; i > -1; --i)
            {
                CommandSettings cmd = commandsList[i];

                ICommandBase currCmd = cmd.Command.GetExcutingCommand();

                if (currCmd.CommandState == COMMAND_STATE.WATING)
                {
                    currCmd.Start();
                }

                if (currCmd.IsDone())
                {
                    if (currCmd.RemoveWhenDone)
                    {
                        commandsList.RemoveAt(i);
                        OnCommandRemoved?.Invoke(this, currCmd);
                    }

                    if (currCmd.CommandState == COMMAND_STATE.INTERRUPTED && propagateFailOrInterrupt)
                    {
                        Interrupt();
                        return;
                    }

                    if(currCmd.CommandState == COMMAND_STATE.FAILED && propagateFailOrInterrupt)
                    {
                        Fail();
                        return;
                    }

                    continue;
                }

                // execute the commands on tick
                currCmd.OnTick(delta);
            }


            if (commandsList.Count == 0 && RemoveWhenDone)
            {
                Success();
                return;
            }
        }

        public override void OnEnd()
        {
            if (CommandState == COMMAND_STATE.INTERRUPTED)
            {
                for (int i = commandsList.Count - 1; i >= 0; --i)
                {
                    CommandSettings cmd = commandsList[i];

                    cmd.Command.GetExcutingCommand().Interrupt();

                    commandsList.RemoveAt(i);

                    OnCommandRemoved?.Invoke(this, cmd.Command);
                }
            }
        }

    }
}
