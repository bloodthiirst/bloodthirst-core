using System;

namespace Bloodthirst.BJson
{
    internal class BJsonArrayFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t.IsArray;
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonArrayConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}