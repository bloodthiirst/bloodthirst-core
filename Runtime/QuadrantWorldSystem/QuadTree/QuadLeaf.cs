using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quadrant
{
    public class QuadLeaf<TKey, TElement>
    {
        internal List<TKey> PreviousKeys { get; set; }
        public TKey Key { get; set; }
        public HashSet<TElement> Elements { get; set; }

        public List<QuadLeaf<TKey, TElement>> SubLeafs { get; set; }

        public QuadLeaf(TKey key)
        {
            Key = key;
            Elements = new HashSet<TElement>();
            PreviousKeys = new List<TKey>();
            SubLeafs = new List<QuadLeaf<TKey, TElement>>();
        }

        public List<TKey> GetKeySequence()
        {
            List<TKey> keySequence = new List<TKey>();

            keySequence.Clear();
            keySequence.AddRange(PreviousKeys);
            keySequence.Add(Key);

            return keySequence;
        }

        public QuadLeaf(TKey key, HashSet<TElement> elements)
        {
            Key = key;
            Elements = elements;
            PreviousKeys = new List<TKey>();
            SubLeafs = new List<QuadLeaf<TKey, TElement>>();
        }

        /// <summary>
        /// Get all the elements recursively
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QuadLeaf<TKey, TElement>> GetAllRecursively()
        {
            yield return this;

            foreach (QuadLeaf<TKey, TElement> l in SubLeafs)
            {
                foreach (QuadLeaf<TKey, TElement> e in l.GetAllRecursively())
                {
                    yield return e;
                }
            }

        }

        /// <summary>
        /// Get the final leafs (leafs with no sub-leafs) start with this leafs as the root
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QuadLeaf<TKey, TElement>> GetFinalLeafs()
        {
            if (SubLeafs.Count == 0)
                yield return this;

            foreach (QuadLeaf<TKey, TElement> l in SubLeafs)
            {
                foreach (QuadLeaf<TKey, TElement> s in l.GetFinalLeafs())
                {
                    yield return s;
                }
            }

        }
        /// <summary>
        /// Clear subleafs and current element
        /// </summary>
        public void Clear()
        {
            foreach (QuadLeaf<TKey, TElement> sub in SubLeafs)
            {
                sub.Clear();
            }

            SubLeafs.Clear();
        }

        public QuadLeaf<TKey, TElement> Get(TKey key)
        {
            return SubLeafs.FirstOrDefault(l => l.Key.Equals(key));
        }

        public QuadLeaf<TKey, TElement> GetOrCreate(TKey key)
        {
            QuadLeaf<TKey, TElement> res = SubLeafs.FirstOrDefault(l => l.Key.Equals(key));

            if (res != null)
                return res;

            res = new QuadLeaf<TKey, TElement>(key);
            res.PreviousKeys.AddRange(PreviousKeys);
            res.PreviousKeys.Add(Key);
            SubLeafs.Add(res);

            return res;
        }
    }
}
