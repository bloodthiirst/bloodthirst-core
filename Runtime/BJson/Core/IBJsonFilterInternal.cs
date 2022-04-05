using System;

namespace Bloodthirst.BJson
{

    public interface IBJsonFilter
    {
        bool CanConvert(Type t);
        IBJsonConverter GetConverter(Type t);
    }

    public interface IBJsonFilterInternal : IBJsonFilter
    {
        IBJsonConverterInternal GetConverter_Internal(Type t);
    }


}
