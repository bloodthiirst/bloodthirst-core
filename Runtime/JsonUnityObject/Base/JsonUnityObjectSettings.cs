using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.JsonUnityObject
{
    [InitializeOnLoad]
    public static class JsonUnityObjectSettings
    {
        private static Queue<JsonSerializerSettings> pooledSettings = new Queue<JsonSerializerSettings>();

        private static List<string> monoBehaviourIgnorableMembers = new List<string>()
        {
            "rigidbody",
            "rigidbody2D",
            "camera",
            "light",
            "animation",
            "renderer",
            "constantForce",
            "audio",
            "networkView",
            "collider",
            "collider2D",
            "hingeJoint",
            "particleSystem",
            "hideFlags",
            "name",
            "useGUILayout",
            "runInEditMode",
            "enabled",
            "gameObject",
            "allowPrefabModeInPlayMode",
            "isActiveAndEnabled",
            "transform",
            "tag"
        };
        public static IReadOnlyList<string> MonoBehaviourIgnorableMembers => monoBehaviourIgnorableMembers;

        /// <summary>
        /// Resolver used to skip some unity specific fields
        /// </summary>
        private static UnityObjectContractResolver UnityObjectResolver = new UnityObjectContractResolver();

        /// <summary>
        /// Converters used for custom data
        /// </summary>
        private static List<JsonConverter> Converters = new List<JsonConverter>()
        {
            new Vector2Converter(),
            new UnityObjectJsonConverter()
        };

        /// <summary>
        /// Returns the settings used to serialize the node data
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
#if UNITY_EDITOR
                Formatting = Formatting.Indented,
#else
                Formatting = Formatting.None,
#endif
                ContractResolver = UnityObjectResolver,
                Converters = Converters
            };
        }

        public static JsonSerializerSettings GetSettings()
        {
            if(pooledSettings.Count == 0)
            {
                return CreateSerializerSettings();
            }

            return pooledSettings.Dequeue();
        }

        public static void ReturnSettings(JsonSerializerSettings settings)
        {
            pooledSettings.Enqueue(settings);
        }
    }
}
