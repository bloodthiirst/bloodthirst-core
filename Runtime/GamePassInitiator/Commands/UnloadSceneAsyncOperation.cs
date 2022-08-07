using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class UnloadSceneAsyncWrapper : IAsynOperationWrapper
    {
        private string _scenePath;

        public UnloadSceneAsyncWrapper(string scenePath)
        {
            _scenePath = scenePath;
        }

        IProgressCommand IAsynOperationWrapper.CreateOperation()
        {
            return new UnloadSceneAsyncOperation(_scenePath);
        }
    }
    public class UnloadSceneAsyncOperation : CommandBase<UnloadSceneAsyncOperation>, IProgressCommand
    {
        private string _scenePath;

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
        string IProgressCommand.TaskName => $"Unloading Scene {_scenePath}";

        public UnloadSceneAsyncOperation(string scenePath)
        {
            _scenePath = scenePath;
        }

        public override void OnStart()
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(_scenePath);

            List<GameObject> gos = scene.GetRootGameObjects().ToList();

            GameObjectUtils.GetAllComponents<IBeforeSceneUnload>(gos, true).ForEach(e => e.Execute());

            op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_scenePath);

            op.completed += OnSceneUnloadComplete;
        }

        public override void OnTick(float delta)
        {
            CurrentProgress = op.progress;
        }

        public void OnSceneUnloadComplete(AsyncOperation op)
        {

            Debug.Log($"Scene {_scenePath} has been unloaded");

            op.completed -= OnSceneUnloadComplete;


            CurrentProgress = 1;

            Success();
        }
    }
}
