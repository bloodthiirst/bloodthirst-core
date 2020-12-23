using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandBatchParallelQueue : ICommandBatch
    {
        [SerializeField]
        private List<ParallelLayer> layersList;
        public List<ParallelLayer> LayersList { get => layersList; set => layersList = value; }

        [SerializeField]
        private object owner;
        public object Owner { get => owner; set => owner = value; }

        [SerializeField]
        private bool removeWhenDone;
        public bool RemoveWhenDone { get => removeWhenDone; set => removeWhenDone = value; }

        [SerializeField]
        private BATCH_STATE batchState;
        public BATCH_STATE BatchState { get => batchState; set => batchState = value; }

        public CommandBatchParallelQueue()
        {
            LayersList = new List<ParallelLayer>();
            BatchState = BATCH_STATE.EXECUTING;
        }

        public void AddLayer(ParallelLayer parallelLayer)
        {
            layersList.Add(parallelLayer);
        }

        public ParallelLayer AppendLayer()
        {
            ParallelLayer layer = new ParallelLayer();

            layersList.Add(layer);

            return layer;
        }

        public void Tick(float delta)
        {
            if (layersList.Count == 0) {
                BatchState = BATCH_STATE.DONE;
                return;
            }

            ParallelLayer current = layersList[0];

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
                    layersList[0] = next;
                }

                // else remove the top layer since its done

                else
                {
                    layersList.RemoveAt(0);
                }
            }

            // 
            if (layersList.Count == 0)
            {
                BatchState = BATCH_STATE.DONE;
                return;
            }

            current = layersList[0];

            // if command is not started , execute the command start

            if (!current.IsStarted)
            {
                current.Start();

                for (int i = 0; i < current.WaitableCommmands.Count; i++)
                {
                    current.WaitableCommmands[i].GetExcutingCommand().Start();

                }

                for (int i = 0; i < current.InterruptableCommmands.Count; i++)
                {
                    current.InterruptableCommmands[i].GetExcutingCommand().Start();
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
            for (int i = 0; i < layersList.Count; i++)
            {
                // interruptable

                for (int j = 0; j < layersList[i].InterruptableCommmands.Count; j++)
                {
                    if (layersList[i].WaitableCommmands[j].CommandState == COMMAND_STATE.EXECUTING)
                    {
                        layersList[i].InterruptableCommmands[j].Interrupt();
                    }
                }

                // waitable

                for (int j = 0; j < layersList[i].WaitableCommmands.Count; j++)
                {
                    if (layersList[i].WaitableCommmands[j].CommandState == COMMAND_STATE.EXECUTING)
                    {
                        layersList[i].WaitableCommmands[j].Interrupt();
                    }
                }
            }

            BatchState = BATCH_STATE.DONE;
        }
    }
}
