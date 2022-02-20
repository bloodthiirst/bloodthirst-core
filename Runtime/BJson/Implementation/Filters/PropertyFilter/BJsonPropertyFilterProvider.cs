using Bloodthirst.BType;
using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    public static class BJsonPropertyFilterProvider
    {
        private static Dictionary<Type, BTypeData> PropertyFilters { get; set; }

        private static List<IBJsonPropertyFilter> AllFilters { get; set; }

        static BJsonPropertyFilterProvider()
        {
            PropertyFilters = new Dictionary<Type, BTypeData>();

            AllFilters = new List<IBJsonPropertyFilter>()
            {
                new UnityObjectPropertyFilter(),
                new JsonIgnorePropertyFilter(),
                new Vector2PropertyFilter(),
                new Vector3PropertyFilter()
            };
        }

        public static BTypeData GetFilteredProperties(Type t)
        {
            if(!PropertyFilters.TryGetValue(t , out BTypeData filteredProps))
            {
                filteredProps = BTypeProvider.GetOrCreate(t);

                foreach(IBJsonPropertyFilter filter in AllFilters)
                {
                    if(filter.CanFilter(t))
                    {
                        filteredProps = filter.FilteredProperties(filteredProps);
                    }
                }

                PropertyFilters.Add(t, filteredProps);
                return filteredProps;
            }

            return filteredProps;
        }
    }
}
