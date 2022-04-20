using System;

namespace Bloodthirst.BJson
{
    internal class BJsonULongFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(ulong);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonULongConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}