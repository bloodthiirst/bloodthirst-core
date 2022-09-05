using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public class GlobalSceneManager : MonoBehaviour
    {

        public event Action<ISceneInstanceManager> OnAfterSceneLoaded;

        public event Action<ISceneInstanceManager> OnBeforeSceneUnloaded;

        [ShowInInspector]
        private List<ISceneInstanceManager> currentActiveScene;

        private CommandManagerBehaviour commandManagerBehaviour;

        private LoadingManager loadingManager;

        public void Initialize(CommandManagerBehaviour commandManagerBehaviour, LoadingManager loadingManager)
        {
            currentActiveScene = new List<ISceneInstanceManager>();
            this.commandManagerBehaviour = commandManagerBehaviour;
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
