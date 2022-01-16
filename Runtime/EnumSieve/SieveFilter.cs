using Bloodthirst.Core.Utils;
using Bloodthirst.System.Quadrant;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Bloodthirst.Core.Collections
{
    /// <summary>
    /// Builder class used to construct the sieve conditions
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public struct SieveFilter<TEntity, TEnum> where TEnum : Enum
    {
        private List<TEntity> _entities;
        private HashSet<TEnum> _flags;
        private EnumSieve<TEntity, TEnum> _targetSieve;

        internal SieveFilter(EnumSieve<TEntity, TEnum> targetSieve)
        {
            this._targetSieve = targetSieve;
            _entities = new List<TEntity>();
            _flags = new HashSet<TEnum>();
        }

        public SieveFilter<TEntity, TEnum> AddConstraint(TEnum flag)
        {
            _flags.Add(flag);
            return this;
        }

        public SieveFilter<TEntity, TEnum> AddConstraints(IEnumerable<TEnum> flags)
        {
            foreach (TEnum f in flags)
            {
                _flags.Add(f);
            }
            return this;
        }

        public void Search()
        {
            // find the smallest sieve bucket to start with
            TEnum minBucket = _flags.First();
            int minCount = _targetSieve.enumToSieveBucket[minBucket].Count;

            IEnumerable<TEnum> firstSkipped = _flags.Skip(1);
            foreach (TEnum f in firstSkipped)
            {
                int curr = _targetSieve.enumToSieveBucket[f].Count;
                if (curr < minCount)
                {
                    minBucket = f;
                    minCount = curr;
                }
            }

            // start with the smallest list that fits a condition
            _entities.Clear();
            _entities.AddRange(_targetSieve.enumToSieveBucket[minBucket]);

            // start filtering according to the rest of the conditions
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                foreach (TEnum f in firstSkipped)
                {
                    Predicate<TEntity> condition = _targetSieve.enumToCondition[f];
                    if (!condition(_entities[i]))
                    {
                        _entities.RemoveAt(i);
                    }
                }
            }
        }

        public IEnumerable<TEntity> GetResult()
        {
            return _entities;
        }

    }
}
