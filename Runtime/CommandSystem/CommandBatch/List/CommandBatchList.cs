using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandBatchList : ICommandBatch
    {
        public event Action<ICommandBatch> OnBatchEnded;
        public event Action<ICommandBatch, ICommandBase> OnCommandRemoved;
        public event Action<ICommandBatch, ICommandBase> OnCommandAdded;

        [SerializeField]
        private List<CommandSettings> commandList;
        public List<CommandSettings> CommandsList { get => commandList; set => commandList = value; }

        [SerializeField]
        private object owner;
        public object Owner { get => owner; set => owner = value; }

        [SerializeField]
        private bool removeWhenDone;
        public bool RemoveWhenDone { get => removeWhenDone; set => removeWhenDone = value; }

        [SerializeField]
        private BATCH_STATE batchState;
        public BATCH_STATE BatchState { get => batchState; set => batchState = value; }

        public CommandBatchList()
        {
            CommandsList = new List<CommandSettings>();
            BatchState = BATCH_STATE.EXECUTING;
        }
        public void Tick(float delta)
        {

            // register the removable commands

            for (int i = CommandsList.Count - 1; i > -1; i--)
            {
                CommandSettings cmd = CommandsList[i];

                if (cmd.Command.GetExcutingCommand().IsDone)
                {
                    CommandsList.RemoveAt(i);
                    OnCommandRemoved?.Invoke(this, cmd.Command);

                    if(cmd.Command.CommandState == COMMAND_STATE.FAILED && cmd.InterruptBatchOnFail)
                    {
                        Interrupt();
                        return;
                    }

                    continue;
                }

                // if command is not started , execute the command start
                if (!cmd.Command.GetExcutingCommand().IsStarted)
                {
                    cmd.Command.GetExcutingCommand().Start();
                }

                // execute the commands on tick
                cmd.Command.GetExcutingCommand().OnTick(delta);
            }

        }

        public CommandBatchList Append(ICommandBase command , bool shouldInterrupt = false)
        {
            CommandSettings cmd = new CommandSettings() { Command = command, InterruptBatchOnFail = shouldInterrupt };
            return Append(cmd);
        }

        internal CommandBatchList Append(CommandSettings commandSettings)
        {
            CommandsList.Add(commandSettings);
            OnCommandAdded?.Invoke(this, commandSettings.Command);
            return this;
        }

        public void Interrupt()
        {
            BatchState = BATCH_STATE.INTERRUPTED;

            InterruptSubcommands();
        }

        private void InterruptSubcommands()
        {
            for (int i = commandList.Count - 1; i > -1; i--)
            {
                CommandSettings cmd = commandList[i];
                cmd.Command.GetExcutingCommand().Interrupt();
                commandList.RemoveAt(i);
                OnCommandRemoved?.Invoke(this, cmd.Command);
            }
        }

        public void End()
        {
            if (BatchState != BATCH_STATE.INTERRUPTED)
            {
                InterruptSubcommands();
                BatchState = BATCH_STATE.DONE;
            }

            OnBatchEnded?.Invoke(this);
        }

        public bool ShouldRemove()
        {
            return removeWhenDone && commandList.Count == 0;
        }
    }
}
