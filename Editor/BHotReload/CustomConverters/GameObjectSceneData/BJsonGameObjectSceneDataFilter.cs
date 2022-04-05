using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BHotReload;
using System;
using System.Collections;

namespace Bloodthirst.BJson
{
    internal class BJsonGameObjectSceneDataFilter : IBJsonFilterInternal
    {
        public bool CanConvert(Type t)
        {
            return t == typeof(GameObjectSceneData);
        }

        public IBJsonConverterInternal GetConverter_Internal(Type t)
        {
            return new BJsonGameObjectSceneDataConverter(t);
        }


        public IBJsonConverter GetConverter(Type t)
        {
            return GetConverter_Internal(t);
        }
    }
}