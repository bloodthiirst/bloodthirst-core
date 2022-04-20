using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    internal class BJsonFactory
    {
        internal List<IBJsonFilterInternal> ConverterFilters { get; }

        public BJsonFactory()
        {
            ConverterFilters = new List<IBJsonFilterInternal>()
            {
                // basic types
                new BJsonStringFilter(),
                new BJsonIntFilter(),
                new BJsonFloatFilter(),
                new BJsonBooleanFilter(),

                new BJsonFlagFilter(),
                new BJsonEnumFilter(),

                new BJsonByteFilter(),
                new BJsonUIntFilter(),
                new BJsonULongFilter(),
                new BJsonCharFilter(),

                // system.object
                new BJsonObjectFilter(),


                // type
                new BJsonTypeFilter(),

                // unity object
                new BJsonUnityObjectFilter(),

                // collections
                new BJsonArraySealedElementFilter(),
                new BJsonArrayFilter(),
                new BJsonListSealedElementFilter(),
                new BJsonListFilter(),
                new BJsonDictionaryFilter(),

                // interface or abstract
                new BJsonInterfaceOrAbstractFilter()
            };
        }

        internal IBJsonConverterInternal CreateConverter(Type t)
        {
            for (int i = 0; i < ConverterFilters.Count; i++)
            {
                IBJsonFilterInternal curr = ConverterFilters[i];

                if (curr.CanConvert(t))
                {
                    return curr.GetConverter_Internal(t);
                }
            }

            return new BJsonComplexConverter(t);
        }

    }
}