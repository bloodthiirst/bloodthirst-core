using System;

namespace Bloodthirst.BJson
{
    internal class BJsonTypeFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(Type);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonTypeConverter();
        }
    }
}