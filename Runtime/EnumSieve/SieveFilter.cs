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
    public struct SieveFilter<TEntity, TEnum> where TEnum : Enum
    {
        private List<TEntity> entities;
        private HashSet<TEnum> enums;
        private EnumSieve<TEntity, TEnum> targetSieve;

        internal SieveFilter(EnumSieve<TEntity, TEnum> targetSieve)
        {
            this.targetSieve = targetSieve;
            entities = new List<TEntity>();
            enums = new HashSet<TEnum>();

        }

        internal SieveFilter<TEntity, TEnum> Start(TEnum flag)
        {
            List<TEntity> flagEntities = targetSieve.enumToSieveBucket[flag];

            enums.Add(flag);
            entities.AddRange(flagEntities);

            return this;
        }

        public SieveFilter<TEntity, TEnum> Append(TEnum flag)
        {
            if (!enums.Add(flag))
                return this;

            Predicate<TEntity> condition = targetSieve.enumToCondition[flag];

            for(int i = entities.Count - 1; i >= 0; i--)
            {
                if( !condition(entities[i]))
                {
                    entities.RemoveAt(i);
                }
            }

            return this;
        }

        public IReadOnlyList<TEntity> GetResult()
        {
            return entities;
        }
            
    }
}
