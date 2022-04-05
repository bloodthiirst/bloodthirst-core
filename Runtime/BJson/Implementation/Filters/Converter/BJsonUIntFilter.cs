using System;

namespace Bloodthirst.BJson
{
    internal class BJsonUIntFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(uint);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonUIntConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}