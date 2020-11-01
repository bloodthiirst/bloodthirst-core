using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public abstract class SceneInstanceManager<T> : UnitySingleton<T>, ISceneInstanceManager where T : SceneInstanceManager<T>
    {

        [SerializeField]
        private bool isConfigScene;

        public bool IsConfigScene { get => isConfigScene; set => isConfigScene = value; }

        public int sceneIndex;

        public int SceneIndex { get => sceneIndex; set => sceneIndex = value; }

        public Scene Scene
        {
            get
            {
                return UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);
            }
        }

        public List<GameObject> sceneGameObjects;

        public void QuerySceneGameObjects()
        {
            if (sceneGameObjects == null)
                sceneGameObjects = new List<GameObject>();
            else
                sceneGameObjects.Clear();

            Debug.Log(Scene.name + Scene.buildIndex);

            Scene.GetRootGameObjects(sceneGameObjects);
        }

        public void MoveToScene(GameObject gameObject)
        {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, Scene);
        }


        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // register the manager to the list of managers
            if(SceneLoadingManager.Instance == null)
            {
                Debug.LogError("SceneLoadingManager is null");
            }

            SceneLoadingManager.Instance.SceneInstanceManagers.Add(this);

            QuerySceneGameObjects();
        }

        public void OnDestroy()
        {
            SceneLoadingManager.Instance?.SceneInstanceManagers.Remove(this);
        }

        public void SetActive(bool value)
        {
            foreach (GameObject go in sceneGameObjects)
            {
                go.SetActive(value);
            }
        }

        public void Show()
        {
            SetActive(true);
        }

        public void Hide()
        {
            SetActive(false);
        }
    }
}
