using Bloodthirst.BType;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Editor.BInspector
{
    public static class BInspectorPropertyFilterProvider
    {
        private static Dictionary<Type, BTypeData> PropertyFilters { get; set; }

        private static List<IBInspectorPropertyFilter> AllFilters { get; set; }

        static BInspectorPropertyFilterProvider()
        {
            PropertyFilters = new Dictionary<Type, BTypeData>();

            AllFilters = new List<IBInspectorPropertyFilter>()
            {
                new BinspectorUnityObjectPropertyFilter(),
                new BInspectorIgnorePropertyFilter()
            };
        }

        public static BTypeData GetFilteredProperties(Type t)
        {
            if(!PropertyFilters.TryGetValue(t , out BTypeData filteredProps))
            {
                filteredProps = BTypeProvider.GetOrCreate(t);

                foreach(IBInspectorPropertyFilter filter in AllFilters)
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
