using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Bloodthirst.Core.Setup.GameStart;

namespace Bloodthirst.Core.SceneManager
{
    public class SceneStartup : MonoBehaviour , IPreGameSetup , IGameSetup , IPostGameSetup
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

        void IPreGameSetup.Execute()
        {
            BProviderRuntime.Instance.RegisterSingleton(this);
        }


        IAsynOperationWrapper IGameSetup.GetAsynOperations()
        {
            LoadMultipleScenesAsyncOperation op = new LoadMultipleScenesAsyncOperation();

            for (int i = 0; i < ScenesListData.Instance.ScenesList.Count; i++)
            {
                string scenePath = ScenesListData.Instance.ScenesList[i];
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);
                if (!scene.isLoaded)
                {
                    op.Add(scenePath);
                }
            }

            return op;
        }

        void IPostGameSetup.Execute()
        {
            List<GameObject> allGos = GameObjectUtils.GetAllRootGameObjects();
            // scenes
            GameObjectUtils.GetAllComponents<IBeforeAllScenesInitializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<ISceneInitializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostSceneInitializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAfterAllScenesIntializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<ISceneInitializationPass>(allGos, true).ForEach(e => e.Execute());

            // objs
            GameObjectUtils.GetAllComponents<IBeforeAllScenesInitializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<ISceneInitializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostSceneInitializationPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAfterAllScenesIntializationPass>(allGos, true).ForEach(e => e.Execute()); ;

            GameObjectUtils.GetAllComponents<ISetupSingletonPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IQuerySingletonPass>(allGos, true).ForEach(e => e.Execute()); ;
            GameObjectUtils.GetAllComponents<IInjectPass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAwakePass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IEnablePass>(allGos, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostEnablePass>(allGos, true).ForEach(e => e.Execute());
        }
    }

}