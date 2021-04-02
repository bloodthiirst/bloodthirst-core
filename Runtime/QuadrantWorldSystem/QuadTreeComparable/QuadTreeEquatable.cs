using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quadrant
{
    public class QuadTreeEquatable<TKey, TElement> where TKey : IEquatable<TKey>
    {
        private List<QuadLeafEquatable<TKey, TElement>> rootLeafs { get; set; }

        public IReadOnlyList<QuadLeafEquatable<TKey, TElement>> RootLeafs => rootLeafs;

        public QuadTreeEquatable()
        {
            rootLeafs = new List<QuadLeafEquatable<TKey, TElement>>();
        }

        public void Clear()
        {
            foreach (QuadLeafEquatable<TKey, TElement> l in rootLeafs)
            {
                l.Clear();
            }

            rootLeafs.Clear();
        }


        public HashSet<QuadLeafEquatable<TKey, TElement>> GetFinalLeafs()
        {
            HashSet<QuadLeafEquatable<TKey, TElement>> lst = new HashSet<QuadLeafEquatable<TKey, TElement>>();

            foreach (QuadLeafEquatable<TKey, TElement> rootLeaf in RootLeafs)
            {
                foreach (QuadLeafEquatable<TKey, TElement> s in rootLeaf.GetFinalLeafs())
                {
                    lst.Add(s);
                }
            }

            return lst;
        }

        public QuadLeafEquatable<TKey, TElement> Traverse(List<TKey> keys)
        {
            QuadLeafEquatable<TKey, TElement> current = rootLeafs.FirstOrDefault(l => l.Key.Equals(keys[0]));

            // add the entry leaf if it doesn't exist
            if (current == null)
            {
                current = new QuadLeafEquatable<TKey, TElement>(keys[0]);
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
