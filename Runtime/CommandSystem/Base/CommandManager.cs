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

        public CommandManager()
        {
            commandBatches = new List<ICommandBatch>();
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
                // remove if necessary
                if (commandBatches[i].ShouldRemove())
                {
                    commandBatches[i].End();
                    commandBatches.RemoveAt(i);
                    continue;

                }
                // else tick
                commandBatches[i].Tick(deltaTime);
            }
        }


    }
}
