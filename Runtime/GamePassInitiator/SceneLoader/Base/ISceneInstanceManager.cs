using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public interface ISceneInstanceManager
    {
        bool IsScenePlaying { get; }
        bool IsSceneVisible { get; }
        bool IsConfigScene { get; set; }
        Scene Scene{ get; }
        int SceneIndex { get; set; }
        string ScenePath { get; set; }

        void Initialize(LoadingManager loadingManager);
        void AddToScene(GameObject gameObject);
        void RemoveFromScene(GameObject gameObject);
        void Disable();
        void Enable();
        void Show();
        void Hide();
    }
}
