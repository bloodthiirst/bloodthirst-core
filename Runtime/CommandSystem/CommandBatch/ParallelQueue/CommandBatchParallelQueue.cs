using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    /// <summary>
    /// TODO : rework using the already implemented queues
    /// </summary>
    public class CommandBatchParallelQueue : ICommandBatch
    {
        public event Action<ICommandBatch> OnBatchEnded;
        public event Action<ICommandBatch, ICommandBase> OnCommandRemoved;
        public event Action<ICommandBatch, ICommandBase> OnCommandAdded;

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

        private CommandBatchList waitables;

        private CommandBatchList interrptables;

        private CommandBatchList nonSync;

        private bool isStarted;

        public CommandBatchParallelQueue()
        {
            LayersList = new List<ParallelLayer>();

            waitables = CommandManagerBehaviour.AppendBatch<CommandBatchList>(this);
            interrptables = CommandManagerBehaviour.AppendBatch<CommandBatchList>(this);
            nonSync = CommandManagerBehaviour.AppendBatch<CommandBatchList>(this);

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
            if (isStarted)
            {
                return;
            }

            IncrementLayer();
            isStarted = true;
        }

        private void IncrementLayer()
        {

            if (layersList.Count == 0)
            {
                return;
            }

            ParallelLayer current = layersList[0];

            foreach (CommandSettings w in current.WaitableCommmands)
            {
                waitables.Append(w);

                w.Command.OnCommandEnd -= OnWaitableEnded;
                w.Command.OnCommandEnd += OnWaitableEnded;

                OnCommandAdded?.Invoke(this, w.Command);
            }

            foreach (CommandSettings i in current.InterruptableCommmands)
            {
                interrptables.Append(i);
                OnCommandAdded?.Invoke(this, i.Command);
            }

            foreach (CommandSettings n in current.NonSyncedCommands)
            {
                nonSync.Append(n);
                OnCommandAdded?.Invoke(this, n.Command);
            }
        }

        /// <summary>
        /// After every waitable check if we need to apply the parallel logic
        /// </summary>
        /// <param name="cmd"></param>
        private void OnWaitableEnded(ICommandBase cmd)
        {
            cmd.OnCommandEnd -= OnWaitableEnded;

            ParallelLayer current = layersList[0];

            // if one of the commands is not done (success or interruption or failed) then we exit
            for (int i = current.WaitableCommmands.Count - 1; i > -1 ; i--)
            {
                CommandSettings commandBase = current.WaitableCommmands[i];

                if (commandBase.Command.GetExcutingCommand().IsDone)
                {
                    OnCommandRemoved?.Invoke(this, commandBase.Command);
                }
                else
                {
                    return;
                }
            }

            // interrupt interruptables
            interrptables.Interrupt();

            // remove layer
            layersList.RemoveAt(0);

            IncrementLayer();
        }

        public void Interrupt()
        {
            BatchState = BATCH_STATE.INTERRUPTED;

            InterruptSubcommands();
        }

        private void InterruptSubcommands()
        {

            for (int i = 0; i < layersList.Count; i++)
            {
                // interruptable

                for (int j = 0; j < layersList[i].InterruptableCommmands.Count; j++)
                {
                    CommandSettings commandBase = layersList[i].InterruptableCommmands[j];
                    commandBase.Command.GetExcutingCommand().Interrupt();

                    OnCommandRemoved?.Invoke(this, commandBase.Command);
                }

                // waitable

                for (int j = 0; j < layersList[i].WaitableCommmands.Count; j++)
                {
                    CommandSettings commandBase = layersList[i].WaitableCommmands[j];
                    commandBase.Command.GetExcutingCommand().Interrupt();

                    OnCommandRemoved?.Invoke(this, commandBase.Command);
                }
            }

            layersList.Clear();
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
            return removeWhenDone && layersList.Count == 0;
        }
    }
}
