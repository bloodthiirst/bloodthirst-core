using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManager
    {

        [ShowInInspector]
        private List<ICommandBatch> commandBatches;

        [SerializeField]
        private bool isActive = true;

        public bool IsActive { get => isActive; set => isActive = value; }

        private CommandBatchList globalCommandBatch;

        public CommandManager()
        {
            commandBatches = new List<ICommandBatch>();
            globalCommandBatch = AppendBatch<CommandBatchList>(this);
        }


        /// <summary>
        /// Append a batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="removeWhenDone"></param>
        /// <returns></returns>
        public T AppendBatch<T>(object owner, bool removeWhenDone = false) where T : ICommandBatch, new()
        {
            T batch = new T();
            batch.RemoveWhenDone = removeWhenDone;
            batch.Owner = owner;

            commandBatches.Add(batch);

            return batch;
        }

        public void Tick(float deltaTime)
        {
            for (int i = commandBatches.Count - 1; i > -1; i--)
            {
                commandBatches[i].Tick(Time.deltaTime);

                if (commandBatches[i].ShouldRemove())
                {
                    if (commandBatches[i].BatchState == BATCH_STATE.INTERRUPTED)
                    {
                        commandBatches.RemoveAt(i);
                    }
                    else
                    {
                        commandBatches.RemoveAt(i);
                        commandBatches[i].Interrupt();
                        commandBatches[i].BatchState = BATCH_STATE.DONE;
                    }

                }
            }
        }


    }
}
