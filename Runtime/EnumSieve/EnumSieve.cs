using Bloodthirst.Core.EnumLookup;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Core.Collections
{
    public class EnumSieve<TEntity, TEnum> : IReadOnlyList<TEntity> where TEnum : Enum
    {
        private static EnumLookup<TEnum, int> enumToMask = new EnumLookup<TEnum, int>();

        internal EnumLookup<TEnum, List<TEntity>> enumToSieveBucket = new EnumLookup<TEnum, List<TEntity>>();

        internal EnumLookup<TEnum, Predicate<TEntity>> enumToCondition = new EnumLookup<TEnum, Predicate<TEntity>>();

        internal List<TEntity> allEntities = new List<TEntity>();

        public Action<TEntity> OnEntityAdded;
        public Action<TEntity, int> OnEntityAddedWithMask;

        public Action<TEntity> OnEntityRemoved;
        public Action<TEntity, int> OnEntityRemovedWithMask;

        public EnumSieve()
        {
            // convert the enum to bit mask
            for (int i = 0; i < EnumUtils<TEnum>.EnumCount; i++)
            {
                enumToMask[i] = 1 << i;
                enumToSieveBucket[i] = new List<TEntity>();
            }
        }

        public void SetCondition(TEnum flag, Predicate<TEntity> condition)
        {
            enumToCondition[flag] = condition;
        }

        public void Add(TEntity entity)
        {
            allEntities.Add(entity);

            int mask = 0;

            for (int i = 0; i < EnumUtils<TEnum>.EnumCount; i++)
            {
                // check the condition of the sieve
                if (!enumToCondition[i](entity))
                    continue;

                // add the entity to the sieve bucket
                enumToSieveBucket[i].Add(entity);
                mask = mask & enumToMask[i];
            }

            OnEntityAdded?.Invoke(entity);
            OnEntityAddedWithMask?.Invoke(entity, mask);
        }

        public void Remove(TEntity entity)
        {
            allEntities.Remove(entity);

            int mask = 0;

            for (int i = 0; i < enumToMask.Count; i++)
            {
                // check the condition of the sieve
                if (!enumToCondition[i](entity))
                    continue;

                // add the entity to the sieve bucket
                enumToSieveBucket[i].Remove(entity);
                mask = mask & enumToMask[i];
            }

            OnEntityRemoved?.Invoke(entity);
            OnEntityRemovedWithMask?.Invoke(entity, mask);
        }
        public SieveFilter<TEntity, TEnum> GetFilter()
        {
            SieveFilter<TEntity, TEnum> filter = new SieveFilter<TEntity, TEnum>(this);
            return filter;
        }

        #region IReadOnlyList implementation
        public int Count => allEntities.Count;

        public TEntity this[int index] => allEntities[index];
        public IEnumerator<TEntity> GetEnumerator()
        {
            return allEntities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allEntities.GetEnumerator();
        }
        #endregion

    }
}
