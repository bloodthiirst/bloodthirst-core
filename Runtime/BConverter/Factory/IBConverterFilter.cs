using System;

namespace Bloodthirst.BDeepCopy
{
    internal interface IBConverterFilter<TConverter> where TConverter : IBConverter
    {
        bool CanConvert(Type t);
        TConverter GetConverter(Type t);
    }
}
