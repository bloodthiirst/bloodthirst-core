using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Utils
{
    public static class GameObjectUtils
    {
        public static void GetAllRootGameObjects(List<GameObject> lst)
        {
            List<GameObject> cache = ListPool<GameObject>.Get();

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                scene.GetRootGameObjects(cache);

                lst.AddRange(cache);
            }

            ListPool<GameObject>.Release(cache);
        }

        public static void MoveToScene(GameObject go , Scene scene)
        {
            SceneManager.MoveGameObjectToScene(go, scene);
        }

        public static List<GameObject> GetAllRootGameObjects()
        {
            List<GameObject> lst = new List<GameObject>();
            List<GameObject> cache = new List<GameObject>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

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

            GetAllComponents(ref list, gos, includeInactive);
        }

        public static void GetAllComponents<T>(ref List<T> list, IList<GameObject> go, bool includeInactive)
        {
            for (int i = 0; i < go.Count; i++)
            {
                GameObject rootGameObject = go[i];
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
