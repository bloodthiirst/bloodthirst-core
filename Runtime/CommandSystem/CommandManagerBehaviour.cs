using Bloodthirst.Core.Updater;
using Sirenix.OdinInspector;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{

    public class CommandManagerBehaviour : MonoBehaviour , IUpdatable , ICommandManagerProvider
    {
        [ShowInInspector]
        [ShowIf("@UnityEngine.Application.isPlaying")]
        public CommandManager commandManager;

        [ReadOnly]
        [ShowInInspector]
        private bool isActive = false;

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                enabled = isActive;
            }
        }


        private BasicListCommand globalCommandBatch;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void OnDomainReload()
        {
            if (EditorApplication.isPlaying)
            {
                GameObject.FindObjectOfType<CommandManagerBehaviour>().Initialize();
            }
        }
#endif

        public void Initialize()
        {
            commandManager = new CommandManager();
            globalCommandBatch = new BasicListCommand(false);
            AppendCommand(this, globalCommandBatch, false);
            isActive = true;
        }

        private void Reset()
        {
            Initialize();
        }

        /// <summary>
        /// Run the command and run it globally
        /// </summary>
        /// <param name="command"></param>
        public void Run(ICommandBase command)
        {
            globalCommandBatch.Add(command, false );
        }

        /// <summary>
        /// Append a batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="removeWhenDone"></param>
        /// <returns></returns>
        public void AppendCommand(object owner, ICommandBase cmd, bool removeWhenDone = false)
        {
            commandManager.AppendCommand(owner, cmd, removeWhenDone);
        }

        void IUpdatable.OnTick(float deltaTime)
        {
            commandManager.Tick(deltaTime);
        }

        void ICommandManagerProvider.Initialize()
        {
            Initialize();
        }

        CommandManager ICommandManagerProvider.Get()
        {
            return commandManager;
        }
    }
}
