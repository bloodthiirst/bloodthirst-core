using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(SceneStartup))]
    [RequireComponent(typeof(SceneStartup))]
    public class SceneStartupAdapter : MonoBehaviour, IPreGameSetup, IGameSetup, IPostGameSetup
    {
        int IPreGameSetup.Order => 0;

        void IPreGameSetup.Execute()
        {
            SceneStartup bhv = GetComponent<SceneStartup>();
            BProviderRuntime.Instance.RegisterSingleton(bhv);
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
    }
}
