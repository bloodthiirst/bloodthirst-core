using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.BJson
{
    internal class BJsonTypeFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(Type));
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonTypeConverter();
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}