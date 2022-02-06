using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonListSealedElementFilter : IBConverterFilter<IBJsonConverterInternal>
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(IList)) && TypeUtils.IsPureValueType(t.GetGenericArguments()[0]);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonListSealedElementConverter(t);
        }
    }
}