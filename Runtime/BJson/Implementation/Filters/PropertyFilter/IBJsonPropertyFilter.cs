using Bloodthirst.BType;
using System;

namespace Bloodthirst.BJson
{
    internal interface IBJsonPropertyFilter<T> : IBJsonPropertyFilter
    {
    }

    internal interface IBJsonPropertyFilter
    {
        bool CanFilter(Type t);
        BTypeData FilteredProperties(BTypeData typeData);
    }

}
