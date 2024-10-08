using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.BProvider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bloodthirst.Core.Utils;
using UnityEngine.Pool;

namespace Bloodthirst.Core.Setup
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField]
        private bool runInBackground;

        [SerializeField]
        private bool executeOnStart;

        [SerializeField]
        private int targetFramerate;

        public bool ExecuteOnStart
        {
            get => executeOnStart;
            set => executeOnStart = value;
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            Application.runInBackground = runInBackground;

            if (executeOnStart)
            {
                yield return Setup();
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFramerate;
        }

        public IEnumerator Setup()
        {
            // query
            using (ListPool<GameObject>.Get(out List<GameObject> allGos))
            using (ListPool<IPreGameSetup>.Get(out List<IPreGameSetup> preSetups))
            using (ListPool<IGameSetup>.Get(out List<IGameSetup> gameSetups))
            using (ListPool<IAsynOperationWrapper>.Get(out List<IAsynOperationWrapper> asyncOps))
            using (ListPool<IPostGameSetup>.Get(out List<IPostGameSetup> postSetups))
            {
                GameObjectUtils.GetAllRootGameObjects(allGos);

                // pre
                {
                    GameObjectUtils.GetAllComponents<IPreGameSetup>(ref preSetups, allGos, true);
                    preSetups.Sort((a, b) => a.Order.CompareTo(b.Order));

                    foreach (IPreGameSetup p in preSetups)
                    {
                        p.Execute();
                    }
                }

                // setup
                {
                    LoadingManager manager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

                    GameObjectUtils.GetAllComponents(ref gameSetups, true);

                    foreach (IGameSetup g in gameSetups)
                    {
                        asyncOps.AddRange(g.GetAsynOperations());
                    }

                    foreach (IAsynOperationWrapper op in asyncOps)
                    {
                        manager.RunAsyncTask(op);
                    }

                    yield return new WaitWhile(() => manager.State == LOADDING_STATE.LOADING);
                }

                // post
                {
                    allGos.Clear();
                    GameObjectUtils.GetAllRootGameObjects(allGos);

                    GameObjectUtils.GetAllComponents<IPostGameSetup>(ref postSetups ,allGos, true);

                    foreach (IPostGameSetup p in postSetups)
                    {
                        p.Execute();
                    }
                }
            }
        }
    }
}