using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class LoadSingleSceneAsyncWrapper : IAsynOperationWrapper
    {
        private readonly string scenePath;
        private readonly GlobalSceneManager globalSceneManager;
        private readonly bool triggerSceneCallbacks;

        public LoadSingleSceneAsyncWrapper(string scenePath, bool triggerSceneCallbacks, GlobalSceneManager globalSceneManager)
        {
            this.scenePath = scenePath;
            this.triggerSceneCallbacks = triggerSceneCallbacks;
            this.globalSceneManager = globalSceneManager;
        }

        IProgressCommand IAsynOperationWrapper.CreateOperation()
        {
            return new LoadSingleSceneAsyncOperation(scenePath, triggerSceneCallbacks , globalSceneManager);
        }
    }

    public class LoadSingleSceneAsyncOperation : CommandBase<LoadSingleSceneAsyncOperation> , IProgressCommand
    {
        private readonly string scenePath;
        private readonly GlobalSceneManager globalSceneManager;
        private readonly bool triggerSceneCallbacks;
        private AsyncOperation op;

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

        string IProgressCommand.TaskName => $"Loading Scene {scenePath}";

        public LoadSingleSceneAsyncOperation(string scene, bool triggerSceneCallbacks, GlobalSceneManager globalSceneManager)
        {
            this.scenePath = scene;
            this.triggerSceneCallbacks = triggerSceneCallbacks;
            this.globalSceneManager = globalSceneManager;
        }

        public override void OnStart()
        {
            op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            op.completed += OnSceneLoadComplete;
        }

        public override void OnTick(float delta)
        {
            CurrentProgress = op.progress;
        }

        private void OnSceneLoadComplete(AsyncOperation op)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);

            List<GameObject> sceneGOs = ListPool<GameObject>.Get();
            scene.GetRootGameObjects(sceneGOs);

            if (triggerSceneCallbacks)
            {
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
            }

            ISceneInstanceManager sceneInstanceManager = GameObjectUtils.GetAllComponents<ISceneInstanceManager>(sceneGOs, true).FirstOrDefault();

            Assert.IsNotNull(sceneInstanceManager);

            globalSceneManager.RegisterScene(sceneInstanceManager);

            op.completed -= OnSceneLoadComplete;

            ListPool<GameObject>.Release(sceneGOs);

            CurrentProgress = 1;

            Success();
        }
    }
}
