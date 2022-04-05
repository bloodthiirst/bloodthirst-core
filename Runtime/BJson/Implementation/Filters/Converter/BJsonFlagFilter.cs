using System;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BJson
{
    internal class BJsonFlagFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t.IsEnum && t.GetCustomAttributes<FlagsAttribute>().Any();
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonFlagConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}