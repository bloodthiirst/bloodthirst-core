using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BJson
{
    internal class BJsonUnityObjectFilter : IBJsonFilter
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(UnityEngine.Object));
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            return new BJsonUnityObjectConverter(t);
        }
    }
}