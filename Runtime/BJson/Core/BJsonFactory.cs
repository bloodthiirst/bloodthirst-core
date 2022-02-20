using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    internal class BJsonFactory
    {
        internal List<IBJsonFilter> ConverterFilters { get; }

        public BJsonFactory()
        {
            ConverterFilters = new List<IBJsonFilter>()
            {
                // basic types
                new BJsonStringFilter(),
                new BJsonEnumFilter(),
                new BJsonIntFilter(),
                new BJsonFloatFilter(),
                new BJsonBooleanFilter(),

                // type
                new BJsonTypeFilter(),

                // unity object
                new BJsonUnityObjectFilter(),

                // collections
                new BJsonDictionaryFilter(),
                new BJsonListSealedElementFilter(),
                new BJsonListFilter(),

                // interface or abstract
                new BJsonInterfaceOrAbstractFilter()
            };
        }

        internal IBJsonConverterInternal CreateConverter(Type t)
        {
            for (int i = 0; i < ConverterFilters.Count; i++)
            {
                IBJsonFilter curr = ConverterFilters[i];

                if (curr.CanConvert(t))
                {
                    return curr.GetConverter(t);
                }
            }

            return new BJsonComplexConverter(t);
        }

    }
}