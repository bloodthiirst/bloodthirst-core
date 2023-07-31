using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

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
            if (executeOnStart)
                yield return Setup();

            else
                yield break;
        }

        public IEnumerator Setup()
        {
            // query
            using (ListPool<GameObject>.Get(out List<GameObject> allGos))
            {
                GameObjectUtils.GetAllRootGameObjects(allGos);
                string[] oldNames = allGos.Select(p => p.name).ToArray();

                // pre
                List<IPreGameSetup> preSetups = GameObjectUtils.GetAllComponents<IPreGameSetup>(allGos, true).OrderBy(p => p.Order).ToList();

                foreach (IPreGameSetup p in preSetups)
                {
                    p.Execute();
                }
                // setup
                LoadingManager manager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

                List<IGameSetup> gameSetups = new List<IGameSetup>();
                GameObjectUtils.GetAllComponents(ref gameSetups, true);

                List<IAsynOperationWrapper> asyncOps = gameSetups.SelectMany(g => g.GetAsynOperations()).ToList();

                foreach (IAsynOperationWrapper op in asyncOps)
                {
                    manager.RunAsyncTask(op);
                }
                yield return new WaitWhile(() => manager.State == LOADDING_STATE.LOADING);

                // post
                allGos.Clear();
                GameObjectUtils.GetAllRootGameObjects(allGos);

                /*
                 * check for scene count change
                string[] newNames = allGos.Select(p => p.name).ToArray();

                Dictionary<string, int> counter = new Dictionary<string, int>();

                foreach (string n in oldNames)
                {
                    if (counter.ContainsKey(n))
                    {
                        counter[n]++;
                    }
                    else
                    {
                        counter.Add(n, 1);
                    }
                }
                foreach (string n in newNames)
                {
                    if (counter.ContainsKey(n))
                    {
                        counter[n]++;
                    }
                    else
                    {
                        counter.Add(n, 1);
                    }
                }
                */


                List<IPostGameSetup> postSetups = GameObjectUtils.GetAllComponents<IPostGameSetup>(allGos, true).ToList();

                foreach (IPostGameSetup p in postSetups)
                {
                    p.Execute();
                }
            }
        }
    }
}