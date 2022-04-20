using System;

namespace Bloodthirst.BJson
{
    internal class BJsonObjectFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(object);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonObjectConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}