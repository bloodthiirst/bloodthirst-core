using Bloodthirst.System.CommandSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public class GlobalSceneManager : MonoBehaviour
    {

        public event Action<ISceneInstanceManager> OnAfterSceneLoaded;

        public event Action<ISceneInstanceManager> OnBeforeSceneUnloaded;

        [ShowInInspector]
        private List<ISceneInstanceManager> currentActiveScene;

        private ICommandManagerProvider commandManagerProvider;

        private LoadingManager loadingManager;

        public void Initialize(ICommandManagerProvider commandManagerProvider, LoadingManager loadingManager)
        {
            currentActiveScene = new List<ISceneInstanceManager>();
            this.commandManagerProvider = commandManagerProvider;
            this.loadingManager = loadingManager;
        }

        public void RegisterScene(ISceneInstanceManager sceneInstanceManager)
        {
            currentActiveScene.Add(sceneInstanceManager);
            OnAfterSceneLoaded?.Invoke(sceneInstanceManager);
        }

        public void UnregisterScene(ISceneInstanceManager sceneInstanceManager)
        {
            if (currentActiveScene.Remove(sceneInstanceManager))
            {
                OnBeforeSceneUnloaded?.Invoke(sceneInstanceManager);
            }
        }
    }
}
