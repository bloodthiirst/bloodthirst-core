using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quadrant
{
    public class QuadTree<TKey, TElement>
    {
        private List<QuadLeaf<TKey, TElement>> rootLeafs { get; set; }

        public IReadOnlyList<QuadLeaf<TKey, TElement>> RootLeafs => rootLeafs;

        public QuadTree()
        {
            rootLeafs = new List<QuadLeaf<TKey, TElement>>();
        }

        public void Clear()
        {
            foreach (QuadLeaf<TKey, TElement> l in rootLeafs)
            {
                l.Clear();
            }

            rootLeafs.Clear();
        }


        public HashSet<QuadLeaf<TKey, TElement>> GetFinalLeafs()
        {
            HashSet<QuadLeaf<TKey, TElement>> lst = new HashSet<QuadLeaf<TKey, TElement>>();

            foreach (QuadLeaf<TKey, TElement> rootLeaf in RootLeafs)
            {
                foreach (QuadLeaf<TKey, TElement> s in rootLeaf.GetFinalLeafs())
                {
                    lst.Add(s);
                }
            }

            return lst;
        }

        public QuadLeaf<TKey, TElement> Traverse(List<TKey> keys)
        {
            QuadLeaf<TKey, TElement> current = rootLeafs.FirstOrDefault(l => l.Key.Equals(keys[0]));

            // add the entry leaf if it doesn't exist
            if (current == null)
            {
                current = new QuadLeaf<TKey, TElement>(keys[0]);
                rootLeafs.Add(current);
            }

            for (int i = 1; i < keys.Count; i++)
            {
                current = current.GetOrCreate(keys[i]);
            }

            return current;
        }

    }
}
