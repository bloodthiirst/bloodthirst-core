using System.Collections.Generic;

namespace Bloodthirst.Core.TreeList
{
    public class TreeList<TKey, TValue> where TKey : class
    {
        public List<TreeLeaf<TKey, TValue>> SubLeafs { get; set; }

        public HashSet<TKey> AllLeafKeys { get; set; }

        public TreeList()
        {
            SubLeafs = new List<TreeLeaf<TKey, TValue>>();
            AllLeafKeys = new HashSet<TKey>();
        }

        public bool LookForKey(TKey key, out TreeLeafInfo<TKey, TValue> info)
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
        public IEnumerable<TValue> GetElementsRecursivly(TKey key)
        {
            if (!AllLeafKeys.Contains(key))
                yield break;

            LookForKey(key, out TreeLeafInfo<TKey, TValue> info);

            foreach(TValue s in info.TreeLeaf.TraverseAllSubElements())
            {
                yield return s;
            }
        }

        /// <summary>
        /// <para> Get or create a sequence of interconnected leafs using the order in the list passed</para>
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>The leaf associated with the first key of the list passed</returns>
        public TreeLeaf<TKey, TValue> GetOrCreateLeaf(IList<TKey> keys)
        {
            // first key 
            TreeLeaf<TKey, TValue> firstLeaf = null;
            TKey firstKey = keys[0];

            // if the key already exists
            if (!AllLeafKeys.Add(firstKey))
            {
                if (LookForKey(firstKey, out TreeLeafInfo<TKey, TValue> info))
                {
                    firstLeaf = info.TreeLeaf;
                }
            }

            else
            {
                firstLeaf = new TreeLeaf<TKey, TValue>();
                firstLeaf.LeafKey = firstKey;
                SubLeafs.Add(firstLeaf);
            }

            TreeLeaf<TKey, TValue> previousLeaf = firstLeaf;

            TreeLeaf<TKey, TValue> currentLeaf = null;

            // keep navigating to the next leafs
            // and linking the leafs
            for (int i = 1; i < keys.Count; i++)
            {
                TKey curr = keys[i];

                // if the key already exists
                if (!AllLeafKeys.Add(curr))
                {
                    if (LookForKey(curr, out TreeLeafInfo<TKey, TValue> info))
                    {
                        currentLeaf = info.TreeLeaf;
                    }
                }

                // else create it and add it
                else
                {
                    currentLeaf = new TreeLeaf<TKey, TValue>();
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
    }
}