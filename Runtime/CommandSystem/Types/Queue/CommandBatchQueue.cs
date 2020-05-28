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
        private object owner;
        public object Owner { get => owner; set => owner = value; }
        public CommandBatchQueue()
        {
            CommandsList = new Queue<ICommandBase>();
        }
        public void Tick(float delta)
        {
            if (commandList.Count == 0)
                return;

            // if command is done , delete it and do increment index
            if (commandList.Peek().GetExcutingCommand().IsDone)
            {
                commandList.Dequeue().OnEnd();
            }

            if (commandList.Count == 0)
                return;

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
                command.Interrupt();
            }
        }
    }
}
