using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Core.Setup
{
    public class UnloadMultipleScenesAsyncWrapper : IAsynOperationWrapper
    {
        private readonly IList<ISceneInstanceManager> scenes;
        private readonly GlobalSceneManager globalSceneManager;
        
        public UnloadMultipleScenesAsyncWrapper(IList<ISceneInstanceManager> scenes, GlobalSceneManager globalSceneManager)
        {
            this.scenes = scenes;
            this.globalSceneManager = globalSceneManager;
        }

        IProgressCommand IAsynOperationWrapper.CreateOperation()
        {
            return new UnloadMultipleScenesAsyncOperation(scenes , globalSceneManager);
        }
    }

    public class UnloadMultipleScenesAsyncOperation : TreeCommandBase<LoadMultipleScenesAsyncOperation> , IProgressCommand
    {
        private readonly IList<ISceneInstanceManager> scenes;
        private readonly GlobalSceneManager globalSceneManager;
        private List<UnloadSingleSceneAsyncOperation> subCommands;
        public UnloadMultipleScenesAsyncOperation(IList<ISceneInstanceManager> scenes, GlobalSceneManager globalSceneManager) : base()
        {
            this.scenes = scenes;
            this.globalSceneManager = globalSceneManager;

            subCommands = new List<UnloadSingleSceneAsyncOperation>();
        }

        private float currentProgress;
        public float CurrentProgress
        {
            get => currentProgress;
            private set
            {
                if (currentProgress == value)
                    return;

                float old = currentProgress;
                currentProgress = value;

                OnCurrentProgressChanged?.Invoke(this, old, currentProgress);
            }
        }

        public event Action<IProgressCommand, float, float> OnCurrentProgressChanged;
        string IProgressCommand.TaskName => $"Loading {scenes.Count} scenes";
        public override void OnStart()
        {
            List<GameObject> sceneGOs = ListPool<GameObject>.Get();
            List<GameObject> cache = ListPool<GameObject>.Get();

            // trigger scene callbacks
            for (int i = 0; i < scenes.Count; i++)
            {
                ISceneInstanceManager sceneInstanceManager = scenes[i];

                UnityEngine.SceneManagement.Scene scene = sceneInstanceManager.Scene;

                // note : this does a clear already so no need to do it ourselves
                scene.GetRootGameObjects(cache);
                sceneGOs.AddRange(cache);
            }

            GameObjectUtils.GetAllComponents<IBeforeSceneUnload>(sceneGOs, true).ForEach(e => e.Execute());

            ListPool<GameObject>.Release(cache);
            ListPool<GameObject>.Release(sceneGOs);

            for (int i = 0; i < scenes.Count; i++)
            {
                ISceneInstanceManager scene = scenes[i];

                UnloadSingleSceneAsyncOperation cmd = new UnloadSingleSceneAsyncOperation(scene  , globalSceneManager , false);

                cmd.OnCurrentProgressChanged += Cmd_OnCurrentProgressChanged;

                subCommands.Add(cmd);

                Append(cmd);
            }

            base.OnStart();
        }

        private void Cmd_OnCurrentProgressChanged(IProgressCommand arg1, float arg2, float arg3)
        {
            Refresh();
        }

        private void Refresh()
        {
            float curr = 0;
            float total = subCommands.Count;

            for (int i = 0; i < total; i++)
            {
                UnloadSingleSceneAsyncOperation op = subCommands[i];

                curr += op.CurrentProgress;
            }

            CurrentProgress = curr / total;


            if (currentProgress != 1)
                return;

            for (int i = 0; i < total; i++)
            {
                UnloadSingleSceneAsyncOperation op = subCommands[i];

                op.OnCurrentProgressChanged -= Cmd_OnCurrentProgressChanged;
            }

            subCommands.Clear();

            Success();
        }
    }
}
