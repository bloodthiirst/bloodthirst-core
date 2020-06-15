using System.Collections.Generic;
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

        public void AddLayer(ParallelLayer parallelLayer)
        {
            commandList.Add(parallelLayer);
        }

        public ParallelLayer AppendLayer()
        {
            ParallelLayer layer = new ParallelLayer();

            commandList.Add(layer);

            return layer;
        }

        public void Tick(float delta)
        {
            if (commandList.Count == 0)
                return;

            ParallelLayer current = commandList[0];

            // if all waitable commands are done , forcebly end the other interruptable commands

            if (current.IsDone)
            {
                // clear waitables

                for (int i = 0; i < current.WaitableCommmands.Count; i++)
                {
                    ICommandBase waitable = current.WaitableCommmands[i];

                    waitable.OnEnd();
                }

                current.WaitableCommmands.Clear();

                // clear interruptables

                for (int i = 0; i < current.InterruptableCommmands.Count; i++)
                {
                    ICommandBase interruptable = current.InterruptableCommmands[i];

                    interruptable.Interrupt();
                }

                current.InterruptableCommmands.Clear();

                // see if there is a generated layer

                ParallelLayer next = current.GenerateNext();

                // if yes then put it on top

                if (next != null)
                {
                    commandList[0] = next;
                }

                // else remove the top layer since its done

                else
                {
                    commandList.RemoveAt(0);
                }
            }

            if (commandList.Count == 0)
                return;

            current = commandList[0];

            // if command is not started , execute the command start

            if (!current.IsStarted)
            {
                current.Start();

                for (int i = 0; i < current.WaitableCommmands.Count; i++)
                {
                    current.WaitableCommmands[i].GetExcutingCommand().OnStart();
                    current.WaitableCommmands[i].GetExcutingCommand().IsStarted = true;
                    current.WaitableCommmands[i].OnCommandStartNotify();

                }

                for (int i = 0; i < current.InterruptableCommmands.Count; i++)
                {
                    current.InterruptableCommmands[i].GetExcutingCommand().OnStart();
                    current.InterruptableCommmands[i].GetExcutingCommand().IsStarted = true;
                    current.InterruptableCommmands[i].OnCommandStartNotify();
                }
            }

            // execute the commands on tick

            for (int i = 0; i < current.InterruptableCommmands.Count; i++)
            {
                current.InterruptableCommmands[i].GetExcutingCommand().OnTick(delta);
            }

            for (int i = 0; i < current.WaitableCommmands.Count; i++)
            {
                if (!current.WaitableCommmands[i].IsDone)
                {
                    current.WaitableCommmands[i].GetExcutingCommand().OnTick(delta);
                }
            }
        }

        public void Interrupt()
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                // interruptable

                for (int j = 0; j < commandList[i].InterruptableCommmands.Count; j++)
                {
                    commandList[i].InterruptableCommmands[j].Interrupt();
                }

                // waitable

                for (int j = 0; j < commandList[i].WaitableCommmands.Count; j++)
                {
                    commandList[i].WaitableCommmands[j].Interrupt();
                }
            }
        }
    }
}
