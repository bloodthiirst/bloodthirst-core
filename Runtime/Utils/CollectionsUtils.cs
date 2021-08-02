using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for collections
    /// </summary>
    public static class CollectionsUtils
    {


        /// <summary>
        /// Collection case to linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionBase"></param>
        /// <returns></returns>
        public static IEnumerable<T> CollectionToLinq<T>(this CollectionBase collectionBase)
        {
            foreach (object i in collectionBase)
            {
                yield return (T)i;
            }
        }


        /// <summary>
        /// Used to combine coroutines
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IEnumerator Combine(this IEnumerator curr, IEnumerator other)
        {
            while (curr.MoveNext())
            {
                yield return curr.Current;
            }

            while (other.MoveNext())
            {
                yield return other.Current;
            }
        }

        public static bool IsSame<T>(this IList<T> a, IList<T> b) where T : IEquatable<T>
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }

            return true;
        }
        public static string ReplaceInterval(this string value, int startIndex, int endIndex, char replacementChar)
        {
            char[] chatArray = value.ToCharArray();

            for (int i = startIndex; i < endIndex; i++)
            {
                chatArray[i] = replacementChar;
            }

            return new string(chatArray);
        }

        /// <summary>
        /// Add a new key-value pair to the dictionary if the key doesnt exist
        /// </summary>
        /// <typeparam name="K">TKey</typeparam>
        /// <typeparam name="V">TValue</typeparam>
        /// <param name="dict">dictionaty</param>
        /// <param name="key">Key value</param>
        public static void AddKeyValue<K, V>(this Dictionary<K, V> dict, K key) where V : new()
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new V());
            }
        }

        public static V GetOrCreateValue<K, V>(this IDictionary<K, V> dict, K key) where V : new()
        {
            if (!dict.TryGetValue(key, out V val))
            {
                val = new V();
                dict.Add(key, val);
                return val;
            }

            return val;
        }

        public static bool Has<T>(this IEnumerable<T> list, Predicate<T> filter)
        {
            return list.FirstOrDefault(t => filter(t)) != null;
        }

        public static T CreateIfNull<T>(this T instance) where T : new()
        {
            if (instance == null)
                instance = new T();

            return instance;
        }

        public static C CreateOrClear<C, T>(this C collection) where C : ICollection<T>, new()
        {
            if (collection == null)
                collection = new C();
            else
                collection.Clear();

            return collection;
        }

        public static C CreateOrClear<C>(this C collection) where C : IList, new()
        {
            if (collection == null)
                collection = new C();
            else
                collection.Clear();

            return collection;
        }


        public static void ExpandeSize<T>(this List<T> list, int size)
        {
            while (list.Count < size)
            {
                list.Add(default(T));
            }
        }

        /// <summary>
        /// Copy the dictionary's content to another dictionary
        /// doesn't clear the 'to' dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopyDictionary<K, V>(this IDictionary<K, V> from, IDictionary<K, V> to)
        {
            foreach (KeyValuePair<K, V> kv in from)
            {
                to.Add(kv.Key, kv.Value);
            }
        }
    }
}
