using Bloodthirst.Core.Utils;
using Bloodthirst.System.Quadrant;
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
    public struct SieveBranchBuilder<TEntity, TEnum> where TEnum : Enum
    {
        private List<TEnum> keys;
        private List<Predicate<TEntity>> predicates;

        private TreeSieve<TEntity, TEnum> targetSieve;

        public IReadOnlyDictionary<TEnum, Predicate<TEntity>> CurrentBranch => CurrentBranch;

        internal SieveBranchBuilder(TreeSieve<TEntity, TEnum> targetSieve)
        {
            this.targetSieve = targetSieve;
            keys = new List<TEnum>();
            predicates = new List<Predicate<TEntity>>();
        }

        public SieveBranchBuilder<TEntity, TEnum> Add(TEnum id, Predicate<TEntity> predicate)
        {
            keys.Add(id);
            predicates.Add(predicate);
            return this;
        }

        public SieveBranchBuilder<TEntity, TEnum> Remove(TEnum id, Predicate<TEntity> predicate)
        {
            int index = keys.IndexOf(id);

            if (index == -1)
                return this;

            keys.RemoveAt(index);
            predicates.RemoveAt(index);

            return this;
        }

        public List<TEnum> GetKeys()
        {
            return keys;
        }

        /// <summary>
        /// Build the branches accumulated in the builder
        /// </summary>
        public void Build(List<TEnum> keys)
        {
            // create list in the tree 
            targetSieve.quadTree.GetOrCreateWithPath(keys);

            // create list in the flat dictionary
            foreach (TEnum k in keys)
            {
                targetSieve.flatDictionary.GetOrCreateValue(k);
            }
        }
    }
}
