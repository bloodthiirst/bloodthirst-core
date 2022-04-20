using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.BJson
{
    internal class BJsonUnityObjectFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(UnityEngine.Object));
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonUnityObjectConverter(t);
        }

        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}