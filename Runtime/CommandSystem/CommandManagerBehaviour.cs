using Bloodthirst.Core.Updater;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public sealed class CommandManagerBehaviour : MonoBehaviour , IUpdatable , ICommandManagerProvider
    {
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

#if ODIN_INSPECTOR
        [ShowIf("@UnityEngine.Application.isPlaying")]
#endif
        public CommandManager commandManager;

        
#if ODIN_INSPECTOR[ReadOnly]
#endif
        
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

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
                GameObject.FindFirstObjectByType<CommandManagerBehaviour>().Initialize();
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
