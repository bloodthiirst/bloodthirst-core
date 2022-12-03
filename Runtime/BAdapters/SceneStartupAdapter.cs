using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(SceneStartup))]
    [RequireComponent(typeof(SceneStartup))]
    public class SceneStartupAdapter : MonoBehaviour, IPreGameSetup, IGameSetup, IPostGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;
        int IPreGameSetup.Order => preGameSetupOrder;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [InfoBox("$" + nameof(DuplicateMessage), nameof(HasDuplicate), InfoMessageType = InfoMessageType.Error)]
        [ValueDropdown(nameof(GetScenesList), FlattenTreeView = true)]
#endif
        [SerializeField]
        private string[] startupScenesList;
#if UNITY_EDITOR
        private string DuplicateMessage()
        {
            if (TryFindDuplicate(out string scenePath))
            {
                return $"There's a duplicate of the scene {scenePath} in the list , make sure to setup your list correctly";
            }

            return string.Empty;
        }

        private bool HasDuplicate()
        {
            return TryFindDuplicate(out _);
        }

        private bool TryFindDuplicate(out string scenePath)
        {
            for (int i = 0; i < startupScenesList.Length; i++)
            {
                string s = startupScenesList[i];

                if (startupScenesList.Count(scene => scene == s) != 1)
                {
                    scenePath = s;
                    return true;
                }
            }

            scenePath = string.Empty;
            return false;
        }
        private static IEnumerable<string> GetScenesList()
        {
            return ScenesListData.Instance.ScenesList;
        }
#endif
        void IPreGameSetup.Execute()
        {
            SceneStartup bhv = GetComponent<SceneStartup>();
            BProviderRuntime.Instance.RegisterSingleton(bhv);
        }

        IEnumerable<IAsynOperationWrapper> IGameSetup.GetAsynOperations()
        {
            List<string> scenePaths = new List<string>();

            for (int i = 0; i < startupScenesList.Length; i++)
            {
                string scenePath = ScenesListData.Instance.ScenesList[i];
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if (!scene.isLoaded)
                {
                    scenePaths.Add(scenePath);
                }
            }

            GlobalSceneManager globalSceneManager = BProviderRuntime.Instance.GetSingleton<GlobalSceneManager>();

            LoadMultipleScenesAsyncWrapper op = new LoadMultipleScenesAsyncWrapper(scenePaths, globalSceneManager, false);

            yield return op;
        }

        void IPostGameSetup.Execute()
        {
            List<Scene> scenes = ListPool<Scene>.Get();
            List<GameObject> sceneGOs = ListPool<GameObject>.Get();
            List<GameObject> cache = ListPool<GameObject>.Get();

            scenes.Capacity = SceneManager.sceneCount;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                scenes.Add(SceneManager.GetSceneAt(i));
            }

            // trigger scene callbacks
            for (int i = 0; i < scenes.Count; i++)
            {
                string scenePath = scenes[i].path;

                Scene scene = scenes[i];

                // note : this does a clear already so no need to do it ourselves
                scene.GetRootGameObjects(cache);
                sceneGOs.AddRange(cache);
            }

            GameObjectUtils.GetAllComponents<IBeforeAllScenesInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<ISceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostSceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAfterAllScenesIntializationPass>(sceneGOs, true).ForEach(e => e.Execute()); ;

            GameObjectUtils.GetAllComponents<ISetupSingletonPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IQuerySingletonPass>(sceneGOs, true).ForEach(e => e.Execute()); ;
            GameObjectUtils.GetAllComponents<IInjectPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAwakePass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IEnablePass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostEnablePass>(sceneGOs, true).ForEach(e => e.Execute());

            ListPool<GameObject>.Release(cache);
            ListPool<GameObject>.Release(sceneGOs);
            ListPool<Scene>.Release(scenes);
        }
    }
}
