using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Bloodthirst.Core.Collections
{
    /// <summary>
    /// Builder class used to construct the sieve conditions
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public struct SieveBranchRemover<TEntity, TEnum> where TEnum : Enum
    {
        private List<TEnum> keys;

        private TreeSieve<TEntity, TEnum> targetSieve;

        public IReadOnlyDictionary<TEnum, Predicate<TEntity>> CurrentBranch => CurrentBranch;

        internal SieveBranchRemover(TreeSieve<TEntity, TEnum> targetSieve)
        {
            this.targetSieve = targetSieve;
            keys = new List<TEnum>();
        }

        public SieveBranchRemover<TEntity, TEnum> Add(TEnum id, Predicate<TEntity> predicate)
        {
            keys.Add(id);
            return this;
        }

        public SieveBranchRemover<TEntity, TEnum> Remove(TEnum id, Predicate<TEntity> predicate)
        {
            int index = keys.IndexOf(id);

            if (index == -1)
                return this;

            keys.RemoveAt(index);

            return this;
        }

        /// <summary>
        /// Build the branches accumulated in the builder
        /// </summary>
        public void Build()
        {
            // remove from the tree
            targetSieve.quadTree.Remove(keys);

            // remove from the flat dictionary
            foreach(TEnum k in keys)
            {
                targetSieve.flatDictionary.Remove(k);   
            }
        }
    }
}
