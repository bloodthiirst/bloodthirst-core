using System;

namespace Bloodthirst.BJson
{
    internal class BJsonIntFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(int);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonIntConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}