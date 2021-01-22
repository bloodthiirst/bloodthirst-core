﻿using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Bloodthirst.Socket.Serializer
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class SerializerProvider : SingletonScriptableObject<SerializerProvider>
    {
        /// <summary>
        /// Container that has all the serializers used by the networking layer
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private static Dictionary<Type, object> typeToSerializer;

        private static Dictionary<Type, object> TypeToSerializer
        {
            get
            {
                if (typeToSerializer == null)
                {
                    typeToSerializer = new Dictionary<Type, object>();
                }

                return typeToSerializer;
            }
        }

        static SerializerProvider()
        {
            var types = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseNetworkSerializer<>)))
                .Select(t =>
                {
                    List<Type> serializeInterface = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INetworkSerializer<>)).ToList();

                    if (serializeInterface.Count == 0)
                        return null;

                    return new { ValueType = serializeInterface[0].GetGenericArguments()[0], SerializerType = t };
                })
                .Where(t => t != null)
                .ToList();

            foreach (var query in types)
            {
                Debug.Log("Type to serialize : " + query.ValueType.Name + " , Serializer class : " + query.SerializerType.Name);

                object typeSerializer = Activator.CreateInstance(query.SerializerType);

                TypeToSerializer.Add(query.ValueType, typeSerializer);
            }
        }

        /// <summary>
        /// Get the appropriate serializer per type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static INetworkSerializer<T> Get<T>()
        {
            Type t = typeof(T);

            if (!TypeToSerializer.TryGetValue(t, out object ser))
            {
                BaseNetworkSerializer<T> baseSerializer = new BaseNetworkSerializer<T>();

                TypeToSerializer.Add(t, baseSerializer);

                return baseSerializer;
            }

            return (INetworkSerializer<T>)ser;
        }

    }
}
