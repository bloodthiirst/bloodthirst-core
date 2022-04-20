using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.BJson
{
    internal class BJsonArraySealedElementFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            if (t.IsArray)
            {
                Type elemType = t.GetElementType();
                return TypeUtils.IsPureValueType(elemType);
            }

            return false;
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonArraySealedElementConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}