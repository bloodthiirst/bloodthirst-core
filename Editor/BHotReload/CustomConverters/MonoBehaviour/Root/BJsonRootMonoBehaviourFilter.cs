using Bloodthirst.Core.Utils;
using System;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class BJsonRootMonoBehaviourFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(Component));
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonRootMonoBehaviourConverter(t);
        }


        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}