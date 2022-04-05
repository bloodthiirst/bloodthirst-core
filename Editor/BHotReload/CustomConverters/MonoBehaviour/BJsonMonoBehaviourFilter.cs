using Bloodthirst.Core.Utils;
using System;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class BJsonMonoBehaviourFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(Component));
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonMonoBehaviourConverter(t);
        }


        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}