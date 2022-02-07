using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonUnityObjectFilter : IBConverterFilter<IBJsonConverterInternal>
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