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
            // if command is done , dequeue and execute on end
            if (commandList.Peek().GetExcutingCommand().IsDone)
            {
                commandList.Dequeue().OnEnd();
            }

            if (commandList.Count == 0)
            {
                BatchState = BATCH_STATE.DONE;
                return;
            }

            BatchState = BATCH_STATE.EXECUTING;

            // if command is not started , execute the command start
            if (!commandList.Peek().IsStarted)
            {
                commandList.Peek().GetExcutingCommand().OnStart();
                commandList.Peek().GetExcutingCommand().IsStarted = true;
                commandList.Peek().OnCommandStartNotify();
            }

            // execute the commands on tick
            CommandsList.Peek().GetExcutingCommand().OnTick(delta);
        }

        public ICommandBatch Append(ICommandBase command)
        {
            CommandsList.Enqueue(command);
            return this;
        }

        public void Interrupt()
        {
            foreach(ICommandBase command in commandList)
            {
                if (command.CommandState == COMMAND_STATE.EXECUTING)
                {
                    command.Interrupt();
                }
            }

            BatchState = BATCH_STATE.DONE;
        }
    }
}
