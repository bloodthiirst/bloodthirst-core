using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace Bloodthirst.System.Quest.Editor
{
    public partial class BQuestSystemSettings
    {
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
        public static JsonSerializerSettings GetSerializerSettings()
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
    }
}
