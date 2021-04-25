using Bloodthirst.Core.Setup;
using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Singleton;
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
    public class SceneLoadingManager : MonoBehaviour , IPreGameSetup , IGameSetup , IPostGameSetup
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
        public UnityEvent beforeAllScenesLoaded;

        [SerializeField]
        public UnityEvent afterAllScenesLoaded;

        int IGameSetup.Order => 0;
        
        void IPreGameSetup.Execute()
        {
            beforeAllScenesLoaded?.Invoke();
            BProviderRuntime.Instance.RegisterSingleton(this);
        }

        IEnumerable<AsyncOperation> IGameSetup.Operations()
        {
            for (int i = 0; i < ScenesListData.Instance.ScenesList.Count; i++)
            {
                if (!UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(i).isLoaded)
                    yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
            }
        }

        void IPostGameSetup.Execute()
        {
            afterAllScenesLoaded?.Invoke();
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

    }

}