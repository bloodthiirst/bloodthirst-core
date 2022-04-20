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
                // general
                new JsonIgnorePropertyFilter(),

                // common structs
                new Vector2PropertyFilter(),
                new Vector3PropertyFilter(),
                new Vector4PropertyFilter(),
                new QuaternionPropertyFilter(),
                new Matrix4x4PropertyFilter(),
                new RayPropertyFilter(),
                new ColorPropertyFilter(),
                new Color32PropertyFilter(),
                new RectPropertyFilter(),

                // unity objects
                new TransformPropertyFilter(),
                new LayoutGroupPropertyFilter(),
                new RectTransformPropertyFilter(),
                new UnityComponentPropertyFilter(),
                new UnityObjectPropertyFilter(),
                new BJsonTMP_TextPropertyFilter(),
                new BJsonRectOffsetPropertyFilter(),

                // input system
                new BJsonBaseInputModulePropertyFilter()
            };
        }

        public static BTypeData GetFilteredProperties(Type t)
        {
            if (!PropertyFilters.TryGetValue(t, out BTypeData filteredProps))
            {
                filteredProps = BTypeProvider.GetOrCreate(t);

                foreach (IBJsonPropertyFilter filter in AllFilters)
                {
                    if (filter.CanFilter(t))
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
