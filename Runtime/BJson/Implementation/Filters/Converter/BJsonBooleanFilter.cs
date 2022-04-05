using System;

namespace Bloodthirst.BJson
{
    internal class BJsonBooleanFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(bool);
        }
        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonBooleanConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter(t);
        }

    }
}