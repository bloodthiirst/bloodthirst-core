using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManager : UnitySingleton<CommandManager>
    {

        [ShowInInspector]
        private List<ICommandBatch> commandBatches;

        [SerializeField]
        private bool isActive = true;

        public bool IsActive { get => isActive; set => isActive = value; }

        protected override void Awake()
        {
            base.Awake();
            commandBatches = new List<ICommandBatch>();
        }
        public static T AppendBatch<T>(object owner , bool removeWhenDone = false) where T : ICommandBatch, new()
        {
            T batch = new T();
            batch.RemoveWhenDone = removeWhenDone;
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
            if (!isActive)
                return;

            for(int i = commandBatches.Count - 1; i > -1 ; i--)
            {
                commandBatches[i].Tick(Time.deltaTime);

                if(commandBatches[i].BatchState == BATCH_STATE.DONE && commandBatches[i].RemoveWhenDone)
                {
                    commandBatches.RemoveAt(i);
                }
            }
        }


    }
}
