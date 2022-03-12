using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public interface ISceneInstanceManager
    {
        bool IsScenePlaying { get; }
        bool IsSceneVisible { get; }
        bool IsConfigScene { get; set; }
        int SceneIndex { get; set; }
        string ScenePath{ get; set; }
        void QuerySceneGameObjects();
        void AddToScene(GameObject gameObject);
        void RemoveFromScene(GameObject gameObject);
        void Disable();
        void Enable();
        void Show();
        void Hide();
    }
}
