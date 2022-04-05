using System;

namespace Bloodthirst.BJson
{
    internal class BJsonFloatFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(float);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonFloatConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}