using System;

namespace Bloodthirst.BJson
{
    internal class BJsonFloatFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(float);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonFloatConverter();
        }
    }
}