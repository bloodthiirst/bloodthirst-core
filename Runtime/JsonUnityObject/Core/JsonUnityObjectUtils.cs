using Bloodthirst.Core.Utils;
using Bloodthirst.System.Quest.Editor;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bloodthirst.JsonUnityObject
{
    public static class JsonUnityObjectUtils
    {
        /// <summary>
        /// Utility method to deserialize the JSON data into the unity object
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="unityObjectThis"></param>
        /// <param name="unityRefsList"></param>
        public static void DeserializeUnityObject(string jsonString, UnityEngine.Object unityObjectThis, List<UnityEngine.Object> unityRefsList)
        {
            JsonSerializerSettings settings = JsonUnityObjectSettings.GetSettings();

            CustomContext ctx = new CustomContext()
            {
                UnityObjects = unityRefsList
            };

            settings.Context = new StreamingContext(StreamingContextStates.Other, ctx);

            JsonConvert.PopulateObject(jsonString, unityObjectThis, settings);

            JsonUnityObjectSettings.ReturnSettings(settings);
        }

        /// <summary>
        /// Utility method to serialize the unity object into valid JSON while keeping the unity object refs
        /// </summary>
        /// <returns></returns>
        public static string SerializeUnityObject(UnityEngine.Object unityObjectThis, List<UnityEngine.Object> unityRefsList)
        {
            unityRefsList = unityRefsList.CreateOrClear();

            JsonSerializerSettings settings = JsonUnityObjectSettings.GetSettings();

            CustomContext ctx = new CustomContext()
            {
                UnityObjects = unityRefsList
            };

            settings.Context = new StreamingContext(StreamingContextStates.Other, ctx);

            string res = JsonConvert.SerializeObject(unityObjectThis, settings);

            JsonUnityObjectSettings.ReturnSettings(settings);

            return res;
        }
    }
}
