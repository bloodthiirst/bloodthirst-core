using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Core.Utils
{
    public static class GameObjectUtils
    {
        public static List<TPass> GetAllPasses<TPass>(IList<GameObject> gameObjects) where TPass : IGamePass
        {
            List<TPass> list = new List<TPass>();

            GetAllComponents(ref list, gameObjects, true);

            return list;
        }

        public static void ExecuteAllPasses<TPass>(IList<TPass> passes) where TPass : IGamePass
        {
            foreach (TPass p in passes)
            {
                p.Execute();
            }
        }

        public static void GetAndExecute<TPass>(IList<GameObject> gos) where TPass : IGamePass
        {
            List<TPass> lst = GetAllPasses<TPass>(gos);
            ExecuteAllPasses(lst);
        }

        public static List<GameObject> GetAllRootGameObjects()
        {
            List<GameObject> lst = new List<GameObject>();
            List<GameObject> cache = new List<GameObject>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                cache.Clear();
                cache.Capacity = scene.rootCount;
                scene.GetRootGameObjects(cache);

                lst.AddRange(cache);
            }

            return lst;
        }

        public static List<T> GetAllComponents<T>(List<GameObject> gos, bool includeInactive)
        {
            List<T> list = new List<T>();

            GetAllComponents(ref list, gos, true);

            return list;
        }

        public static void GetAllComponents<T>(ref List<T> list, bool includeInactive)
        {
            list = CollectionsUtils.CreateOrClear(list);

            List<GameObject> gos = GetAllRootGameObjects();

            GetAllComponents(ref list, gos, true);
        }

        public static void GetAllComponents<T>(ref List<T> list, IList<GameObject> go, bool includeInactive)
        {
            foreach (GameObject rootGameObject in go)
            {
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>(includeInactive);
                foreach (T childInterface in childrenInterfaces)
                {
                    list.Add(childInterface);
                }
            }
        }

        /// <summary>
        /// Combines a list of coroutines into one with the same order in the params
        /// </summary>
        /// <param name="enumerators"></param>
        /// <returns></returns>
        public static IEnumerator CombineCoroutine(params IEnumerator[] enumerators)
        {
            foreach (IEnumerator c in enumerators)
            {
                while (c.MoveNext())
                {
                    yield return c.Current;
                }
            }
        }
        public static void ClearTransform(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(t.GetChild(i).gameObject);
            }
        }

        public static void ClearTransformEditor(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(t.GetChild(i).gameObject);
            }
        }
    }
}
