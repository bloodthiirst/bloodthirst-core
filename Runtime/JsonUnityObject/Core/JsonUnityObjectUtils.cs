using Bloodthirst.Core.Utils;
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
            // get pooled settings
            JsonSerializerSettings settings = JsonUnityObjectSettings.GetSettings();

            // custom context
            // make it a struct to minimize GC
            CustomContext ctx = new CustomContext()
            {
                UnityObjects = unityRefsList
            };

            settings.Context = new StreamingContext(StreamingContextStates.Other, ctx);

            // deserialize into the object
            JsonConvert.PopulateObject(jsonString, unityObjectThis, settings);

            // return the pooled settings
            JsonUnityObjectSettings.ReturnSettings(settings);
        }

        /// <summary>
        /// Utility method to serialize the unity object into valid JSON while keeping the unity object refs
        /// </summary>
        /// <returns></returns>
        public static string SerializeUnityObject(UnityEngine.Object unityObjectThis, List<UnityEngine.Object> unityRefsList)
        {
            // init/clear the list of unity object references
            unityRefsList = unityRefsList.CreateOrClear();

            // get pooled settings
            JsonSerializerSettings settings = JsonUnityObjectSettings.GetSettings();

            // custom context
            // make it a struct to minimize GC
            CustomContext ctx = new CustomContext()
            {
                UnityObjects = unityRefsList
            };

            settings.Context = new StreamingContext(StreamingContextStates.Other, ctx);

            // serialize the object into JSON
            string res = JsonConvert.SerializeObject(unityObjectThis, settings);

            // return the pooled settings
            JsonUnityObjectSettings.ReturnSettings(settings);

            return res;
        }
    }
}
