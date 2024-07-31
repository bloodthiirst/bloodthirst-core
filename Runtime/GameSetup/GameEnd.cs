using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.BProvider;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bloodthirst.Core.Utils;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine.Pool;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bloodthirst.Core.Setup
{
    public class GameEnd : MonoBehaviour, IPreGameEnd, IPostGameEnd
    {
        int IPreGameEnd.Order => 0;

        void IPreGameEnd.Execute()
        {
            Debug.Log("[IPreGameEnd] has been called , this is a IPreGameEnd");
        }

        void IPostGameEnd.Execute()
        {
            Debug.Log("[IPostGameEnd] Game has unloaded successfully");
        }

        private IEnumerator End()
        {
            using (ListPool<GameObject>.Get(out List<GameObject> allGOs))
            using (ListPool<IPreGameEnd>.Get(out List<IPreGameEnd> preGameEnd))
            using (ListPool<IGameEnd>.Get(out List<IGameEnd> gameEnds))
            using (ListPool<IAsynOperationWrapper>.Get(out List<IAsynOperationWrapper> asyncOps))
            using (ListPool<IPostGameEnd>.Get(out List<IPostGameEnd> postGameEnd))
            {
                // query
                GameObjectUtils.GetAllRootGameObjects(allGOs);

                // pre
                {
                    GameObjectUtils.GetAllComponents<IPreGameEnd>(ref preGameEnd, allGOs, true);
                    preGameEnd.Sort((a, b) => a.Order.CompareTo(b.Order));
                    foreach (IPreGameEnd e in preGameEnd)
                    {
                        e.Execute();
                    }
                }

                LoadingManager manager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

                // setup
                {
                    GameObjectUtils.GetAllComponents(ref gameEnds, true);

                    gameEnds.Sort((a, b) => a.Order.CompareTo(b.Order));

                    foreach (IGameEnd e in gameEnds)
                    {
                        IAsynOperationWrapper op = e.GetAsyncOperations();

                        if (op == null)
                            continue;

                        asyncOps.Add(op);
                    }

                    foreach (IAsynOperationWrapper op in asyncOps)
                    {
                        manager.RunAsyncTask(op);
                    }
                }

                yield return new WaitWhile(() => manager.State == LOADDING_STATE.LOADING);

                // post
                {
                    // refetch because IGameEnd will destroy most of the game scenes
                    allGOs.Clear();
                    GameObjectUtils.GetAllRootGameObjects(allGOs);

                    // post
                    GameObjectUtils.GetAllComponents<IPostGameEnd>(ref postGameEnd, allGOs, true);

                    foreach (IPostGameEnd e in postGameEnd)
                    {
                        e.Execute();
                    }

                }
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#else
            Application.Quit(0);
#endif
            }
        }

#if ODIN_INSPECTOR
        [Button(ButtonSizes.Large)]
        [DisableInEditorMode]
#endif
        public void Quit()
        {
            StartCoroutine(End());
        }


    }
}