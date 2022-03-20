using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Singleton;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManagerBehaviour : MonoBehaviour, IPreGameSetup
    {
        [ShowInInspector]
        [ShowIf("@UnityEngine.Application.isPlaying")]
        private CommandManager commandManager;

        [ReadOnly]
        [ShowInInspector]
        private bool isActive = false;

        public bool IsActive { get => isActive; set => isActive = value; }

        private CommandBatchList globalCommandBatch;

        int IPreGameSetup.Order => 0;
        void IPreGameSetup.Execute()
        {
            
            commandManager = new CommandManager();
            globalCommandBatch = AppendBatch<CommandBatchList>(this);
            isActive = true;

            BProviderRuntime.Instance.RegisterSingleton<CommandManagerBehaviour , CommandManagerBehaviour>(this);
        }

        /// <summary>
        /// Run the command and run it globally
        /// </summary>
        /// <param name="command"></param>
        public void Run(ICommandBase command)
        {
            globalCommandBatch.Append(command, false);
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
            return commandManager.AppendBatch<T>(owner, removeWhenDone);
        }

        private void Update()
        {
            if (!isActive)
                return;

            commandManager.Tick(Time.deltaTime);
        }
    }
}
