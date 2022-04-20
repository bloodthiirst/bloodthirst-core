using System;

namespace Bloodthirst.BJson
{
    internal class BJsonCharFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(char);
        }
        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonCharConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter(t);
        }

    }
}