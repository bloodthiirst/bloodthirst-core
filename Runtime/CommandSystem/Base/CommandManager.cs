using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManager
    {

        [ShowInInspector]
        private List<List<ICommandBatch>> commandBatches;

        public CommandManager()
        {
            commandBatches = new List<List<ICommandBatch>>();
        }


        /// <summary>
        /// Append a batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="removeWhenDone"></param>
        /// <returns></returns>
        public T AppendBatch<T>(object owner, bool removeWhenDone = false , int updateOrder = 0) where T : ICommandBatch , new()
        {
            T batch = new T();
            batch.RemoveWhenDone = removeWhenDone;
            batch.Owner = owner;
            batch.BatchUpdateOrder = updateOrder;

            int layersDiff = updateOrder - (commandBatches.Count - 1);

            for(int i = 0; i < layersDiff; i++)
            {
                commandBatches.Add(new List<ICommandBatch>());
            }

            commandBatches[updateOrder].Add(batch);

            return batch;
        }

        public void Tick(float deltaTime)
        {
            for (int l = 0; l < commandBatches.Count; l++)
            {
                for (int i = commandBatches[l].Count - 1; i > -1; i--)
                {
                    // remove if necessary
                    if (commandBatches[l][i].ShouldRemove())
                    {
                        commandBatches[l][i].End();
                        commandBatches[l].RemoveAt(i);
                        continue;

                    }
                    // else tick
                    commandBatches[l][i].Tick(deltaTime);
                }
            }
        }


    }
}
