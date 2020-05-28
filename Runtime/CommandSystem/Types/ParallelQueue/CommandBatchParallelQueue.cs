using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandBatchParallelQueue : ICommandBatch
    {
        [SerializeField]
        private List<ParallelLayer> commandList;
        public List<ParallelLayer> CommandsList { get => commandList; set => commandList = value; }

        [SerializeField]
        private object owner;
        public object Owner { get => owner; set => owner = value; }
        public CommandBatchParallelQueue()
        {
            CommandsList = new List<ParallelLayer>();
        }
        public void Tick(float delta)
        {
            if (commandList.Count == 0)
                return;

            ParallelLayer current = commandList[0];

            // if command is done , forcebly end the other interruptable commands

            if (current.IsDone)
            {
                for(int i = 0; i < current.InterruptableCommmands.Count; i++)
                {
                    current.InterruptableCommmands[i].Interrupt();
                }

                commandList.RemoveAt(0);

                ParallelLayer next = current.GenerateNext();

                if (next == null)
                {
                    commandList.Insert(0 ,next);
                }
            }

            if (commandList.Count == 0)
                return;

            current = commandList[0];

            // if command is not started , execute the command start

            for (int i = 0; i < current.CommandBasesPerLayer.Count; i++)
            {

                if (!current.CommandBasesPerLayer[i].IsStarted)
                {
                    current.CommandBasesPerLayer[i].GetExcutingCommand().OnStart();
                    current.CommandBasesPerLayer[i].GetExcutingCommand().IsStarted = true;
                    current.CommandBasesPerLayer[i].OnCommandStartNotify();
                }

                // execute the commands on tick

                current.CommandBasesPerLayer[i].GetExcutingCommand().OnTick(delta);
            }
        }

        public void Interrupt()
        {
            for(int i = 0; i < commandList.Count; i++)
            {
                for (int j = 0; j < commandList[i].InterruptableCommmands.Count; j++)
                {
                    commandList[i].InterruptableCommmands[j].Interrupt();
                }
            }
        }
    }
}
