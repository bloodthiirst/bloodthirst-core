using System;

namespace Bloodthirst.BDeepCopy
{
    internal interface IBConverterFilter<TConverter> : IBConverterFilter where TConverter : IBConverter
    {
        bool CanConvert(Type t);
        TConverter GetConverter(Type t);
    }

    internal interface IBConverterFilter
    {
        bool CanConvert(Type t);
        IBConverter GetConverter(Type t);
    }
}
