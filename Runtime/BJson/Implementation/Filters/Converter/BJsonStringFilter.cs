using System;

namespace Bloodthirst.BJson
{
    internal class BJsonStringFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(string);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonStringConverter();
        }
    }
}