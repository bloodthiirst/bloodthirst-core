using System;

namespace Bloodthirst.BJson
{
    internal class BJsonInterfaceOrAbstractFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t.IsInterface || t.IsAbstract;
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonInterfaceOrAbstractConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}