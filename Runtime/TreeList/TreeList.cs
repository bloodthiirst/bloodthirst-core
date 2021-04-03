using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.TreeList
{
    public class TreeList<TKey, TElement> where TKey : class
    {
        public List<TreeLeaf<TKey, TElement>> SubLeafs { get; set; }

        public HashSet<TKey> AllLeafKeys { get; set; }

        public TreeList()
        {
            SubLeafs = new List<TreeLeaf<TKey, TElement>>();
            AllLeafKeys = new HashSet<TKey>();
        }

        public bool LookForKey(TKey key, out TreeLeafInfo<TKey, TElement> info)
        {
            info = default;

            if (SubLeafs == null)
            {
                return false;
            }

            if (SubLeafs.Count == 0)
                return false;

            for (int i = 0; i < SubLeafs.Count; i++)
            {
                if (SubLeafs[i].LookForKey(key, out info))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// get an element of a key with all the sub element in the subleafs recursively
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<TElement> GetElementsRecursivly(TKey key)
        {
            if (!AllLeafKeys.Contains(key))
                yield break;

            LookForKey(key, out TreeLeafInfo<TKey, TElement> info);

            foreach(TElement s in info.TreeLeaf.TraverseAllSubElements())
            {
                yield return s;
            }
        }

        /// <summary>
        /// <para> Get or create a sequence of interconnected leafs using the order in the list passed</para>
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>The leaf associated with the first key of the list passed</returns>
        public TreeLeaf<TKey, TElement> GetOrCreateLeaf(IList<TKey> keys)
        {
            // first key 
            TreeLeaf<TKey, TElement> firstLeaf = null;
            TKey firstKey = keys[0];

            // if the key already exists
            if (!AllLeafKeys.Add(firstKey))
            {
                if (LookForKey(firstKey, out TreeLeafInfo<TKey, TElement> info))
                {
                    firstLeaf = info.TreeLeaf;
                }
            }

            else
            {
                firstLeaf = new TreeLeaf<TKey, TElement>();
                firstLeaf.LeafKey = firstKey;
                SubLeafs.Add(firstLeaf);
            }

            TreeLeaf<TKey, TElement> previousLeaf = firstLeaf;

            TreeLeaf<TKey, TElement> currentLeaf = null;

            // keep navigating to the next leafs
            // and linking the leafs
            for (int i = 1; i < keys.Count; i++)
            {
                TKey curr = keys[i];

                // if the key already exists
                if (!AllLeafKeys.Add(curr))
                {
                    if (LookForKey(curr, out TreeLeafInfo<TKey, TElement> info))
                    {
                        currentLeaf = info.TreeLeaf;
                    }
                }

                // else create it and add it
                else
                {
                    currentLeaf = new TreeLeaf<TKey, TElement>();
                    currentLeaf.LeafKey = curr;
                    SubLeafs.Add(currentLeaf);
                }

                if (previousLeaf != null)
                {
                    currentLeaf.AddSubLeaf(previousLeaf);
                }

                previousLeaf = currentLeaf;
            }

            // return the first leaf
            return firstLeaf;
        }

        /// <summary>
        /// Add an element in all the list of the subkeys
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public TreeLeaf<TKey, TElement> AddElement(IList<TKey> keys, TElement element)
        {
            TreeLeaf<TKey, TElement> firstLeaf = GetOrCreateLeaf(keys);

            firstLeaf.AddElement(element);

            return firstLeaf;
        }
    }
}
