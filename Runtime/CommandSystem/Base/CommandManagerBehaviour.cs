using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManagerBehaviour : UnitySingleton<CommandManagerBehaviour>
    {

        [ShowInInspector]
        private CommandManager commandManager;

        [SerializeField]
        private bool isActive = true;

        public bool IsActive { get => isActive; set => isActive = value; }

        private CommandBatchList globalCommandBatch;

        protected override void Awake()
        {
            base.Awake();
            commandManager = new CommandManager();
            globalCommandBatch = AppendBatch<CommandBatchList>(this);

        }

        /// <summary>
        /// Run the command and run it globally
        /// </summary>
        /// <param name="command"></param>
        public static void Run(ICommandBase command)
        {
            Instance.globalCommandBatch.Append(command, false);
        }

        /// <summary>
        /// Append a batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="removeWhenDone"></param>
        /// <returns></returns>
        public static T AppendBatch<T>(object owner, bool removeWhenDone = false) where T : ICommandBatch, new()
        {
            return Instance.commandManager.AppendBatch<T>(owner, removeWhenDone);
        }

        private void Update()
        {
            if (!isActive)
                return;

            commandManager.Tick(Time.deltaTime);
        }


    }
}
