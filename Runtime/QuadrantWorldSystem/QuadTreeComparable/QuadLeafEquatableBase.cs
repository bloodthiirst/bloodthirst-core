using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.System.Quadrant
{
    public class QuadLeafEquatableBase<TKey, TElement, TLeaf> where TKey : IEquatable<TKey> where TLeaf : QuadLeafEquatableBase<TKey, TElement, TLeaf>, new()
    {
        internal List<TKey> PreviousKeys { get; set; }
        public TKey Key { get; set; }
        private HashSet<TElement> elements;
        public IReadOnlyCollection<TElement> Elements => elements;
        public List<TLeaf> SubLeafs { get; set; }

        public QuadLeafEquatableBase()
        {
            elements = new HashSet<TElement>();
            PreviousKeys = new List<TKey>();
            SubLeafs = new List<TLeaf>();
        }

        public QuadLeafEquatableBase(TKey key) : base()
        {
            Key = key;
        }

        public List<TKey> GetKeySequence()
        {
            List<TKey> keySequence = new List<TKey>();

            keySequence.Clear();
            keySequence.AddRange(PreviousKeys);
            keySequence.Add(Key);

            return keySequence;
        }

        public void Add(TElement element)
        {
            elements.Add(element);
            OnElementAdded(element);
        }

        public void Remove(TElement element)
        {
            elements.Remove(element);
            OnElementRemoved(element);
        }

        protected virtual void OnElementAdded(TElement element) { }
        protected virtual void OnElementRemoved(TElement element) { }

        /// <summary>
        /// Get all the elements recursively
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TLeaf> GetAllRecursively()
        {
            yield return (TLeaf)this;

            foreach (TLeaf l in SubLeafs)
            {
                foreach (TLeaf e in l.GetAllRecursively())
                {
                    yield return e;
                }
            }

        }

        /// <summary>
        /// Get the final leafs (leafs with no sub-leafs) start with this leafs as the root
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TLeaf> GetFinalLeafs()
        {
            if (SubLeafs.Count == 0)
                yield return (TLeaf)this;

            foreach (TLeaf l in SubLeafs)
            {
                foreach (TLeaf s in l.GetFinalLeafs())
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
            foreach (TLeaf sub in SubLeafs)
            {
                sub.Clear();
            }

            SubLeafs.Clear();
        }

        public TLeaf Get(TKey key)
        {
            return SubLeafs.FirstOrDefault(l => l.Key.Equals(key));
        }

        public TLeaf GetOrCreate(TKey key)
        {
            TLeaf res = SubLeafs.FirstOrDefault(l => l.Key.Equals(key));

            if (res != null)
                return res;

            res = new TLeaf();
            res.Key = key;
            res.PreviousKeys.AddRange(PreviousKeys);
            res.PreviousKeys.Add(Key);
            SubLeafs.Add(res);

            return res;
        }
    }
}
