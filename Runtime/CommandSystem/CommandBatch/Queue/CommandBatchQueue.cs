using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandBatchQueue : ICommandBatch
    {
        public event Action<ICommandBatch> OnBatchEnded;
        public event Action<ICommandBatch, ICommandBase> OnCommandRemoved;
        public event Action<ICommandBatch, ICommandBase> OnCommandAdded;

        [SerializeField]
        private Queue<CommandSettings> commandQueue;
        public Queue<CommandSettings> CommandsQueue { get => commandQueue; set => commandQueue = value; }

        [SerializeField]
        private bool removeWhenDone;
        public bool RemoveWhenDone { get => removeWhenDone; set => removeWhenDone = value; }

        [SerializeField]
        private BATCH_STATE batchState;
        public BATCH_STATE BatchState
        {
            get => batchState;
            set
            {
                batchState = value;
            }
        }

        [SerializeField]
        private object owner;

        public object Owner { get => owner; set => owner = value; }
        public int BatchUpdateOrder { get; set; }

        public CommandBatchQueue()
        {
            CommandsQueue = new Queue<CommandSettings>();
            BatchState = BATCH_STATE.EXECUTING;
        }
        public void Tick(float delta)
        {
            if (commandQueue.Count == 0)
            {
                return;
            }

            CommandSettings cmd = commandQueue.Peek();

            // if command is done , dequeue
            if (cmd.Command.IsDone)
            {
                // if the command is FAILED and InterruptOnFail == true
                // then the entire queue is failed
                // else we just dequeue and continue
                if (cmd.Command.CommandState == COMMAND_STATE.FAILED && cmd.InterruptBatchOnFail)
                {
                    Interrupt();
                    return;
                }
                // if SUCCESS or INTERRUPTED
                commandQueue.Dequeue();
                OnCommandRemoved?.Invoke(this, cmd.Command);
            }

            if (commandQueue.Count == 0)
            {
                return;
            }

            // Note : we recall the GetExecutingCommand since there's a chance the previous once was dequeued
            cmd = commandQueue.Peek();

            // if command is not started , execute the command start
            if (!cmd.Command.IsStarted)
            {
                cmd.Command.Start();
            }

            // todo : check if it's done in start before going to the tick
            if(cmd.Command.IsDone)
            {
                return;
            }

            // execute the commands on tick
            cmd.Command.OnTick(delta);
        }

        public CommandBatchQueue Append(ICommandBase command, bool interruptBatchOnFail = false)
        {
            return Append(new CommandSettings() { Command = command, InterruptBatchOnFail = interruptBatchOnFail });
        }

        internal CommandBatchQueue Append(CommandSettings commandSettings)
        {
            CommandsQueue.Enqueue(commandSettings);
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
            while (commandQueue.Count != 0)
            {
                CommandSettings command = commandQueue.Dequeue();
                // Note : the interrupt of ICommandBase is expected to handle it's internal commands if it has any
                // example : interrupting sub commands
                command.Command.GetExcutingCommand().Interrupt();

                OnCommandRemoved?.Invoke(this, command.Command);
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
            return removeWhenDone && commandQueue.Count == 0;
        }
    }
}
