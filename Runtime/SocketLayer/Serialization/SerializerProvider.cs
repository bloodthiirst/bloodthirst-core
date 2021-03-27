using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using Bloodthirst.Socket.Serializer;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bloodthirst.Socket.Serialization
{
#if UNITY_EDITOR
    [InlineEditor]
    public struct SerializerInfo
    {
        [ShowInInspector]
        [HorizontalGroup]
        [LabelWidth(70)]
        public string TypeName { get; set; }

        [ShowInInspector]
        [HorizontalGroup]
        [LabelWidth(90)]
        public string SerializerName { get; set; }
    }
#endif

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class SerializerProvider : SingletonScriptableObject<SerializerProvider>
    {
        private static Dictionary<Type, INetworkSerializer> typeToSerializer;

        private static Dictionary<Type, INetworkSerializer> TypeToSerializer
        {
            get
            {
                if (typeToSerializer == null)
                {
                    typeToSerializer = new Dictionary<Type, INetworkSerializer>();
                }

                return typeToSerializer;
            }
        }


#if UNITY_EDITOR
        private static List<SerializerInfo> serializedTypes;

        /// <summary>
        /// Container the list of the serialized
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private List<SerializerInfo> SerializedTypes => serializedTypes;

        private static void RefreshInfo()
        {
            serializedTypes = serializedTypes.CreateOrClear();

            foreach (KeyValuePair<Type, INetworkSerializer> kv in TypeToSerializer)
            {
                serializedTypes.Add(new SerializerInfo() { TypeName = kv.Key.GetNiceName(), SerializerName = kv.Value.GetType().GetNiceName() });
            }
        }
#endif

        static SerializerProvider()
        {
            var types = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(DefaultNetworkSerializer<>)))
                .Where(t => !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EmptyStructNetworkSerializer<>)))
                .Select(t =>
                {
                    List<Type> serializeInterface = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INetworkSerializer<>)).ToList();

                    if (serializeInterface.Count == 0)
                        return null;

                    return new Tuple<Type, Type>(serializeInterface[0].GetGenericArguments()[0], t);
                })
                .Where(t => t != null)
                .ToList();

            // spawn the specific serializers
            for (int i = 0; i < types.Count; i++)
            {
                var query = types[i];

                INetworkSerializer serializer = (INetworkSerializer)Activator.CreateInstance(query.Item2);

                TypeToSerializer.Add(query.Item1, serializer);
            }

            // spawn serializers for emptyStruct
            List<Type> emptyTypes = TypeUtils.AllTypes
                    .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IEmptyStruct)))
                    .Where(t => t != typeof(IEmptyStruct))
                    .ToList();

            for (int i = 0; i < emptyTypes.Count; i++)
            {
                Type emptyStruct = emptyTypes[i];
                Type constructedType = typeof(EmptyStructNetworkSerializer<>).MakeGenericType(emptyStruct);
                INetworkSerializer serializer = (INetworkSerializer)Activator.CreateInstance(constructedType);
                TypeToSerializer.Add(emptyStruct, serializer);
            }
#if UNITY_EDITOR
            RefreshInfo();
#endif
        }



        /// <summary>
        /// Get the appropriate serializer per type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static INetworkSerializer<T> Get<T>()
        {
            Type t = typeof(T);

            if (!TypeToSerializer.TryGetValue(t, out INetworkSerializer alreadyCreated))
            {
                INetworkSerializer<T> serializer = new DefaultNetworkSerializer<T>();

                TypeToSerializer.Add(t, serializer);
#if UNITY_EDITOR
                RefreshInfo();
#endif
                return serializer;
            }

            return (INetworkSerializer<T>)alreadyCreated;
        }

    }
}
