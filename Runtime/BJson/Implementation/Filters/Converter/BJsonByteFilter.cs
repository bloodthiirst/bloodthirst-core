using System;

namespace Bloodthirst.BJson
{
    internal class BJsonByteFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(byte);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonByteConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}