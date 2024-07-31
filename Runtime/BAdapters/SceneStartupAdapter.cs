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
using UnityEngine.Assertions;
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
                string scenePath = startupScenesList[i];
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if (!scene.isLoaded)
                {
                    scenePaths.Add(scenePath);
                }
            }
            
            LoadMultipleScenesAsyncWrapper op = new LoadMultipleScenesAsyncWrapper(scenePaths, false , true);

            yield return op;
        }

        void IPostGameSetup.Execute()
        {
            using (ListPool<Scene>.Get(out List<Scene> scenes))
            using (ListPool<GameObject>.Get(out List<GameObject> sceneGOs))
            using (ListPool<IOnSceneLoaded>.Get(out List<IOnSceneLoaded> loaded))
            using (ListPool<ISceneInstanceManager>.Get(out List<ISceneInstanceManager> managers))
            {
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

                    scene.GetRootGameObjects(sceneGOs);

                    GameObjectUtils.GetAllComponents(ref loaded, sceneGOs, true);
                    GameObjectUtils.GetAllComponents(ref managers, sceneGOs, true);

                    Assert.IsTrue(managers.Count == 1);

                    ISceneInstanceManager sceneManager = managers[0];

                    foreach (IOnSceneLoaded e in loaded)
                    {
                        e.OnLoaded(sceneManager);
                    }
                }

            }
        }
    }
}
