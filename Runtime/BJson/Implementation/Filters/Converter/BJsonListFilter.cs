using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BJson
{
    internal class BJsonListFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            Type[] args = t.GetGenericArguments();

            return TypeUtils.IsSubTypeOf(t, typeof(IList)) &&
                    args.Length != 0;
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonListConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}