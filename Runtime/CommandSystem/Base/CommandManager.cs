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

        private CommandBatchList globalCommandBatch;

        protected override void Awake()
        {
            base.Awake();
            commandBatches = new List<ICommandBatch>();
            globalCommandBatch = AppendBatch<CommandBatchList>(this);

        }

        /// <summary>
        /// Run the command and run it globally
        /// </summary>
        /// <param name="command"></param>
        public static void Run(ICommandBase command)
        {
            Instance.globalCommandBatch.Append(command);
        }

        /// <summary>
        /// Append a batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="removeWhenDone"></param>
        /// <returns></returns>
        public static T AppendBatch<T>(object owner , bool removeWhenDone = false) where T : ICommandBatch, new()
        {
            T batch = new T();
            batch.RemoveWhenDone = removeWhenDone;
            batch.Owner = owner;

            Instance.commandBatches.Add(batch);

            return batch;
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
