using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core.Utils
{
    public static class GameObjectUtils
    {
        /// <summary>
        /// Combines a list of coroutines into one with the same order in the params
        /// </summary>
        /// <param name="enumerators"></param>
        /// <returns></returns>
        public static IEnumerator CombineCoroutine( params IEnumerator[] enumerators )
        {
            foreach(IEnumerator c in enumerators)
            {
                while(c.MoveNext())
                {
                    yield return c.Current;
                }
            }
        }
        public static void ClearTransform(this Transform t)
        {
            for(int i = t.childCount - 1; i >= 0; i--)
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

        public static T CreateIfNull<T>(this T instance) where T : new()
        {
            if (instance == null)
                instance = new T();

            return instance;
        }

        public static C CreateOrClear<C> (this C collection) where C : IList , new()
        {
            if (collection == null)
                collection = new C();
            else
                collection.Clear();

            return collection;
        }

        public static void CreateOrClearDict<D>(this D collection) where D : IDictionary, new()
        {
            if (collection == null)
                collection = new D();
            else
                collection.Clear();

        }
    }
}
