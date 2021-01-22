using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.SceneManager
{
    public class SceneLoadingManager : UnitySingleton<SceneLoadingManager>
#if UNITY_EDITOR
        , IPreprocessBuildWithReport
#endif
    {
#if UNITY_EDITOR
        public int callbackOrder => 100;

        public void OnPreprocessBuild(BuildReport report)
        {
            ScenesListData.Instance.LoadAllScenesAvailable();
        }
#endif

        public List<AsyncOperation> asyncOperations;

        public List<ISceneInstanceManager> SceneInstanceManagers
        {
            get
            {
                if (sceneInstanceManagers == null)
                {
                    sceneInstanceManagers = new List<ISceneInstanceManager>();
                }

                return sceneInstanceManagers;
            }
        }

        [ShowInInspector]
        private List<ISceneInstanceManager> sceneInstanceManagers;

        [SerializeField]
        public UnityEvent BeforeAllScenesLoaded;

        [SerializeField]
        public UnityEvent AfterAllScenesLoaded;

        [SerializeField]
        private bool loadInStart;

        private IEnumerator Start()
        {
            yield return CrtLoadScenes();
        }

        private IEnumerator CrtLoadScenes()
        {
            // start scene loading
            LoadScene();

            BeforeAllScenesLoaded?.Invoke();

            // wait until all loading done
            yield return new WaitUntil(() => asyncOperations.TrueForAll(op => op.isDone));

            AfterAllScenesLoaded?.Invoke();

            yield return null;

        }

        public void HideAllScenes()
        {
            foreach (ISceneInstanceManager sceneManager in SceneInstanceManagers)
            {
                if (sceneManager.IsConfigScene)
                    continue;

                sceneManager.Hide();
            }
        }

        void LoadScene()
        {
            if (asyncOperations == null)
                asyncOperations = new List<AsyncOperation>();
            else
                asyncOperations.Clear();

            for (int i = 0; i < ScenesListData.Instance.ScenesList.Count; i++)
            {

                if (!UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(i).isLoaded)
                    asyncOperations.Add(UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive));
            }
        }
    }

}