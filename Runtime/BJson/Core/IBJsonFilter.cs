using System;

namespace Bloodthirst.BJson
{
    internal interface IBJsonFilter
    {
        bool CanConvert(Type t);
        IBJsonConverterInternal GetConverter(Type t);
    }
}
