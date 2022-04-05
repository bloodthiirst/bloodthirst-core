using System;

namespace Bloodthirst.BJson
{
    internal class BJsonEnumFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t.IsEnum;
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonEnumConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}