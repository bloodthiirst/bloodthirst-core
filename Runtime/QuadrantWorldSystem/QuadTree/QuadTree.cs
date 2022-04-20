using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quadrant
{
    public class QuadTree<TKey, TElement>
    {
        public QuadLeaf<TKey, TElement> rootLeaf { get; private set; }

        public QuadTree()
        {
            rootLeaf = new QuadLeaf<TKey, TElement>(default);
        }

        public void Clear()
        {
            rootLeaf.Clear();
        }

        public List<QuadLeaf<TKey, TElement>> GetFinalLeafs()
        {
            List<QuadLeaf<TKey, TElement>> lst = new List<QuadLeaf<TKey, TElement>>();

            if (rootLeaf.SubLeafs.Count == 0)
                return lst;

            foreach (QuadLeaf<TKey, TElement> s in rootLeaf.SubLeafs)
            {
                lst.AddRange(s.GetFinalLeafs());
            }

            return lst;
        }

        /// <summary>
        /// <para>Traverse the quad tree using the keys passed</para>
        /// <para>If a key is not found along the traversal then it gets created</para>
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>returns the last leaf corresponding to the last key</returns>
        public QuadLeaf<TKey, TElement> Get(IEnumerable<TKey> keys)
        {
            QuadLeaf<TKey, TElement> current = rootLeaf;

            // skip first and keep running down the tree
            foreach (TKey k in keys)
            {
                current = current.Get(k);

                if (current == null)
                    return null;
            }

            // return the last leaf
            return current;
        }

        /// <summary>
        /// <para>Traverse the quad tree using the keys passed</para>
        /// <para>If a key is not found along the traversal then it gets created</para>
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>returns the last leaf corresponding to the last key</returns>
        public QuadLeaf<TKey, TElement> GetOrCreate(IEnumerable<TKey> keys)
        {
            QuadLeaf<TKey, TElement> current = rootLeaf;

            // skip first and keep running down the tree
            foreach (TKey k in keys)
            {
                current = current.GetOrCreate(k);
            }

            // return the last leaf
            return current;
        }

        /// <summary>
        /// <para>Traverse the quad tree using the keys passed</para>
        /// <para>If a key is not found along the traversal then it gets created</para>
        /// </summary>
        /// <param name="keys">List of keys to traverse</param>
        /// <returns>Returns all the values found along the way in order</returns>
        public List<QuadLeaf<TKey, TElement>> GetOrCreateWithPath(IEnumerable<TKey> keys)
        {
            List<QuadLeaf<TKey, TElement>> path = new List<QuadLeaf<TKey, TElement>>();

            QuadLeaf<TKey, TElement> current = rootLeaf;

            // skip first and keep running down the tree while traking the leafs
            foreach (TKey k in keys)
            {
                current = current.GetOrCreate(k);
                path.Add(current);
            }

            // return the path
            return path;
        }

        public void Remove(IEnumerable<TKey> keys)
        {
            // if keys are empty then exist
            if (keys.Count() == 0)
                return;

            QuadLeaf<TKey, TElement> current = rootLeaf;

            // keep going down the tree until we find the key we want
            // we exist if a key is not found
            foreach (TKey k in keys)
            {
                current = current.Get(k);

                if (current == null)
                    return;
            }

            // the node
            current.Clear();
        }
    }
}
