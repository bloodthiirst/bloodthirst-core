using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using Bloodthirst.Socket.Serializer;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
#if ODIN_INSPECTOR
	using Sirenix.Serialization;
#endif
#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Bloodthirst.Socket.Serialization
{
#if UNITY_EDITOR && ODIN_INSPECTOR
    [InlineEditor]
#endif
    public struct SerializerInfo
    {
        
#if ODIN_INSPECTOR
        [ShowInInspector]
        [HorizontalGroup]
        [LabelWidth(70)]
#endif
        public string TypeName { get; set; }


#if ODIN_INSPECTOR
        [ShowInInspector]
        [HorizontalGroup]
        [LabelWidth(90)]
#endif
        public string SerializerName { get; set; }
    }

    public class SerializerProvider : SingletonScriptableObject<SerializerProvider>
    {
        
#if ODIN_INSPECTOR
[OdinSerialize]
#endif

        [HideInInspector]
        private Dictionary<Type, INetworkSerializer> typeToSerializer;


#if UNITY_EDITOR

        private List<SerializerInfo> serializedTypes;

        /// <summary>
        /// Container the list of the serialized
        /// </summary>
        
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        
#if ODIN_INSPECTOR
[ReadOnly]
#endif

        private List<SerializerInfo> SerializedTypes
        {
            get
            {
                RefreshInfo();
                return serializedTypes;
            }
        }


        private void RefreshInfo()
        {
            serializedTypes = serializedTypes.CreateOrClear();

            foreach (KeyValuePair<Type, INetworkSerializer> kv in typeToSerializer)
            {
                serializedTypes.Add(new SerializerInfo() { TypeName = TypeUtils.GetNiceName(kv.Key), SerializerName = TypeUtils.GetNiceName(kv.Value.GetType()) });
            }
        }
#endif

        
#if ODIN_INSPECTOR
[Button]
#endif

        public void Initialize()
        {
            if (typeToSerializer == null)
            {
                typeToSerializer = new Dictionary<Type, INetworkSerializer>();
            }
            else
            {
                typeToSerializer.Clear();
            }

            List<Tuple<Type, Type>> types = TypeUtils.AllTypes
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

                typeToSerializer.Add(query.Item1, serializer);
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
                typeToSerializer.Add(emptyStruct, serializer);
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

            if (!Instance.typeToSerializer.TryGetValue(t, out INetworkSerializer alreadyCreated))
            {
                INetworkSerializer<T> serializer = new DefaultNetworkSerializer<T>();

                Instance.typeToSerializer.Add(t, serializer);
#if UNITY_EDITOR
                Instance.RefreshInfo();
#endif
                return serializer;
            }

            return (INetworkSerializer<T>)alreadyCreated;
        }

    }
}
