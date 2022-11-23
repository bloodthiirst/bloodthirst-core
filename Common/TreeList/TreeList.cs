using Sirenix.Serialization;
using System.Collections.Generic;

namespace Bloodthirst.Core.TreeList
{
    public class TreeList<TKey, TValue>
    {
        [OdinSerialize]
        private List<TreeLeaf<TKey, TValue>> allSubLeafs;

        public List<TreeLeaf<TKey, TValue>> AllSubLeafs { get => allSubLeafs; set => allSubLeafs = value; }

        public TreeList()
        {
            AllSubLeafs = new List<TreeLeaf<TKey, TValue>>();
        }

        public bool LookForKey(TKey key, out TreeLeafInfo<TKey, TValue> info)
        {
            info = default;

            if (AllSubLeafs == null)
            {
                return false;
            }

            if (AllSubLeafs.Count == 0)
                return false;

            for (int i = 0; i < AllSubLeafs.Count; i++)
            {
                if (AllSubLeafs[i].LookForKeyRecursive(key, out info))
                    return true;
            }

            return false;
        }

        public void Clear()
        {
            AllSubLeafs.Clear();
        }

        public IEnumerable<TreeLeaf<TKey, TValue>> GetFinalLeafs()
        {
            if (AllSubLeafs == null)
                yield break;

            foreach (TreeLeaf<TKey, TValue> l in AllSubLeafs)
            {
                if (l.SubLeafs == null || l.SubLeafs.Count == 0)
                    yield return l;
            }
        }

        public IEnumerable<TreeLeaf<TKey, TValue>> GetRootLeafs()
        {
            if (AllSubLeafs == null)
                yield break;

            foreach (TreeLeaf<TKey, TValue> l in AllSubLeafs)
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

        public IEnumerable<TreeLeaf<TKey, TValue>> GetAllLeafsDepthFirst()
        {
            foreach (TreeLeaf<TKey, TValue> sub in AllSubLeafs)
            {
                foreach (TreeLeaf<TKey, TValue> val in sub.TraverseAllSubLeafsDepthFirst())
                {
                    yield return val;
                }
            }
        }

        public IEnumerable<TreeLeaf<TKey, TValue>> GetAllLeafsDepthFirstFromFinalLeafsUp()
        {
            foreach (TreeLeaf<TKey, TValue> sub in AllSubLeafs)
            {
                foreach (TreeLeaf<TKey, TValue> val in sub.TraverseAllLeafsDepthFirstFromFinalLeafsUp())
                {
                    yield return val;
                }
            }
        }


        /// <summary>
        /// <para> Get or create a sequence of interconnected leafs using the order in the list passed</para>
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>The leaf associated with the first key of the list passed</returns>
        public TreeLeaf<TKey, TValue> GetOrCreateLeaf(IList<TKey> keys)
        {
            // skip the leafs that already exist
            TreeLeaf<TKey, TValue> lastLeaf = null;

            int i = 0;
            while (i < keys.Count)
            {
                TKey currKey = keys[i];

                TreeLeaf<TKey, TValue> tryFindLastLeaf = AllSubLeafs.Find(l => l.LeafKey.Equals(currKey));

                if (tryFindLastLeaf == null)
                    break;

                lastLeaf = tryFindLastLeaf;
                i++;
            }

            // create new leafs
            // keep navigating to the next leafs
            // and linking the leafs
            for (; i < keys.Count; i++)
            {
                // create
                TKey currKey = keys[i];
                TreeLeaf<TKey, TValue> currLeaf = new TreeLeaf<TKey, TValue>();
                currLeaf.LeafKey = currKey;

                if (lastLeaf != null)
                {
                    lastLeaf.AddSubLeaf(currLeaf);
                }

                AllSubLeafs.Add(currLeaf);

                lastLeaf = currLeaf;
            }

            // return the first leaf
            return lastLeaf;
        }
    }
}
