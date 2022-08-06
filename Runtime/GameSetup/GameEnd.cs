using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.BProvider;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bloodthirst.Core.Setup
{
    public class GameEnd : MonoBehaviour , IPreGameEnd  , IPostGameEnd
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

        public IEnumerator End()
        {
            // query
            List<GameObject> allGos = GameObjectUtils.GetAllRootGameObjects();

            // pre
            GameObjectUtils.GetAllComponents<IPreGameEnd>(allGos, true).OrderBy(p => p.Order).ToList().ForEach((e) => e.Execute());

            // setup
            LoadingManager manager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

            List<IGameEnd> gameEnds = new List<IGameEnd>();

            GameObjectUtils.GetAllComponents(ref gameEnds, true);

            IEnumerable<IAsynOperationWrapper> asyncOps = gameEnds.OrderBy( p => p.Order ).Select(g => g.GetAsyncOperations()).Where( g => g != null);

            foreach (IAsynOperationWrapper op in asyncOps)
            {
                manager.Load(op);
            }
            yield return new WaitWhile(() => manager.State == LOADDING_STATE.LOADING);

            // refetch because IGameEnd will destroy most of the game scenes
            allGos = GameObjectUtils.GetAllRootGameObjects();

            // post
            GameObjectUtils.GetAllComponents<IPostGameEnd>(allGos, true).ForEach((e) => e.Execute());

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit(0);
#endif
        }

        [Button(ButtonSizes.Large)]
        public void Quit()
        {
            StartCoroutine(End());
        }


    }
}