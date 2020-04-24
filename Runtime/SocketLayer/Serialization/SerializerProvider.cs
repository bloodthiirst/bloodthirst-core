using Bloodthirst.Core.PersistantAsset;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                if(typeToSerializer == null)
                {
                    typeToSerializer = new Dictionary<Type, object>();
                }

                return typeToSerializer;
            }
        }

        static SerializerProvider()
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => !( t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseNetworkSerializer<>) ))
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

            foreach(var query in types)
            {
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

            if (!TypeToSerializer.ContainsKey(t))
            {
                BaseNetworkSerializer<T> baseSerializer = new BaseNetworkSerializer<T>();

                TypeToSerializer.Add(t, baseSerializer);
            }

            return (INetworkSerializer<T>)TypeToSerializer[t];
        }

    }
}
