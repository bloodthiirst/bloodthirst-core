using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bloodthirst.Core.SceneManager
{
    public abstract class SceneInstanceManager<T> : UnitySingleton<T>, ISceneInstanceManager where T : SceneInstanceManager<T>
    {
        public event Action<T> OnSceneVisibilityChanged;
        public event Action<T> OnSceneStateChanged;

        [SerializeField]
        private bool isConfigScene;

        public bool IsConfigScene { get => isConfigScene; set => isConfigScene = value; }

        public int sceneIndex;

        public int SceneIndex { get => sceneIndex; set => sceneIndex = value; }

        [SerializeField]
        private bool isScenePlaying = true;
        public bool IsScenePlaying { get => isScenePlaying; }

        [SerializeField]
        private bool sceneVisible = true;
        public bool IsSceneVisible { get => sceneVisible; }
        public Scene Scene
        {
            get
            {
                return UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);
            }
        }



        [ShowInInspector]
        /// <summary>
        /// List of root gameObjects in the scene (Doesn't include child objects)
        /// </summary>
        protected List<GameObject> sceneGameObjects = new List<GameObject>();

        [ShowInInspector]
        /// <summary>
        /// Renderable UIs related to the scene
        /// </summary>
        protected List<Graphic> sceneUis = new List<Graphic>();

        [ShowInInspector]
        /// <summary>
        /// Renderable Objects other than UI related to the scene
        /// </summary>
        protected List<Renderer> sceneRenderers = new List<Renderer>();


        private List<GameObject> previouslyActiveGO = new List<GameObject>();

        private List<Graphic> previouslyActiveUIs = new List<Graphic>();

        private List<Renderer> previouslyActiveRenderers = new List<Renderer>();

        public void QuerySceneGameObjects()
        {
            if (sceneGameObjects == null)
                sceneGameObjects = new List<GameObject>();
            else
                sceneGameObjects.Clear();

            Debug.Log(Scene.name + Scene.buildIndex);

            List<GameObject> tmpSceneGameObjects = new List<GameObject>();
            Scene.GetRootGameObjects(tmpSceneGameObjects);

            // fetch renderable elements elements
            for (int i = 0; i < tmpSceneGameObjects.Count; i++)
            {
                GameObject current = tmpSceneGameObjects[i];
                AddToScene(current);
            }
        }

        public void AddToScene(GameObject gameObject)
        {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, Scene);

            // add to the appropriate lists
            sceneGameObjects.Add(gameObject);
            if(gameObject.activeSelf && !sceneVisible)
            {
                gameObject.SetActive(false);
                previouslyActiveGO.Add(gameObject);
            }

            Renderer[] rend = gameObject.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rend.Length; i++)
            {
                Renderer r = rend[i];
                sceneRenderers.Add(r);

                // if scene isn't active then hide the uis as well
                if(r.enabled && !sceneVisible)
                {
                    r.enabled = false;
                    previouslyActiveRenderers.Add(r);
                }
            }
            Graphic[] uis = gameObject.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < uis.Length; i++)
            {
                Graphic g = uis[i];
                sceneUis.Add(g);

                // if scene isn't active then hide the renderer as well
                if (g.enabled && !sceneVisible)
                {
                    g.enabled = false;
                    previouslyActiveUIs.Add(g);
                }
            }

        }

        /// <summary>
        /// Unregister and object from a scene
        /// </summary>
        /// <param name="gameObject"></param>
        public void RemoveFromScene(GameObject gameObject)
        {
            // remove from the appropriate lists
            sceneGameObjects.Remove(gameObject);
            previouslyActiveGO.Remove(gameObject);

            Renderer[] rend = gameObject.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rend.Length; i++)
            {
                Renderer r = rend[i];
                sceneRenderers.Remove(r);
                previouslyActiveRenderers.Remove(r);
            }
            Graphic[] uis = gameObject.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < uis.Length; i++)
            {
                Graphic g = uis[i];
                sceneUis.Remove(g);
                previouslyActiveUIs.Remove(g);
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // register the manager to the list of managers
            if (SceneLoadingManager.Instance == null)
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

        [Button]
        [InfoBox("disables all the gameObjects under the scene")]
        public void Pause()
        {
            if (!isScenePlaying)
                return;

            isScenePlaying = false;

            Hide();

            for (int i = 0; i < sceneGameObjects.Count; i++)
            {
                GameObject go = sceneGameObjects[i];
                if (go.activeSelf)
                {
                    previouslyActiveGO.Add(go);
                    go.SetActive(false);
                }
            }

            OnSceneStateChanged?.Invoke((T)this);
        }

        [Button]
        [InfoBox("enables all the gameObjects under the scene")]
        public void Play()
        {
            if (isScenePlaying)
                return;
            
            isScenePlaying = true;

            Show();

            for (int i = 0; i < previouslyActiveGO.Count; i++)
            {
                GameObject go = sceneGameObjects[i];
                go.SetActive(true);
            }

            previouslyActiveGO.Clear();

            OnSceneStateChanged?.Invoke((T)this);
        }

        [Button]
        [InfoBox("shows all the visual elements of the scene that were previously hidden")]
        public void Show()
        {
            if (sceneVisible)
                return;

            sceneVisible = true;

            // reactivate the cached renderers before showing
            for (int i = 0; i < previouslyActiveRenderers.Count; i++)
            {
                Renderer current = sceneRenderers[i];
                current.enabled = true;
            }
            previouslyActiveRenderers.Clear();

            // reactivate the cached uis before showing
            for (int i = 0; i < previouslyActiveUIs.Count; i++)
            {
                Graphic current = sceneUis[i];
                current.enabled = true;
            }
            previouslyActiveUIs.Clear();

            // trigger event

            OnSceneVisibilityChanged?.Invoke((T) this);
        }

        [Button]
        [InfoBox("Hides all the visual elements of the scene \nNOTE : the objects are still active , just not visible")]
        public void Hide()
        {
            if (!sceneVisible)
                return;

            sceneVisible = false;

            // cache the active visible renderers before hiding
            for (int i = 0; i < sceneRenderers.Count; i++)
            {
                Renderer current = sceneRenderers[i];
                if (current.enabled)
                {
                    previouslyActiveRenderers.Add(current);
                    current.enabled = false;
                }
            }

            // cache the active visible uis before hiding
            for (int i = 0; i < sceneUis.Count; i++)
            {
                Graphic current = sceneUis[i];
                if (current.enabled)
                {
                    previouslyActiveUIs.Add(current);
                    current.enabled = false;
                }
            }

            // trigger event

            OnSceneVisibilityChanged?.Invoke((T)this);
        }
    }
}
