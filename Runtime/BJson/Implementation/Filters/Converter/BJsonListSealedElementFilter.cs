using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BJson
{
    internal class BJsonListSealedElementFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            Type[] args = t.GetGenericArguments();

            return TypeUtils.IsSubTypeOf(t, typeof(IList)) &&
                    args.Length != 0 &&
                    TypeUtils.IsPureValueType(args[0]);
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonListSealedElementConverter(t);
        }
    }
}