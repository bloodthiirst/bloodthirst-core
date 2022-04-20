using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BJson
{
    internal class BJsonListFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            if (!TypeUtils.IsSubTypeOf(t, typeof(IList)))
                return false;

            Type[] allListArgs = t.GetGenericArguments();

            if (allListArgs.Length == 0)
                return false;

            return true;

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