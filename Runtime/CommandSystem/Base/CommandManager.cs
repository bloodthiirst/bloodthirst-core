using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManager : UnitySingleton<CommandManager>
    {
        [ShowInInspector]
        private List<ICommandBatch> commandBatches;

        protected override void Awake()
        {
            base.Awake();
            commandBatches = new List<ICommandBatch>();
        }
        public static ICommandBatch AppendBatch<T>(object owner) where T : ICommandBatch, new()
        {
            T batch = new T();
            batch.Owner = owner;

            Instance.commandBatches.Add(batch);

            return batch;
        }

        public static void RemoveBatch<T>(T batch) where T : ICommandBatch
        {
            if (batch == null)
                return;

            Instance.commandBatches.Remove(batch);
        }

        private void Update()
        {
            for(int i = 0; i < commandBatches.Count; i++)
            {
                commandBatches[i].Tick(Time.deltaTime);
            }
        }


    }
}
