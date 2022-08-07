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
    public class LoadSingleSceneAsyncOperation : CommandBase<LoadSingleSceneAsyncOperation> , IProgressCommand
    {
        private string scenePath;
        
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

        public LoadSingleSceneAsyncOperation(string scene)
        {
            this.scenePath = scene;
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
            op.completed -= OnSceneLoadComplete;

            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);

            List<GameObject> sceneGOs = scene.GetRootGameObjects().ToList();

            GameObjectUtils.GetAllComponents<IBeforeAllScenesInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<ISceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostSceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAfterAllScenesIntializationPass>(sceneGOs, true).ForEach(e => e.Execute()); ;

            GameObjectUtils.GetAllComponents<IQuerySingletonPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IQuerySingletonPass>(sceneGOs, true).ForEach(e => e.Execute()); ;
            GameObjectUtils.GetAllComponents<IInjectPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAwakePass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IEnablePass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostEnablePass>(sceneGOs, true).ForEach(e => e.Execute());

            CurrentProgress = 1;

            Success();
        }
    }
}
