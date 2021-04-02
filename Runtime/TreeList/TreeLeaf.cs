using System.Collections.Generic;

namespace Bloodthirst.Core.TreeList
{
    public class TreeLeaf<TKey, TElement> where TKey : class
    {
        public TKey LeafKey { get; set; }
        public List<TElement> Elements { get; set; }
        public TreeLeaf<TKey,TElement> Parent { get; set; }
        private List<TreeLeaf<TKey, TElement>> _SubLeafs { get; set; }
        public IReadOnlyList<TreeLeaf<TKey, TElement>> SubLeafs => _SubLeafs;

        private bool LookForKeyInternal(TKey searchKey, TreeLeafInfo<TKey, TElement> info, out TreeLeafInfo<TKey, TElement> result)
        {
            info.Depth++;

            if (LeafKey == searchKey)
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
                if (SubLeafs[i].LookForKeyInternal(searchKey, info, out result))
                {
                    return true;
                }
            }

            result = info;
            return false;
        }

        public IEnumerable<TElement> TraverseAllSubElements()
        {
            if (Elements != null)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    yield return Elements[i];
                }
            }

            if (SubLeafs == null)
                yield break;

            for (int i = 0; i < SubLeafs.Count; i++)
            {
                TreeLeaf<TKey, TElement> s = SubLeafs[i];
                foreach (TElement e in s.TraverseAllSubElements())
                {
                    yield return e;
                }
            }
        }

        /// <summary>
        /// <para>Return the leaf with the Key value of <paramref name="key"/></para>
        /// <para>Returns null if keys not found in the leaf and it's sub leafs</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool LookForKey(TKey key, out TreeLeafInfo<TKey, TElement> info)
        {
            info = new TreeLeafInfo<TKey, TElement>();
            info.Depth = -1;
            info.KeysEncountered = new HashSet<TKey>();

            return LookForKeyInternal(key, info, out info);
        }

        /// <summary>
        /// Add e
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public void AddElement(TElement element)
        {
            if (Elements == null)
                Elements = new List<TElement>();

            Elements.Add(element);
        }

        /// <summary>
        /// Add e
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public void AddSubLeaf(TreeLeaf<TKey, TElement> leaf)
        {
            if (SubLeafs == null)
                _SubLeafs = new List<TreeLeaf<TKey, TElement>>();

            leaf.Parent = this;
            _SubLeafs.Add(leaf);
        }
    }
}
