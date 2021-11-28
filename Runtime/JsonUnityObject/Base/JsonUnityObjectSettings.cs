using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;

namespace Bloodthirst.JsonUnityObject
{
    [InitializeOnLoad]
    public static class JsonUnityObjectSettings
    {
        /// <summary>
        /// Pool to recycle settings instances to minimize GC when serializing
        /// </summary>
        private static Queue<JsonSerializerSettings> pooledSettings = new Queue<JsonSerializerSettings>();

        /// <summary>
        /// List of members to ignore when serializing
        /// </summary>
        private static readonly List<string> monoBehaviourIgnorableMembers = new List<string>()
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

        /// <summary>
        /// ReadOnlyList of members to ignore when serializing
        /// </summary>
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

        /// <summary>
        /// Get a pooled instance of the serialization settings
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetSettings()
        {
            if(pooledSettings.Count == 0)
            {
                return CreateSerializerSettings();
            }

            return pooledSettings.Dequeue();
        }

        /// <summary>
        /// Return the instance of the serialization settings
        /// </summary>
        /// <param name="settings"></param>
        public static void ReturnSettings(JsonSerializerSettings settings)
        {
            pooledSettings.Enqueue(settings);
        }
    }
}
