using System;

namespace Bloodthirst.BJson
{
    internal class BJsonInterfaceOrAbstractFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return t.IsInterface || t.IsAbstract;
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonInterfaceOrAbstractConverter(t);
        }
    }
}