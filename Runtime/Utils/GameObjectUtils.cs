using Bloodthirst.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Core.Utils
{
    public static class GameObjectUtils
    {
        public static void GetAllComponents<T>(ref List<T> list, bool includeInactive)
        {
            list = CollectionsUtils.CreateOrClear(list);

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach (GameObject rootGameObject in rootGameObjects)
                {
                    T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>(includeInactive);
                    foreach (T childInterface in childrenInterfaces)
                    {
                        list.Add(childInterface);
                    }
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
