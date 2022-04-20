using System.Collections.Generic;

namespace Bloodthirst.Core.TreeList
{
    public class TreeList<TKey, TValue>
    {
        public List<TreeLeaf<TKey, TValue>> SubLeafs { get; set; }

        public TreeList()
        {
            SubLeafs = new List<TreeLeaf<TKey, TValue>>();
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
                if (SubLeafs[i].LookForKeyRecursive(key, out info))
                    return true;
            }

            return false;
        }

        public void Clear()
        {
            SubLeafs.Clear();
        }

        public IEnumerable<TreeLeaf<TKey, TValue>> GetFinalLeafs()
        {
            if (SubLeafs == null)
                yield break;

            foreach (TreeLeaf<TKey, TValue> l in SubLeafs)
            {
                if (l.SubLeafs == null || l.SubLeafs.Count == 0)
                    yield return l;
            }
        }

        public IEnumerable<TreeLeaf<TKey, TValue>> GetRootLeafs()
        {
            if (SubLeafs == null)
                yield break;

            foreach (TreeLeaf<TKey, TValue> l in SubLeafs)
            {
                if (l.Parent == null)
                    yield return l;
            }
        }

        /// <summary>
        /// get an element of a key with all the sub element in the subleafs recursively
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<TValue> GetElementsRecursivly(TKey key)
        {
            if (!LookForKey(key, out TreeLeafInfo<TKey, TValue> info))
                yield break;

            foreach (TValue s in info.TreeLeaf.TraverseAllSubElements())
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
            TKey firstKey = keys[0];
            TreeLeaf<TKey, TValue> firstLeaf = SubLeafs.Find(l => l.LeafKey.Equals(firstKey));

            if (firstLeaf == null)
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
                TKey currKey = keys[i];

                TreeLeaf<TKey, TValue> leaf = previousLeaf.LookForKeyDirect(currKey);

                // if the key already exists
                if (leaf != null)
                {
                    currentLeaf = leaf;
                }

                // else create it and add it
                else
                {
                    currentLeaf = new TreeLeaf<TKey, TValue>();
                    currentLeaf.LeafKey = currKey;
                    previousLeaf.AddSubLeaf(currentLeaf);
                }

                previousLeaf = currentLeaf;
            }

            // return the first leaf
            return firstLeaf;
        }
    }
}
