using System;

namespace Bloodthirst.BJson
{
    internal class BJsonBooleanFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(bool);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonBooleanConverter();
        }
    }
}