using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.TreeList
{
    public class TreeLeaf<TKey, TValue>
    {
        public TKey LeafKey { get; set; }
        public TValue Value { get; set; }
        public TreeLeaf<TKey, TValue> Parent { get; set; }
        private List<TreeLeaf<TKey, TValue>> _SubLeafs { get; set; }
        public IReadOnlyList<TreeLeaf<TKey, TValue>> SubLeafs => _SubLeafs;

        private bool LookForKeyRecursiveInternal(TKey searchKey, TreeLeafInfo<TKey, TValue> info, out TreeLeafInfo<TKey, TValue> result)
        {
            info.Depth++;

            if (LeafKey.Equals(searchKey))
            {
                info.TreeLeaf = this;
                result = info;
                return true;
            }


            // try saving the encoutred key
            // if we get false , that means that we already have seen this key
            // so we skip
            if (!info.KeysEncountered.Add(LeafKey))
            {
                result = info;
                return false;
            }


            if (SubLeafs == null || SubLeafs.Count == 0)
            {
                result = info;
                return false;
            }

            for (int i = 0; i < SubLeafs.Count; i++)
            {
                if (SubLeafs[i].LookForKeyRecursiveInternal(searchKey, info, out result))
                {
                    return true;
                }
            }

            result = info;
            return false;
        }
        /// <summary>
        /// Get the elements of the leaf and all of the elements in the subleafs recursivly
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TValue> TraverseAllSubElements()
        {
            if (Value != null)
            {
                yield return Value;
            }

            if (SubLeafs == null)
                yield break;

            for (int i = 0; i < SubLeafs.Count; i++)
            {
                TreeLeaf<TKey, TValue> s = SubLeafs[i];
                foreach (TValue e in s.TraverseAllSubElements())
                {
                    yield return e;
                }
            }
        }

        public TreeLeaf<TKey, TValue> LookForKeyDirect(TKey key)
        {
            return _SubLeafs?.Find(l => l.LeafKey.Equals(key));
        }

        /// <summary>
        /// <para>Return the leaf with the Key value of <paramref name="key"/></para>
        /// <para>Returns null if keys not found in the leaf and it's sub leafs</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool LookForKeyRecursive(TKey key, out TreeLeafInfo<TKey, TValue> info)
        {
            info = new TreeLeafInfo<TKey, TValue>();
            info.Depth = -1;
            info.KeysEncountered = new HashSet<TKey>();

            return LookForKeyRecursiveInternal(key, info, out info);
        }

        /// <summary>
        /// Add e
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public void AddSubLeaf(TreeLeaf<TKey, TValue> leaf)
        {
            if (SubLeafs == null)
                _SubLeafs = new List<TreeLeaf<TKey, TValue>>();

            leaf.Parent = this;
            _SubLeafs.Add(leaf);
        }
    }
}
