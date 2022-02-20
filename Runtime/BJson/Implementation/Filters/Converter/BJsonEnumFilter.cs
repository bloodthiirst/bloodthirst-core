using System;

namespace Bloodthirst.BJson
{
    internal class BJsonEnumFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t.IsEnum;
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonEnumConverter(t);
        }
    }
}