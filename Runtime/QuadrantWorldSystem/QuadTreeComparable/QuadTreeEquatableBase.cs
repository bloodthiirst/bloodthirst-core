using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quadrant
{
    public class QuadTreeEquatableBase<TKey, TElement , TLeaf> where TKey : IEquatable<TKey> where TLeaf : QuadLeafEquatableBase<TKey,TElement,TLeaf> , new()
    {
        private List<TLeaf> rootLeafs { get; set; }

        public IReadOnlyList<TLeaf> RootLeafs => rootLeafs;

        public QuadTreeEquatableBase()
        {
            rootLeafs = new List<TLeaf>();
        }

        public void Clear()
        {
            foreach (TLeaf l in rootLeafs)
            {
                l.Clear();
            }

            rootLeafs.Clear();
        }


        public HashSet<TLeaf> GetFinalLeafs()
        {
            HashSet<TLeaf> lst = new HashSet<TLeaf>();

            foreach (TLeaf rootLeaf in RootLeafs)
            {
                foreach (TLeaf s in rootLeaf.GetFinalLeafs())
                {
                    lst.Add(s);
                }
            }

            return lst;
        }

        public TLeaf Traverse(List<TKey> keys)
        {
            TLeaf current = rootLeafs.FirstOrDefault(l => l.Key.Equals(keys[0]));

            // add the entry leaf if it doesn't exist
            if (current == null)
            {
                current = new TLeaf();
                current.Key = keys[0];
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
