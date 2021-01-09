﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for collections
    /// </summary>
    public static class CollectionsUtils
    {

        public static bool IsSame<T>(this IList<T> a , IList<T> b ) where T : IEquatable<T>
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            if (a.Count != b.Count)
                return false;

            for(int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }

            return true;
        }
        public static string ReplaceInterval(this string value , int startIndex , int endIndex , char replacementChar)
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
        public static void AddKeyValue<K,V>( this Dictionary<K,V> dict , K key) where V : new() {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new V());
            }
        }

        public static bool Has<T>(this IEnumerable<T> list , Predicate<T> filter)
        {
            return list.FirstOrDefault(t => filter(t)) != null;
        }

        /// <summary>
        /// Copy the dictionary's content to another dictionary
        /// doesn't clear the 'to' dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopyDictionary<K,V>( this IDictionary<K,V> from, IDictionary<K, V> to )
        {
            foreach(KeyValuePair<K, V> kv in from)
            {
                to.Add(kv.Key, kv.Value);
            }
        }
    }
}
