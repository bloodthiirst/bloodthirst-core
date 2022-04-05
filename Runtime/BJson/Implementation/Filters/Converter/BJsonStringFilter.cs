using System;

namespace Bloodthirst.BJson
{
    internal class BJsonStringFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(string);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonStringConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}