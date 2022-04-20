using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bloodthirst.Core.Utils;

namespace Bloodthirst.Core.Setup
{
    public class GameStart : MonoBehaviour
    {

        [SerializeField]
        private bool executeOnStart;
        public bool ExecuteOnStart
        {
            get => executeOnStart;
            set => executeOnStart = value;
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            if (ExecuteOnStart)
                yield return Setup();
            else
                yield break;
        }

        public IEnumerator Setup()
        {
            // query
            List<GameObject> allGos = GameObjectUtils.GetAllRootGameObjects();

            // pre
            GameObjectUtils.GetAllComponents<IPreGameSetup>(allGos, true).OrderBy(p => p.Order).ToList().ForEach((e) => e.Execute());

            // setup
            LoadingManager manager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

            List<IGameSetup> gameSetups = new List<IGameSetup>();
            GameObjectUtils.GetAllComponents(ref gameSetups, true);

            IEnumerable<IAsynOperationWrapper> asyncOps = gameSetups.Select(g => g.GetAsynOperations());

            foreach (IAsynOperationWrapper op in asyncOps)
            {
                manager.Load(op);
            }
            yield return new WaitWhile(() => manager.State == LOADDING_STATE.LOADING);

            // post
            GameObjectUtils.GetAllComponents<IPostGameSetup>(allGos, true).ForEach((e) => e.Execute());
        }

    }
}