using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandBatchQueue : ICommandBatch
    {
        [SerializeField]
        private Queue<ICommandBase> commandList;
        public Queue<ICommandBase> CommandsList { get => commandList; set => commandList = value; }

        [SerializeField]
        private bool removeWhenDone;
        public bool RemoveWhenDone { get => removeWhenDone; set => removeWhenDone = value; }

        [SerializeField]
        private BATCH_STATE batchState;
        public BATCH_STATE BatchState { get => batchState; set => batchState = value; }

        [SerializeField]
        private object owner;
        public object Owner { get => owner; set => owner = value; }

        public CommandBatchQueue()
        {
            CommandsList = new Queue<ICommandBase>();
            BatchState = BATCH_STATE.EXECUTING;
        }
        public void Tick(float delta)
        {
            if (commandList.Count == 0)
            {
                BatchState = BATCH_STATE.DONE;
                return;
            }
            ICommandBase current = commandList.Peek().GetExcutingCommand();
            // if command is done , dequeue
            if (current.IsDone)
            {
                // if the command is failed then the entire queue is failed
                if (current.CommandState == COMMAND_STATE.FAILED)
                {
                    Interrupt();
                    return;
                }
                commandList.Dequeue();
            }

            if (commandList.Count == 0)
            {
                BatchState = BATCH_STATE.DONE;
                return;
            }

            BatchState = BATCH_STATE.EXECUTING;

            // Note : we recall the GetExecutingCommand since there's a chance the previous once was dequeued
            current = commandList.Peek().GetExcutingCommand();

            // if command is not started , execute the command start
            if (!current.IsStarted)
            {
                current.Start();
            }

            // execute the commands on tick
            current.OnTick(delta);
        }

        public ICommandBatch Append(ICommandBase command)
        {
            CommandsList.Enqueue(command);
            return this;
        }

        public void Interrupt()
        {
            foreach (ICommandBase command in commandList)
            {
                // Note : the interrupt of ICommandBase is expected to handle it's internal commands if it has any
                // example : interrupting sub commands
                command.GetExcutingCommand().Interrupt();
            }
            BatchState = BATCH_STATE.DONE;
        }
    }
}
