using Bloodthirst.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.Setup
{
    public class GameSetup : MonoBehaviour
    {
        [SerializeField]
        private float progress;
        public float Progress => progress;

        [SerializeField]
        private bool executeOnStart;
        public bool ExecuteOnStart 
        { 
            get => executeOnStart;
            set => executeOnStart = value; 
        }

        private List<AsyncOperation> asyncOps = new List<AsyncOperation>();

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

            List<IPreGameSetup> preGameSetups = new List<IPreGameSetup>();

            List<IGameSetup> gameSetups = new List<IGameSetup>();

            List<IPostGameSetup> postGameSetups = new List<IPostGameSetup>();

            QueryScenes(ref preGameSetups);

            QueryScenes(ref gameSetups);

            QueryScenes(ref postGameSetups);

            List<IGrouping<int, IGameSetup>> ordered = gameSetups.GroupBy(o => o.Order).OrderBy(g => g.Key).ToList();

            // execute
            foreach (IPreGameSetup pre in preGameSetups)
            {
                pre.Execute();
            }

            for (int j = 0; j < ordered.Count; j++)
            {
                List<IGameSetup> kv = ordered[j].ToList();

                for (int g = 0; g < kv.Count; g++)
                {
                    IGameSetup v = kv[g];
                    asyncOps.Clear();
                    asyncOps.AddRange(v.Operations().ToList());

                    if (asyncOps.Count == 0)
                    {
                        continue;
                    }
                    
                    else
                    {

                        bool waveIsDone = false;
                        float currentProg = 0;


                        while (!waveIsDone)
                        {
                            currentProg = 0;
                            for (int i = 0; i < asyncOps.Count; i++)
                            {
                                currentProg += asyncOps[i].progress / asyncOps.Count;
                            }

                            progress = currentProg;
                            waveIsDone = progress == 1;

                            yield return null;
                        }
                    }
                }
            }

            foreach (IPostGameSetup post in postGameSetups)
            {
                post.Execute();
            }

        }

        public static void QueryScenes<T>(ref List<T> list)
        {
            list = CollectionsUtils.CreateOrClear(list);

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach (GameObject rootGameObject in rootGameObjects)
                {
                    T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>(true);
                    foreach (T childInterface in childrenInterfaces)
                    {
                        list.Add(childInterface);
                    }
                }

            }
        }

    }
}