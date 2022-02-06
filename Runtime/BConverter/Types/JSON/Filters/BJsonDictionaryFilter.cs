using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonDictionaryFilter : IBConverterFilter<IBJsonConverterInternal>
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(IDictionary));
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonDictionaryConverter(t);
        }
    }
}