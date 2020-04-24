using UnityEngine;

namespace Bloodthirst.Core.SceneManager {
    public interface ISceneInstanceManager {
        bool IsConfigScene { get; set; }
        int SceneIndex { get; set; }
        void QuerySceneGameObjects();
        void MoveToScene(GameObject gameObject);
        void SetActive(bool value);
        void Show();
        void Hide();
    }
}
