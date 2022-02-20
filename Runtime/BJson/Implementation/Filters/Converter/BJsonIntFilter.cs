using System;

namespace Bloodthirst.BJson
{
    internal class BJsonIntFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(int);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonIntConverter();
        }
    }
}