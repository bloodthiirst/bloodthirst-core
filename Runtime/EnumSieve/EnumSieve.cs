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
        private EnumLookup<TEnum, int> enumToMask = new EnumLookup<TEnum, int>();

        internal EnumLookup<TEnum, List<TEntity>> enumToSieveBucket = new EnumLookup<TEnum, List<TEntity>>();

        internal EnumLookup<TEnum, Predicate<TEntity>> enumToCondition = new EnumLookup<TEnum, Predicate<TEntity>>();

        public EnumSieve()
        {
            // convert the enum to bit mask
            for (int i = 0; i < EnumLookup<TEnum, int>.EnumCount; i++)
            {
                enumToMask.Set(i, 1 << i);
            }
        }

        public void Add(TEntity entity)
        {
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
            for (int i = 0; i < enumToMask.Count; i++)
            {
                // check the condition of the sieve
                if (!enumToCondition[i](entity))
                    continue;

                // add the entity to the sieve bucket
                enumToSieveBucket[i].Remove(entity);
            }
        }


        public SieveFilter<TEntity, TEnum> Get(TEnum flag)
        {
            SieveFilter<TEntity, TEnum> filter = new SieveFilter<TEntity, TEnum>(this);

            filter.Start(flag);

            return filter;
        }

        public SieveFilter<TEntity, TEnum> Get(IEnumerable<TEnum> flags)
        {
            TEnum minBucket = flags.First();
            int minCount = enumToSieveBucket[minBucket].Count;

            foreach (TEnum f in flags.Skip(1))
            {
                int curr = enumToSieveBucket[f].Count;
                if (curr < minCount)
                {
                    minBucket = f;
                    minCount = curr;
                }
            }

            SieveFilter<TEntity, TEnum> filter = new SieveFilter<TEntity, TEnum>(this);

            filter.Start(minBucket);

            return filter;
        }

    }
}
