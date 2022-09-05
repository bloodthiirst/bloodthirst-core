using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class UnloadSingleSceneAsyncWrapper : IAsynOperationWrapper
    {
        private ISceneInstanceManager sceneInstanceManager;
        private readonly bool triggerCallbacks;
        private readonly GlobalSceneManager globalSceneManager;

        public UnloadSingleSceneAsyncWrapper(ISceneInstanceManager sceneInstanceManager, GlobalSceneManager globalSceneManager, bool triggerCallbacks)
        {
            this.sceneInstanceManager = sceneInstanceManager;
            this.triggerCallbacks = triggerCallbacks;
            this.globalSceneManager = globalSceneManager;
        }

        IProgressCommand IAsynOperationWrapper.CreateOperation()
        {
            return new UnloadSingleSceneAsyncOperation(sceneInstanceManager, globalSceneManager , triggerCallbacks);
        }
    }

    public class UnloadSingleSceneAsyncOperation : CommandBase<UnloadSingleSceneAsyncOperation>, IProgressCommand
    {
        private readonly ISceneInstanceManager sceneInstanceManager;
        private readonly GlobalSceneManager globalSceneManager;
        private readonly bool triggerCallbacks;
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
        string IProgressCommand.TaskName => $"Unloading Scene {sceneInstanceManager.ScenePath}";

        public UnloadSingleSceneAsyncOperation(ISceneInstanceManager sceneInstanceManager, GlobalSceneManager globalSceneManager, bool triggerCallbacks)
        {
            this.sceneInstanceManager = sceneInstanceManager;
            this.globalSceneManager = globalSceneManager;
            this.triggerCallbacks = triggerCallbacks;
        }

        public override void OnStart()
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(sceneInstanceManager.ScenePath);

            if (triggerCallbacks)
            {
                List<GameObject> sceneGOs = ListPool<GameObject>.Get();
                scene.GetRootGameObjects(sceneGOs);

                GameObjectUtils.GetAllComponents<IBeforeSceneUnload>(sceneGOs, true).ForEach(e => e.Execute());

                ListPool<GameObject>.Release(sceneGOs);
            }

            globalSceneManager.UnregisterScene(sceneInstanceManager);

            op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneInstanceManager.ScenePath);

            op.completed += OnSceneUnloadComplete;
        }

        public override void OnTick(float delta)
        {
            CurrentProgress = op.progress;
        }

        public void OnSceneUnloadComplete(AsyncOperation op)
        {
            Debug.Log($"Scene {sceneInstanceManager.ScenePath} has been unloaded");

            op.completed -= OnSceneUnloadComplete;

            CurrentProgress = 1;

            Success();
        }
    }
}
