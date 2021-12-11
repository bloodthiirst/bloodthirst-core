using Bloodthirst.Core.EnumLookup;
using Bloodthirst.System.Quadrant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Core.Collections
{
    public class EnumSieve<TEntity, TEnum> where TEnum : Enum
    {
        private static EnumLookup<TEnum, int> enumToMask = new EnumLookup<TEnum, int>();

        internal EnumLookup<TEnum, List<TEntity>> enumToSieveBucket = new EnumLookup<TEnum, List<TEntity>>();

        internal EnumLookup<TEnum, Predicate<TEntity>> enumToCondition = new EnumLookup<TEnum, Predicate<TEntity>>();

        internal List<TEntity> allEntities = new List<TEntity>();

        public static int GetMask(IEnumerable<TEnum> flags)
        {
            int mask = 0;

            foreach (TEnum f in flags)
            {
                mask &= enumToMask[f];
            }

            return mask;
        }

        public EnumSieve()
        {
            // convert the enum to bit mask
            for (int i = 0; i < EnumLookup<TEnum, int>.EnumCount; i++)
            {
                enumToMask.Set(i , 1 << i);
                enumToSieveBucket.Set(i, new List<TEntity>());
            }
        }

        public void SetCondition(TEnum flag , Predicate<TEntity> condition)
        {
            enumToCondition.Set(flag, condition);
        }

        public void Add(TEntity entity)
        {
            allEntities.Add(entity);

            for (int i = 0; i < enumToSieveBucket.Count; i++)
            {
                // check the condition of the sieve
                if (!enumToCondition[i](entity))
                    continue;

                // add the entity to the sieve bucket
                enumToSieveBucket[i].Add(entity);
            }
        }

        public void Remove(TEntity entity)
        {
            allEntities.Remove(entity);

            for (int i = 0; i < enumToMask.Count; i++)
            {
                // check the condition of the sieve
                if (!enumToCondition[i](entity))
                    continue;

                // add the entity to the sieve bucket
                enumToSieveBucket[i].Remove(entity);
            }
        }
        public SieveFilter<TEntity, TEnum> GetFilter()
        {
            SieveFilter<TEntity, TEnum> filter = new SieveFilter<TEntity, TEnum>(this);
            return filter;
        }

    }
}
