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
    public class LoadMultipleScenesAsyncWrapper : IAsynOperationWrapper
    {
        private readonly IList<string> scenes;
        private readonly GlobalSceneManager globalSceneManager;
        private readonly bool triggerCallbacks;

        public LoadMultipleScenesAsyncWrapper(IList<string> scenes, GlobalSceneManager globalSceneManager, bool triggerCallbacks)
        {
            this.scenes = scenes;
            this.globalSceneManager = globalSceneManager;
            this.triggerCallbacks = triggerCallbacks;
        }

        IProgressCommand IAsynOperationWrapper.CreateOperation()
        {
            return new LoadMultipleScenesAsyncOperation(scenes, globalSceneManager, triggerCallbacks);
        }
    }



    public class LoadMultipleScenesAsyncOperation : TreeCommandBase<LoadMultipleScenesAsyncOperation>, IProgressCommand
    {
        private readonly IList<string> scenes;
        private readonly GlobalSceneManager globalSceneManager;
        private readonly bool triggerCallbacks;
        private List<LoadSingleSceneAsyncOperation> subCommands;
        public LoadMultipleScenesAsyncOperation(IList<string> scenes, GlobalSceneManager globalSceneManager, bool triggerCallbacks) : base()
        {
            this.scenes = scenes;
            this.globalSceneManager = globalSceneManager;
            this.triggerCallbacks = triggerCallbacks;
            subCommands = new List<LoadSingleSceneAsyncOperation>();
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
            for (int i = 0; i < scenes.Count; i++)
            {
                string scene = scenes[i];

                LoadSingleSceneAsyncOperation cmd = new LoadSingleSceneAsyncOperation(scene, false, globalSceneManager);

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
                LoadSingleSceneAsyncOperation op = subCommands[i];

                curr += op.CurrentProgress;
            }

            CurrentProgress = curr / total;


            if (currentProgress != 1)
                return;

            // scenes are done loading

            for (int i = 0; i < total; i++)
            {
                LoadSingleSceneAsyncOperation op = subCommands[i];

                op.OnCurrentProgressChanged -= Cmd_OnCurrentProgressChanged;
            }

            if (triggerCallbacks)
            {
                List<GameObject> sceneGOs = ListPool<GameObject>.Get();
                List<GameObject> cache = ListPool<GameObject>.Get();

                for (int i = 0; i < scenes.Count; i++)
                {
                    string scenePath = scenes[i];

                    UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);

                    // note : this does a clear already so no need to do it ourselves
                    scene.GetRootGameObjects(cache);
                    sceneGOs.AddRange(cache);

                }

                // trigger scene callbacks
                GameObjectUtils.GetAllComponents<IBeforeAllScenesInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<ISceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<IPostSceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<IAfterAllScenesIntializationPass>(sceneGOs, true).ForEach(e => e.Execute()); ;

                GameObjectUtils.GetAllComponents<ISetupSingletonPass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<IQuerySingletonPass>(sceneGOs, true).ForEach(e => e.Execute()); ;
                GameObjectUtils.GetAllComponents<IInjectPass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<IAwakePass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<IEnablePass>(sceneGOs, true).ForEach(e => e.Execute());
                GameObjectUtils.GetAllComponents<IPostEnablePass>(sceneGOs, true).ForEach(e => e.Execute());


                ListPool<GameObject>.Release(cache);
                ListPool<GameObject>.Release(sceneGOs);
            }

            subCommands.Clear();

            Success();
        }
    }
}
