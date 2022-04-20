using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BJson
{
    internal class BJsonListSealedElementFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            if (!TypeUtils.IsSubTypeOf(t, typeof(IList)))
                return false;

            Type[] allListArgs = t.GetGenericArguments();

            if (allListArgs.Length == 0)
                return false;

            Type elemType = allListArgs[0];

            return TypeUtils.IsPureValueType(elemType);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonListSealedElementConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}